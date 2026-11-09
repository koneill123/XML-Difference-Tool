using System.Xml.Linq;

namespace XMLDifferenceTool.Core;

/// <summary>
/// A progress update from a long-running comparison.
/// <paramref name="Fraction"/> is 0.0 to 1.0 when the amount of work is known, or null when it is not
/// (the UI should then show an indeterminate/marquee indicator).
/// </summary>
public sealed record DiffProgress(string Message, double? Fraction = null);

/// <summary>
/// The two diffed trees produced by <see cref="XmlDiffService.CompareAsync"/>.
/// </summary>
public sealed record DiffResult(DiffNode Left, DiffNode Right);

/// <summary>
/// Async, progress-reporting entry point for comparing two XML documents.
/// Has no UI dependencies so it can be used from both WinForms and Blazor.
/// </summary>
public static class XmlDiffService
{
    /// <summary>
    /// Builds a <see cref="DiffNode"/> tree for each document and diffs them.
    /// Progress is reported between stages. Each stage awaits a short delay so that a single-threaded host
    /// (such as Blazor WebAssembly) gets a chance to repaint; on a thread-pool thread the delay is harmless.
    /// <c>yieldDelayMs</c> sets how long each of those pauses lasts. 1 ms is enough on a thread-pool thread, but a
    /// browser may not repaint that quickly, so a web host can pass a larger value (about 30) to make sure each
    /// progress message is actually drawn.
    /// </summary>
    public static async Task<DiffResult> CompareAsync(
        XDocument leftDocument,
        XDocument rightDocument,
        MatchRepeateElementsBy matchRules,
        IProgress<DiffProgress>? progress = null,
        int yieldDelayMs = 1)
    {
        ArgumentNullException.ThrowIfNull(leftDocument);
        ArgumentNullException.ThrowIfNull(rightDocument);

        progress?.Report(new DiffProgress("Reading the structure of the left file..."));
        await Task.Delay(yieldDelayMs);
        DiffNode left = new DiffNode(leftDocument, matchRules);

        progress?.Report(new DiffProgress("Reading the structure of the right file..."));
        await Task.Delay(yieldDelayMs);
        DiffNode right = new DiffNode(rightDocument, matchRules);

        progress?.Report(new DiffProgress("Comparing the two files..."));
        await Task.Delay(yieldDelayMs);
        XmlDiffer.DiffNodeDataCombined(left, right);

        return new DiffResult(left, right);
    }
}
