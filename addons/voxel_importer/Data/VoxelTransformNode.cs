using System.Collections.Generic;
using VoxelImporter.addons.voxel_importer.Chunks;

namespace VoxelImporter.addons.voxel_importer.Data;

public class VoxelTransformNode(
    int id,
    string? name,
    bool hidden,
    VoxelNode child,
    VoxelLayer layer,
    List<VoxFrame> frames
) : VoxelNode {

    public int Id = id;
    public string? Name = name;
    public bool Hidden = hidden;
    public VoxelNode Child = child;
    public VoxelLayer Layer = layer;
    public List<VoxFrame> Frames = frames;

    public override List<VoxelNode> GetChildren() => VoxUtils.ListOf(Child);

    public bool IsHidden() => Hidden || Layer.Hidden;

    public VoxFrame? GetFrameAtIndex(int index) => Frames.Find(f => f.GetFrameIndex() == index);

}