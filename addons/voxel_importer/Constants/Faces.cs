using Godot;

namespace VoxelImporter.addons.voxel_importer.Constants;

public static class Faces {

    // Each "set of 3" are the base vertexes needed to make two triangles
    // that becomes the face of a voxel square
    
    public static Vector3[] Top = [
        new(1, 1, 1),
        new(0, 1, 1),
        new(0, 1, 0),

        new(0, 1, 0),
        new(1, 1, 0),
        new(1, 1, 1),
    ];

    public static Vector3[] Bottom = [
        new(0, 0, 0),
        new(0, 0, 1),
        new(1, 0, 1),

        new(1, 0, 1),
        new(1, 0, 0),
        new(0, 0, 0),
    ];

    public static Vector3[] Front = [
        new(0, 1, 1),
        new(1, 1, 1),
        new(1, 0, 1),

        new(1, 0, 1),
        new(0, 0, 1),
        new(0, 1, 1),
    ];

    public static Vector3[] Back = [
        new(1, 0, 0),
        new(1, 1, 0),
        new(0, 1, 0),

        new(0, 1, 0),
        new(0, 0, 0),
        new(1, 0, 0)
    ];

    public static Vector3[] Left = [
        new(0, 1, 1),
        new(0, 0, 1),
        new(0, 0, 0),

        new(0, 0, 0),
        new(0, 1, 0),
        new(0, 1, 1),
    ];

    public static Vector3[] Right = [
        new(1, 1, 1),
        new(1, 1, 0),
        new(1, 0, 0),

        new(1, 0, 0),
        new(1, 0, 1),
        new(1, 1, 1),
    ];

}