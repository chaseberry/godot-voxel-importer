using System.Collections.Generic;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Chunks;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Data;

public class VoxelModel(
    int x,
    int y,
    int z,
    List<VoxelInstance> voxels
) {

    public VoxelModel(
        VoxSizeChunk size,
        VoxXyziChunk xyzi,
        List<VoxelColor> colors
    ) : this(size.X, size.Y, size.Z,
        xyzi.Voxels.Select(v => new VoxelInstance(v.X, v.Y, v.Z, colors[v.ColorIndex])).ToList()) { }

    public int X = x;
    public int Y = y;
    public int Z = z;

    // Only used for building mesh libs
    public string? Name = null;

    public List<VoxelInstance> Voxels = voxels;

    public Dictionary<Vector3, VoxelColor> ToDictionary() => Voxels.ToDictionary(
        m => new Vector3(m.X, m.Y, m.Z),
        m => m.Color
    );

}