using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Chunks;
using VoxelImporter.addons.voxel_importer.Functions;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Data;

public class VoxFile(int version) {

    public int Version = version;
    public List<VoxelModel> Models = [];
    public List<VoxelColor> Colors = [];
    public VoxelNode? Node = null;

    public List<SearchedVoxelObject> GatherObjects(bool includeHidden) {
        if (Node == null) {
            return Models.Select(m => new SearchedVoxelObject {
                VoxelObject = new(0, new() { [0] = m }),
                Chain = []
            }).ToList();
        }

        return includeHidden ? AllObjects() : VisibleObjects();
    }

    private List<SearchedVoxelObject> AllObjects() => SearchObjects(Node!, true, ImmutableList<VoxelNode>.Empty);
    private List<SearchedVoxelObject> VisibleObjects() => SearchObjects(Node!, false, ImmutableList<VoxelNode>.Empty);

    private List<SearchedVoxelObject> SearchObjects(
        VoxelNode node,
        bool includeHidden,
        ImmutableList<VoxelNode> chain
    ) {
        return node switch {
            VoxelTransformNode t => includeHidden || !t.IsHidden()
                ? SearchObjects(t.Child, includeHidden, chain.Append(t).ToImmutableList())
                : [],
            VoxelGroupNode group => group.Children.SelectMany(c =>
                SearchObjects(c, includeHidden, chain.Append(group).ToImmutableList())
            ).ToList(),
            VoxelObject vo => VoxUtils.ListOf(
                new SearchedVoxelObject {
                    VoxelObject = vo,
                    Chain = chain.ToList()
                }),
            _ => []
        };
    }

    public List<int> GetFrameIndexes() => Node != null ? SearchNodes(Node, false).ToList() : VoxUtils.ListOf(0);

    private ISet<int> SearchNodes(VoxelNode node, bool includeHidden) {
        return node switch {
            VoxelTransformNode t => includeHidden || !t.IsHidden()
                ? t.Frames.Select(f => f.GetFrameIndex()).Union(SearchNodes(t.Child, includeHidden)).ToHashSet()
                : ImmutableHashSet<int>.Empty,
            VoxelGroupNode group => group.Children.SelectMany(c => SearchNodes(c, includeHidden)).ToHashSet(),
            VoxelObject vo => vo.Frames.Keys.ToHashSet(),
            _ => ImmutableHashSet<int>.Empty
        };
    }

}