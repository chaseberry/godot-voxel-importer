using System.Collections.Generic;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxRgbaChunk(List<Color> colors): VoxChunk {

    public List<Color> Colors = colors;
    
    public override VoxChunkType GetVoxChunkType() => VoxChunkType.Rgba;

    public override string ToString() => $"RGBA({Colors.Dbg()})";
}