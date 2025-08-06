namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxSizeChunk(int x, int y, int z) : VoxChunk {

    public int X = x, Y = y, Z = z;

    public override VoxChunkType GetVoxChunkType() => VoxChunkType.Size;

    public override string ToString() => $"SIZE({X}, {Y}, {Z})";
}