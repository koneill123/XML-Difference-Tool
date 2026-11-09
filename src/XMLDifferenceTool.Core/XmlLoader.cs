#nullable enable
using System.Xml;
using System.Xml.Linq;

namespace XMLDifferenceTool.Core;

/// <summary>
/// How XML names that carry a namespace prefix (such as xlink:href) are read.
/// </summary>
public enum NamespaceHandling
{
    /// <summary>
    /// Normal XML namespace rules. A file that uses a prefix it never declares is rejected.
    /// </summary>
    Standard,

    /// <summary>
    /// Prefixes are treated as part of the name, so an undeclared prefix is not an error. The colon is replaced
    /// by <see cref="XmlLoader.PrefixSeparator"/> for comparison and shown as a colon again in the trees.
    /// Two names are the same only if they are spelled the same, whatever namespace the prefix stands for.
    /// </summary>
    LiteralPrefixes,

    /// <summary>
    /// Standard rules first. Only if a file fails with a problem that literal prefixes cure (typically an
    /// undeclared prefix) is it loaded with literal prefixes, and the result carries a notice saying so.
    /// </summary>
    Automatic,
}

/// <summary>
/// A loaded document. <paramref name="Notice"/> is set when the loader did something the user should be told
/// about, such as falling back to literal prefixes.
/// </summary>
public sealed record LoadedXml(XDocument Document, bool UsedLiteralPrefixes, string? Notice);

/// <summary>
/// Marks a document that was loaded with literal prefixes, so DiffNode knows to show the separator as a colon.
/// </summary>
internal sealed class LiteralPrefixesMarker
{
    public static readonly LiteralPrefixesMarker Instance = new();
}

/// <summary>
/// Loads XML for comparison. Shared by the WinForms and web front ends so both treat files the same way.
/// It never edits the text of a file: with literal prefixes only the names of elements and attributes are
/// read differently; values, text, comments and the file on disk are untouched.
/// </summary>
public static class XmlLoader
{
    /// <summary>
    /// Replaces the colon in prefixed names when literal prefixes are used. U+00B7 (middle dot) is legal inside
    /// an XML name but practically never used, so it cannot be confused with a real name the way a dot could.
    /// </summary>
    public const char PrefixSeparator = '\u00B7';

    private const string StandardFailureHint =
        "This file can be read if namespace prefixes are treated literally. " +
        "Set the Namespace handling option to Automatic or Literal prefixes.";

    /// <summary>
    /// Loads a document from its bytes (the bytes are used so a file can be parsed again without being re-read).
    /// </summary>
    public static LoadedXml Load(byte[] xml, NamespaceHandling handling)
    {
        ArgumentNullException.ThrowIfNull(xml);

        switch (handling)
        {
            case NamespaceHandling.LiteralPrefixes:
                return new LoadedXml(LoadLiteral(xml), true, null);

            case NamespaceHandling.Automatic:
                XmlException standardError;
                try
                {
                    return new LoadedXml(LoadStandard(xml), false, null);
                }
                catch (XmlException ex)
                {
                    standardError = ex;
                }

                // Literal parsing skips only the namespace checks, so if it succeeds where standard parsing
                // failed, the problem was a namespace one. Otherwise report the original error.
                if (TryLoadLiteral(xml, out XDocument? literal))
                {
                    return new LoadedXml(
                        literal,
                        true,
                        "Loaded with literal prefixes because standard parsing failed: " + standardError.Message);
                }

                throw standardError;

            default:
                try
                {
                    return new LoadedXml(LoadStandard(xml), false, null);
                }
                catch (XmlException ex) when (TryLoadLiteral(xml, out _))
                {
                    throw new InvalidDataException(
                        ex.Message + Environment.NewLine + Environment.NewLine + StandardFailureHint, ex);
                }
        }
    }

    /// <summary>
    /// In Automatic mode, if only one of two files needed literal prefixes, loads the other one the same way
    /// so that both use the same names. Otherwise returns both unchanged.
    /// </summary>
    public static (LoadedXml Left, LoadedXml Right) Reconcile(
        LoadedXml left, byte[] leftXml, LoadedXml right, byte[] rightXml, NamespaceHandling handling)
    {
        if (handling != NamespaceHandling.Automatic || left.UsedLiteralPrefixes == right.UsedLiteralPrefixes)
        {
            return (left, right);
        }

        return left.UsedLiteralPrefixes
            ? (left, MatchLiterally(right, rightXml))
            : (MatchLiterally(left, leftXml), right);
    }

    /// <summary>
    /// Joins the notices of the two files into one message, or returns null when there is nothing to report.
    /// </summary>
    public static string? DescribeNotices(string leftName, LoadedXml left, string rightName, LoadedXml right, string separator)
    {
        List<string> notices = new();
        if (left.Notice is not null)
        {
            notices.Add(leftName + ": " + left.Notice);
        }

        if (right.Notice is not null)
        {
            notices.Add(rightName + ": " + right.Notice);
        }

        return notices.Count == 0 ? null : string.Join(separator, notices);
    }

