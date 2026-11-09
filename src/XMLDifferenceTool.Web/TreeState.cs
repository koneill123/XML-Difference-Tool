using XMLDifferenceTool.Core;

namespace XMLDifferenceTool.Web;

/// <summary>
/// Expand/collapse and selection state shared by every DiffRow. A row is identified by its
/// path (the chain of node keys from the root), the Blazor equivalent of FullUniqueTag in
/// the WinForms form. Left and right sides share one row, so they can never drift apart.
/// </summary>
public sealed class TreeState
{
    private readonly Dictionary<string, bool> overrides = new();
    private bool defaultOpen;

    public string? SelectedPath { get; private set; }

    /// <summary>Raised after any change. Rows listen and re-render only if their own state changed.</summary>
    public event Action? Changed;

    public static string KeyOf(DiffNode node) => $"{(int)node.Kind}:{node.Tag}";

    public static string Combine(string parentPath, DiffNode node)
        => parentPath.Length == 0 ? KeyOf(node) : parentPath + Helpers.PathSeparator + KeyOf(node);

    public bool IsOpen(string path)
        => overrides.TryGetValue(path, out bool open) ? open : defaultOpen;

    public void Toggle(string path)
    {
        overrides[path] = !IsOpen(path);
        Changed?.Invoke();
    }

    public void Select(string path)
    {
        if (SelectedPath == path) return;
        SelectedPath = path;
        Changed?.Invoke();
    }

    public void ExpandAll()
    {
        defaultOpen = true;
        overrides.Clear();
        Changed?.Invoke();
    }

    /// <summary>Collapses everything except the root row, so the tree never disappears entirely.</summary>
    public void CollapseAll(string rootPath)
    {
        defaultOpen = false;
        overrides.Clear();
        overrides[rootPath] = true;
        Changed?.Invoke();
    }

    /// <summary>Used when new files are compared: only the root starts expanded, nothing selected.</summary>
    public void Reset(string rootPath)
    {
        defaultOpen = false;
        overrides.Clear();
        overrides[rootPath] = true;
        SelectedPath = null;
        Changed?.Invoke();
    }
}
