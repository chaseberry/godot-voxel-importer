using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxGroupChild(
    int nodeId
) {

    public int NodeId = nodeId;

    public override string ToString() => $"child({NodeId})";
}