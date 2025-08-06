using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxXyziChunk(List<VoxelData> voxels): VoxChunk {

    public List<VoxelData> Voxels = voxels;
    
    public override VoxChunkType GetVoxChunkType() => VoxChunkType.Xyzi;

    public override string ToString() => $"XYZI({Voxels.Dbg()})";
}