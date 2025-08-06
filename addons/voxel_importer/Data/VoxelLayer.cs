using VoxelImporter.addons.voxel_importer.Chunks;

namespace VoxelImporter.addons.voxel_importer.Data;

public class VoxelLayer(
    int layerId,
    string? name,
    bool hidden
) {

    public VoxelLayer(VoxLayerChunk vlc) : this(vlc.LayerId, vlc.GetName(), vlc.IsHidden()) { }

    public int LayerId = layerId;
    public string? Name = name;
    public bool Hidden = hidden;

}