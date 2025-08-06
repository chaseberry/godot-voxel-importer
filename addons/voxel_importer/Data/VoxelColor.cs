using VoxelImporter.addons.voxel_importer.Chunks;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Data;

public class VoxelColor(Color color) {

    public Color Color = color;
    public VoxMaterialChunk? Material;

}