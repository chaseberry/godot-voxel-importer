using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoxelImporter.addons.voxel_importer.Functions;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxFrame(
    Dictionary<string, string> attrs
) {

    public readonly Dictionary<string, string> Attributes = attrs;

    public override string ToString() => $"Frame({Attributes.Dbg()})";

    public Vector3 GetTranslation() {
        var v = Attributes.GetValueOrDefault("_t");
        if (v == null) {
            return Vector3.Zero;
        }

        var components = v.Split(" ");
        if (components.Length != 3) {
            GD.PushError($"Invalid Transform for frame. {v}");
            return Vector3.Zero;
        }

        if (!float.TryParse(components[0], out var x)) {
            x = 0f;
        }

        if (!float.TryParse(components[1], out var y)) {
            y = 0f;
        }

        if (!float.TryParse(components[2], out var z)) {
            z = 0f;
        }

        return new(x, y, z);
    }

    public Basis GetRotation() {
        var raw = Attributes.GetValueOrDefault("_r");
        if (raw == null) {
            return Basis.Identity;
        }

        if (!int.TryParse(raw, out var data)) {
            return Basis.Identity;
        }

        // The amount of shenanigans needed to make rotations work
        // Firstly, magica voxel has the Y and Z axis flipped, so rotations and coordinates are just... wrong.

        var xIndex = (data >> 0) & 0b11;
        var yIndex = (data >> 2) & 0b11;
        var zIndex = ParseIndex(VoxUtils.ListOf(0, 1, 2).First(i => i != xIndex && i != yIndex));
        xIndex = ParseIndex(xIndex);
        yIndex = ParseIndex(yIndex);
        // So we flip the y & z axis when parsing the sub data out.
   
        var xSign = ((data >> 4) & 0b1) == 0 ? 1 : -1;
        var ySign = ((data >> 5) & 0b1) == 0 ? 1 : -1;
        var zSign = ((data >> 6) & 0b1) == 0 ? 1 : -1;

        var basis = new Basis();
        basis.X = new() { [xIndex] = xSign };
        basis.Z = new() { [yIndex] = ySign }; // and flip the axis here
        basis.Y = new() { [zIndex] = zSign }; // and here
        // basis = basis.Inverse(); // Solves rotation around the Y axis, but breaks rotation around the X and Z axis

        return basis;
    }

    public int GetFrameIndex() {
        if (!int.TryParse(Attributes.GetValueOrDefault("_f"), out var f)) {
            f = 0;
        }

        return f;
    }

    public static string BuildRow(int i, int v) {
        var s = new StringBuilder();
        for (int z = 0; z < 3; z++) {
            if (z != i) {
                s.Append('0');
                continue;
            }

            s.Append(v);
        }

        return s.ToString();
    }

    private static int ParseIndex(int i) {
        // return i;
        return i switch {
            1 => 2, 2 => 1, _ => i
        };
    }

}