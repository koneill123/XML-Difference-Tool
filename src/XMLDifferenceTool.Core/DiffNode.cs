#nullable disable
using System.Xml;
using System.Xml.Linq;

namespace XMLDifferenceTool.Core;

/// <summary>
/// A generic class for storing an element or attribute from an XML file.
/// </summary>
public class DiffNode
{
    public string Name { get; private set; }

    public string Value { get; private set; }

    public List<DiffNode> Children = new List<DiffNode>();

    public string Tag { get; private set; }

    public string RawTag { get; private set; }

    /// <summary>
    /// True when the document was loaded with literal prefixes (see <see cref="NamespaceHandling"/>). Names then
    /// contain <see cref="XmlLoader.PrefixSeparator"/> where the file has a colon, and are shown with the colon.
    /// </summary>
    public bool LiteralPrefixes { get; private set; }

    // The name with its prefix as written in the file, for example xlink:href or xmlns:xlink. Null for nodes
    // that have nothing better to show than Name (literal-prefix documents, empty placeholder nodes).
    private string displayName;

    /// <summary>
    /// The name as it should be shown to the user: with the prefix used in the file (xlink:href, xmlns:xlink)
    /// rather than the namespace URI that <see cref="Name"/> holds for attributes in a namespace.
    /// Comparison uses <see cref="Tag"/>, never this.
    /// </summary>
    public string DisplayName => this.displayName
        ?? (this.LiteralPrefixes && this.Name != null
            ? this.Name.Replace(XmlLoader.PrefixSeparator, ':')
            : this.Name);

    public NodeKind Kind { get; private set; }

    public DiffKindState ChangeType { get; set; } = DiffKindState.None;
    public DiffKindState ChildrenChangeType { get; set; } = DiffKindState.None;

    public MatchRepeateElementsBy MatchRules { get; internal set; } = MatchRepeateElementsBy.OrderInFile;

    public int LineNumber { get; private set; }

    private Dictionary<Tuple<NodeKind, string>,int> childCounts = new Dictionary<Tuple<NodeKind, string>, int>();

    public DiffNode(string name, NodeKind dataType, MatchRepeateElementsBy matchRules, int lineNumber) 
                    : this(name, name, dataType, matchRules, lineNumber)
    {
    }

    public DiffNode(string name, string tag, NodeKind dataType, MatchRepeateElementsBy matchRules, int lineNumber)
    {
        this.Name = name;
        this.Kind = dataType; 
        this.Tag = tag;
        this.RawTag = tag;
        this.MatchRules = matchRules;
        this.LineNumber = lineNumber;
    }

    public DiffNode(XDocument xmlFile, MatchRepeateElementsBy matchRules)
                : this(xmlFile.Root, NodeKind.Element, matchRules, 0, xmlFile.Annotation<LiteralPrefixesMarker>() != null)
    {
    }

    public DiffNode(XElement xelement, NodeKind dataType, MatchRepeateElementsBy matchRules, int currentDepth, bool literalPrefixes = false)
        : this(xelement.Name.LocalName, xelement.Name.LocalName, dataType, matchRules, -1)
    {
        this.LiteralPrefixes = literalPrefixes;
        this.displayName = literalPrefixes ? null : FormatName(xelement.Name, xelement);
        this.Value = (!HasDirectText(xelement) || xelement.Value == null) ? "" : xelement.Value.ToString();
        this.LineNumber = GetLine(xelement);

        currentDepth++;
        if (currentDepth > Helpers.MaxDepth)
        {
            throw new InvalidDataException(string.Format("The XML is nested more than {0} levels deep and can't be compared.", Helpers.MaxDepth));
        }

        foreach (XAttribute attribute in xelement.Attributes())
        {
            DiffNode attributeData = new DiffNode(attribute.Name.ToString(), NodeKind.Attribute, matchRules, -1);
            attributeData.LiteralPrefixes = literalPrefixes;
            attributeData.displayName = literalPrefixes ? null : FormatAttributeName(attribute);
            attributeData.Value = attribute.Value;
            this.AddChild(attributeData);
        }

        this.Tag = this.Name;

        foreach (XElement xe in xelement.Elements())
        {
            this.AddChild(new DiffNode(xe, NodeKind.Element, matchRules, currentDepth, literalPrefixes));
        }
    }

