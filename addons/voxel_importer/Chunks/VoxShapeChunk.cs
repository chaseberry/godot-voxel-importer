using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxShapeChunk(
    int id,
    Dictionary<string, string> attrs,
    List<VoxShapeModel> models
) : VoxChunk {

    public int Id = id;
    public Dictionary<string, string> Attributes = attrs;
    public List<VoxShapeModel> Models = models;

    public override VoxChunkType GetVoxChunkType() => VoxChunkType.Nshp;

    public override string ToString() => $"nSHP({Id}, {Attributes.Dbg()}, {Models.Dbg()})";
}