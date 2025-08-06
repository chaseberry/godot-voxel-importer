using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Data;

public class VoxelGroupNode(
    int id,
    List<VoxelNode> children
) : VoxelNode {

    public int Id = id;
    public List<VoxelNode> Children = children;

    public override List<VoxelNode> GetChildren() => Children;

}