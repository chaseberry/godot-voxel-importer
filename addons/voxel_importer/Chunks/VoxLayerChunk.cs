using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxLayerChunk(
    int layerId,
    Dictionary<string, string> attrs,
    int reservedId
) : VoxChunk {

    public readonly int LayerId = layerId;
    public readonly Dictionary<string, string> Attributes = attrs;
    public readonly int ReservedId = reservedId;

    public override VoxChunkType GetVoxChunkType() => VoxChunkType.Layr;

    public override string ToString() => $"LAYR({LayerId}, {Attributes.Dbg()}, {ReservedId})";
    
    public string? GetName() => Attributes.GetValueOrDefault("_name");

    public bool IsHidden() => Attributes.GetValueOrDefault("_hidden") == "1";

}