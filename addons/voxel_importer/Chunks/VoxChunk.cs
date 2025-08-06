namespace VoxelImporter.addons.voxel_importer.Chunks;

public abstract class VoxChunk {

    public abstract VoxChunkType GetVoxChunkType();

    public new abstract string ToString();
    // public override string ToString() => $"";

}