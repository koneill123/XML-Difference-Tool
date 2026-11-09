using System.ComponentModel.DataAnnotations;
using XMLDifferenceTool.Core;
namespace XMLDifferenceTool.Core;

/// <summary>
/// Compares two DiffNode trees and marks each node as Same, Different or Missing.
/// Placeholder (empty) nodes are added to the opposite side for Missing items so both
/// trees stay aligned. Call once per pair of freshly built DiffNode trees.
/// </summary>
public static class XmlDiffer
{
    public static void DiffNodeDataCombined(DiffNode leftDiffNode, DiffNode rightDiffNode)
    {
        DiffKindState dt = DiffNodeDataCombined(leftDiffNode.Children, rightDiffNode.Children);
        leftDiffNode.ChildrenChangeType |= dt;
        rightDiffNode.ChildrenChangeType |= dt;
    }

    private static DiffKindState DiffNodeDataCombined(List<DiffNode> leftDiffNodeList, List<DiffNode> rightDiffNodeList)
    {
        DiffKindState childrenDiffKindState = DiffKindState.None;
        childrenDiffKindState |= DiffDiffNodeList(leftDiffNodeList, rightDiffNodeList); // Compare left to right
        childrenDiffKindState |= DiffDiffNodeList(rightDiffNodeList, leftDiffNodeList); // Now compare right to left to find any missing nodes in the left tree
        return childrenDiffKindState;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="DiffNodeList1"></param>
    /// <param name="DiffNodeList2"></param>
    /// <returns></returns>
    /// Note: This method is called twice, once for each side of the comparison, to ensure that all nodes are compared and any missing nodes are identified.
    /// so the terms "right" and "left" lose meaning here
    private static DiffKindState DiffDiffNodeList(List<DiffNode> DiffNodeList1, List<DiffNode> DiffNodeList2)
    {
        DiffKindState totalChildrenDiffKindState = DiffKindState.None;

        // Index list 2 once by (kind, tag) so each lookup is a hash lookup instead of a scan of the whole list.
        // The comparison is ordinal and case-sensitive, the same rule DiffNode.GetUniqueTag uses to decide that
        // two nodes are the same item. TryAdd keeps the first node for a key, as FirstOrDefault did.
        // Placeholders added to list 2 inside the loop are not indexed; tags are unique within list 1, so
        // nothing later in list 1 can match them.
        Dictionary<(NodeKind Kind, string Tag), DiffNode> lookup = new(DiffNodeList2.Count);
        foreach (DiffNode candidate in DiffNodeList2)
        {
            lookup.TryAdd((candidate.Kind, candidate.Tag), candidate);
        }

        foreach (DiffNode DiffNodeItem1 in DiffNodeList1.Where(vdl1 => vdl1.ChangeType != DiffKindState.ElementMissing && vdl1.ChangeType != DiffKindState.AttributeMissing))
        {
            DiffKindState DiffKindStateChildrenDiffKindState = DiffKindState.Same.ForKind(DiffNodeItem1.Kind);
            lookup.TryGetValue((DiffNodeItem1.Kind, DiffNodeItem1.Tag), out DiffNode DiffNodeItem2);
            if (DiffNodeItem2 != null)
            {
                if (DiffNodeItem1.Value == DiffNodeItem2.Value)
                {
                    DiffNodeItem1.ChangeType = DiffNodeItem2.ChangeType = DiffKindState.Same.ForKind(DiffNodeItem1.Kind);
                }
                else
                {
                    DiffNodeItem1.ChangeType = DiffNodeItem2.ChangeType = DiffKindState.Different.ForKind(DiffNodeItem1.Kind);

                    totalChildrenDiffKindState |= DiffKindState.Different.ForKind(DiffNodeItem1.Kind);
                }

                DiffKindStateChildrenDiffKindState |= DiffDiffNodeList(DiffNodeItem1.Children, DiffNodeItem2.Children);
                DiffNodeItem1.ChildrenChangeType |= DiffKindStateChildrenDiffKindState;
                DiffNodeItem2.ChildrenChangeType |= DiffKindStateChildrenDiffKindState;
            }
            else
            {
                DiffNode newEmptyDiffNode = DiffNodeItem1.CloneEmpty();
                DiffNodeList2.Add(newEmptyDiffNode);

                DiffKindState diffKindState = DiffKindState.Missing.ForKind(DiffNodeItem1.Kind);
                DiffKindStateChildrenDiffKindState |= diffKindState;
                totalChildrenDiffKindState |= diffKindState;
                DiffNodeItem1.ChildrenChangeType |= DiffKindStateChildrenDiffKindState;
            }

            totalChildrenDiffKindState |= DiffKindStateChildrenDiffKindState;
        }

        return totalChildrenDiffKindState;
    }
}