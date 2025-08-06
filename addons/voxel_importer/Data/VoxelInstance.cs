using System;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Data;

public class VoxelInstance(int x, int y, int z, VoxelColor color) {

    public VoxelInstance(Vector3 v, VoxelColor c) : this((int)v.X, (int)v.Y, (int)v.Z, c) { }

    public int X = x;
    public int Y = y;
    public int Z = z;
    public readonly VoxelColor Color = color;

    public Vector3 ToV3() => new(X, Y, Z);
    public int GetCoordinateForAxis(Vector3.Axis axis) => axis switch {
        Vector3.Axis.X => X,
        Vector3.Axis.Y => Y,
        Vector3.Axis.Z => Z,
        _ => throw new ArgumentOutOfRangeException(nameof(axis), axis, null)
    };

}