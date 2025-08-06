namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxMainChunk: VoxChunk {

    public override VoxChunkType GetVoxChunkType() => VoxChunkType.Main;

    public override string ToString() => "MAIN()";
}