    /// <summary>
    /// An element or attribute name with the prefix the file uses for its namespace: xlink:href, svg:rect.
    /// Names with no namespace, and names in the default namespace, show just the local name. LINQ to XML keeps only
    /// the namespace, not the prefix each name was written with, so if a document declares two prefixes for the same
    /// namespace the first one found is shown.
    /// </summary>
    private static string FormatName(XName name, XElement scope)
    {
        if (name.Namespace == XNamespace.None)
        {
            return name.LocalName;
        }

        string prefix = scope?.GetPrefixOfNamespace(name.Namespace);
        if (prefix == null)
        {
            // An element in the default namespace (xmlns="...") has no prefix to show
            if (scope != null && scope.GetDefaultNamespace() == name.Namespace)
            {
                return name.LocalName;
            }

            return name.ToString();     // no prefix found; fall back to {namespace}name rather than lose the namespace
        }

        return prefix.Length == 0 ? name.LocalName : prefix + ":" + name.LocalName;
    }

    private static string FormatAttributeName(XAttribute attribute)
    {
        // A namespace declaration is stored as an attribute named {http://www.w3.org/2000/xmlns/}prefix
        if (attribute.Name.Namespace == XNamespace.Xmlns)
        {
            return "xmlns:" + attribute.Name.LocalName;
        }

        return FormatName(attribute.Name, attribute.Parent);
    }

    private static int GetLine(XObject node)
    { 
        return node is IXmlLineInfo info && info.HasLineInfo() ? info.LineNumber : 0;
    }

    private string GetUniqueTag(DiffNode diffNode)
    {
        string newTag = diffNode.Kind == NodeKind.Element && diffNode.MatchRules == MatchRepeateElementsBy.NameAndValue ? diffNode.Tag + Helpers.XmlSeparator + diffNode.Value : diffNode.Tag;

        // Slow compare for testing long operations.
        //int index = 0;
        //while (this.Children.Any(c => c.Tag == newTag && c.Kind == diffNode.Kind))
        //{
        //    newTag = Tag + Helpers.XmlSeparator + index;
        //    index++;
        //}

        // fast compare
        Tuple<NodeKind, string> key = new Tuple<NodeKind, string>(diffNode.Kind, newTag);
        if (this.childCounts.TryGetValue(key, out int nextIndex))
        {
            newTag = newTag + Helpers.XmlSeparator + nextIndex;
            this.childCounts[key] = nextIndex + 1;
        }
        else
        {
            this.childCounts[key] = 1;
        }
        return newTag;
    }

    private void AddChild(DiffNode newChild)
    {
        newChild.Tag = GetUniqueTag(newChild);
        this.Children.Add(newChild);
    }

    public List<DiffNode> ChildrenOfDiffKindState(DiffKindState displayDiffKindStates)
    {
        return this.Children.Where(c => (c.ChangeType & displayDiffKindStates) > 0 || c.ChildrenOfDiffKindState(displayDiffKindStates).Count() > 0).ToList();
    }

    /// <summary>
    /// Make an empty copy of this and all its children, marking both as Missing.
    /// </summary>
    public DiffNode CloneEmpty()
    {
        this.ChangeType = this.Kind == NodeKind.Element?DiffKindState.ElementMissing : DiffKindState.AttributeMissing;

        DiffNode clone = new DiffNode(new string(' ', this.ToString().Length), this.RawTag, this.Kind, this.MatchRules, -1);
        clone.ChangeType = this.ChangeType;
        clone.Tag = this.Tag;

        foreach (DiffNode child in this.Children)
        {
            this.ChildrenChangeType = clone.ChildrenChangeType = this.ChildrenChangeType | (child.Kind == NodeKind.Element ? DiffKindState.ElementMissing : DiffKindState.AttributeMissing);
            clone.Children.Add(child.CloneEmpty());
        }

        return clone;
    }

    public string ToDisplayText(bool includeLineNumber)
    {
        string retVal = this.DisplayName;
        if (!string.IsNullOrWhiteSpace(this.Name))
        {
            // Built by concatenation, never string.Format: a name or value can contain braces
            // (for example {http://www.w3.org/1999/xlink}href), which Format would treat as placeholders.
            if (this.Kind == NodeKind.Attribute)
            {
                retVal += " (a)";
            }
            else
            {
                retVal += includeLineNumber && this.LineNumber >= 0 ? " (e " + this.LineNumber + ")" : " (e)";
            }
        }

        if (!string.IsNullOrEmpty(this.Value))
        {
            retVal += " = " + this.Value;
        }

        return retVal;
    }

    private static bool HasDirectText(XElement xelement)
    {
        return xelement.Nodes().OfType<XText>().Any(x => !string.IsNullOrWhiteSpace(x.Value));
    }
}