    private static LoadedXml MatchLiterally(LoadedXml loaded, byte[] xml)
    {
        if (TryLoadLiteral(xml, out XDocument? literal))
        {
            return new LoadedXml(literal, true, "Also loaded with literal prefixes so both files use the same names.");
        }

        return loaded with
        {
            Notice = "Could not be loaded with literal prefixes like the other file, so namespace-prefixed names may not match.",
        };
    }

    private static XDocument LoadStandard(byte[] xml)
    {
        // Same call the front ends used before this loader existed.
        using MemoryStream stream = new(xml, writable: false);
        return XDocument.Load(stream, LoadOptions.SetLineInfo);
    }

    private static bool TryLoadLiteral(byte[] xml, out XDocument? document)
    {
        try
        {
            document = LoadLiteral(xml);
            return true;
        }
        catch (Exception)
        {
            document = null;
            return false;
        }
    }

    private static XDocument LoadLiteral(byte[] xml)
    {
        using MemoryStream stream = new(xml, writable: false);
        using XmlTextReader textReader = new(stream)
        {
            Namespaces = false,                                   // prefixes are just part of the name
            DtdProcessing = DtdProcessing.Ignore,                 // never fetch or expand a DTD
            XmlResolver = null,
            WhitespaceHandling = WhitespaceHandling.Significant,  // same as XDocument.Load: drop insignificant whitespace
        };

        using LiteralPrefixReader reader = new(textReader);
        XDocument document = XDocument.Load(reader, LoadOptions.SetLineInfo);
        document.AddAnnotation(LiteralPrefixesMarker.Instance);
        return document;
    }

    /// <summary>
    /// Wraps a reader that has namespace support turned off and swaps the colon in every name for
    /// <see cref="PrefixSeparator"/>, because XName cannot hold a colon. Everything else, including line
    /// information, comes straight from the wrapped reader.
    /// </summary>
    private sealed class LiteralPrefixReader : XmlReader, IXmlLineInfo
    {
        private readonly XmlTextReader inner;

        public LiteralPrefixReader(XmlTextReader inner) => this.inner = inner;

        private static string MapName(string name)
        {
            if (name.IndexOf(':') < 0)
            {
                return name;
            }

            string mapped = name.Replace(':', PrefixSeparator);

            // An XML name cannot start with the separator, so protect a name that began with a colon
            return mapped[0] == PrefixSeparator ? "_" + mapped : mapped;
        }

        // The only members that change anything
        public override string LocalName => MapName(this.inner.LocalName);

        public override string Name => MapName(this.inner.Name);

        public override string Prefix => string.Empty;

        public override string NamespaceURI => string.Empty;

        // Everything below just forwards to the wrapped reader
        public override XmlNodeType NodeType => this.inner.NodeType;

        public override bool HasValue => this.inner.HasValue;

        public override string Value => this.inner.Value;

        public override int Depth => this.inner.Depth;

        public override string BaseURI => this.inner.BaseURI;

        public override bool IsEmptyElement => this.inner.IsEmptyElement;

        public override int AttributeCount => this.inner.AttributeCount;

        public override bool EOF => this.inner.EOF;

        public override ReadState ReadState => this.inner.ReadState;

        public override XmlNameTable NameTable => this.inner.NameTable;

        public override string? GetAttribute(string name) => this.inner.GetAttribute(name);

        public override string? GetAttribute(string name, string? namespaceURI) => this.inner.GetAttribute(name, namespaceURI);

        public override string GetAttribute(int i) => this.inner.GetAttribute(i);

        public override bool MoveToAttribute(string name) => this.inner.MoveToAttribute(name);

        public override bool MoveToAttribute(string name, string? ns) => this.inner.MoveToAttribute(name, ns);

        public override bool MoveToFirstAttribute() => this.inner.MoveToFirstAttribute();

        public override bool MoveToNextAttribute() => this.inner.MoveToNextAttribute();

        public override bool MoveToElement() => this.inner.MoveToElement();

        public override bool ReadAttributeValue() => this.inner.ReadAttributeValue();

        public override bool Read() => this.inner.Read();

        public override string? LookupNamespace(string prefix) => this.inner.LookupNamespace(prefix);

        public override void ResolveEntity() => this.inner.ResolveEntity();

        public override void Close() => this.inner.Close();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.inner.Dispose();
            }

            base.Dispose(disposing);
        }

        // IXmlLineInfo: lets XDocument.Load(LoadOptions.SetLineInfo) record real line numbers
        public bool HasLineInfo() => ((IXmlLineInfo)this.inner).HasLineInfo();

        public int LineNumber => this.inner.LineNumber;

        public int LinePosition => this.inner.LinePosition;
    }
}
