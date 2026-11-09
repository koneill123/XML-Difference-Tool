using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.XPath;

namespace XMLDifferenceTool.Core
{
    public static class Helpers
    {
        public const int MaxDepth = 500; // Prevents stack overflow on very deep XML files. Can be changed for testing or special cases.
        public const char XmlSeparator = '\u001F'; // Used to separate the tag from the index in the unique tag for repeated elements. This character is not allowed in XML names, so it will never be part of a tag.
        public const char PathSeparator = '\u0001'; // Used to separate the keys of each node in the path. This character is not allowed in XML names, so it will never be part of a tag.
    }

    public static class ProjectInformation
    {
        public const string AppName = "XMLDifferenceTool";
        public const string ProjectUrl = "https://github.com/koneill123/XML-Difference-Tool"; //TODO
        public const string CopyrightText = "Copyright © 2026 Keith O'Neill\nReleased under the MIT License";
        public const string DescriptionText =
            "Compares two XML files side by side and highlights elements and attributes " +
            "that are the same, different, or missing.";
    }

    [Flags]
    public enum DiffKindState
    {
        None = 0,
        ElementSame = 1,
        ElementDifferent = 2,
        ElementMissing = 4,
        AttributeSame = 8,
        AttributeDifferent = 16,
        AttributeMissing = 32,
        Same = ElementSame, // The following are convenience flags for the combined state of both elements and attributes for clarity with the ForKind method.
        Different = ElementDifferent,
        Missing = ElementMissing,
    }

    [Flags]
    public enum NodeKind
    {
        Element = 0,
        Attribute = 1,
    }

    public enum MatchRepeateElementsBy
    {
        OrderInFile,
        NameAndValue,
    }

    public static class DiffKindStateExtensions
    {
        private const int StateBits = 3;   // Same, Different, Missing

        /// <summary>
        /// This is a bit of a hack to allow us to store the state of both an element and an attribute in a single DiffKindState value.
        /// It can be called with DiffKindStates such as ElementDifferent or AttributeMissing and will return a DiffKindState that has the same state for both the element and attribute.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="kind"></param>
        /// <returns></returns>
        public static DiffKindState ForKind(this DiffKindState state, NodeKind kind)
        {
            return (DiffKindState)((uint)state << (StateBits * (int)kind));
        }
    }

    public sealed class AppSettings
    {
        public int Version { get; set; } = 1;
        public MatchRepeateElementsBy MatchMode { get; set; } = MatchRepeateElementsBy.OrderInFile;
        public NamespaceHandling NamespaceMode { get; set; } = NamespaceHandling.Automatic;   // new
        public bool ShowSame { get; set; } = true;
        public bool ShowLineNumbers { get; set; } = true;
        public bool ShowDifferent { get; set; } = true;
        public bool ShowMissing { get; set; } = true;
        public bool ShowAttributes { get; set; } = true;     // new
        public bool ShowElements { get; set; } = true;       // new
        public double SplitRatio { get; set; } = 0.5;
        public WindowSettings? Window { get; set; }          // new; Blazor ignores it
    }

    public sealed class WindowSettings
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool Maximized { get; set; }
    }
    public interface ISettingsStore
    {
        Task<AppSettings> LoadAsync();
        Task SaveAsync(AppSettings settings);
    }

    public static class SettingsJson
    {
        static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }     // enums saved by name, not number
        };

        public static string Serialize(AppSettings s) => JsonSerializer.Serialize(s, Options);

        public static AppSettings Deserialize(string? json)
        {
            try { return JsonSerializer.Deserialize<AppSettings>(json ?? "", Options) ?? new(); }
            catch { return new(); }                            // corrupt or empty: use defaults
        }
    }
}
