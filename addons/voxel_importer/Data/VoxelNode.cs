using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Data;

public abstract class VoxelNode {

    public abstract List<VoxelNode> GetChildren();

}