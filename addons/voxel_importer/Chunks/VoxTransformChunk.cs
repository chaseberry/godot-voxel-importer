using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxTransformChunk(
    int id,
    Dictionary<string, string> attrs,
    int childId,
    int reservedId,
    int layerId,
    List<VoxFrame> frames
) : VoxChunk {

    public int Id = id;
    public Dictionary<string, string> Attributes = attrs;
    public int ChildId = childId;
    public int ReservedId = reservedId;
    public int LayerId = layerId;
    public List<VoxFrame> Frames = frames;

    public string? GetName() => Attributes.GetValueOrDefault("_name");

    public bool IsHidden() => Attributes.GetValueOrDefault("_hidden") == "1";

    public override VoxChunkType GetVoxChunkType() => VoxChunkType.Ntrn;

    public override string ToString() =>
        $"nTRN({Id}, {Attributes.Dbg()}, {ChildId}, {ReservedId}, {LayerId}, {Frames.Dbg()})";

}