using System.Drawing;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxelData(int x, int y, int z, int colorIndex) {

    public int X = x;
    public int Y = y;
    public int Z = z;
    public int ColorIndex = colorIndex;

    public override string ToString() => $"Voxel({X}, {Y}, {Z}, {ColorIndex})";
}