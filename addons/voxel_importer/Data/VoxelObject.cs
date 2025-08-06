using System;
using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Data;

public class VoxelObject(
    int id,
    Dictionary<int, VoxelModel> frames
) : VoxelNode {

    public int Id = id;
    public Dictionary<int, VoxelModel> Frames { get; private set; } = frames;

    public VoxelModel GetModelAtFrame(int frame) {
        for (int z = frame; z >= 0; z--) {
            if (Frames.TryGetValue(z, out var m)) {
                return m;
            }
        }

        throw new ArgumentException("Bad voxel data for model {id}");
    }

    public override List<VoxelNode> GetChildren() => VoxUtils.EmptyList<VoxelNode>();

}