using System;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Constants;

public enum FaceOrientation {

    Top,
    Bottom,
    Left,
    Right,
    Front,
    Back

}

public static class FaceOrientationFunctions {

    // In many places the expected values for top/bottom & front/back are flipped.
    // This is because the vox format treats Y as forward/backward and Z as up/down
    // These modified values in the getter functions simulate rotating the object
    // 90 degrees around the X axis to align the model.

    public static Vector3.Axis PrimaryAxis(this FaceOrientation f) => f switch {
        FaceOrientation.Top => Vector3.Axis.Z,
        FaceOrientation.Bottom => Vector3.Axis.Z,
        FaceOrientation.Left => Vector3.Axis.X,
        FaceOrientation.Right => Vector3.Axis.X,
        FaceOrientation.Front => Vector3.Axis.Y,
        FaceOrientation.Back => Vector3.Axis.Y,
        _ => throw new ArgumentOutOfRangeException(nameof(f), f, null)
    };

    // The "width" of a rectangle when looking at the FaceOrientation head on
    public static Vector3.Axis WidthAxis(this FaceOrientation f) => f switch {
        FaceOrientation.Top => Vector3.Axis.Y,
        FaceOrientation.Bottom => Vector3.Axis.Y,
        FaceOrientation.Left => Vector3.Axis.Z,
        FaceOrientation.Right => Vector3.Axis.Z,
        FaceOrientation.Front => Vector3.Axis.X,
        FaceOrientation.Back => Vector3.Axis.X,
        _ => throw new ArgumentOutOfRangeException(nameof(f), f, null)
    };

    // The "height" of a rectangle when looking at the FaceOrientation head on
    public static Vector3.Axis HeightAxis(this FaceOrientation f) => f switch {
        FaceOrientation.Top => Vector3.Axis.X,
        FaceOrientation.Bottom => Vector3.Axis.X,
        FaceOrientation.Left => Vector3.Axis.Y,
        FaceOrientation.Right => Vector3.Axis.Y,
        FaceOrientation.Front => Vector3.Axis.Z,
        FaceOrientation.Back => Vector3.Axis.Z,
        _ => throw new ArgumentOutOfRangeException(nameof(f), f, null)
    };

    public static Vector3 NeighborFor(this FaceOrientation f) => f switch {
        FaceOrientation.Top => Vector3.Back,
        FaceOrientation.Bottom => Vector3.Forward,
        FaceOrientation.Left => Vector3.Left,
        FaceOrientation.Right => Vector3.Right,
        FaceOrientation.Front => Vector3.Down,
        FaceOrientation.Back => Vector3.Up,
        _ => throw new ArgumentOutOfRangeException(nameof(f), f, null)
    };

    public static Vector3 GetNormal(this FaceOrientation f) => f switch {
        FaceOrientation.Top => Vector3.Up,
        FaceOrientation.Bottom => Vector3.Down,
        FaceOrientation.Left => Vector3.Left,
        FaceOrientation.Right => Vector3.Right,
        FaceOrientation.Front => Vector3.Back,
        FaceOrientation.Back => Vector3.Forward,
        _ => throw new ArgumentOutOfRangeException(nameof(f), f, null)
    };

    public static Vector3[] GetFaces(this FaceOrientation f) => f switch {
        FaceOrientation.Top => Faces.Front,
        FaceOrientation.Bottom => Faces.Back,
        FaceOrientation.Left => Faces.Left,
        FaceOrientation.Right => Faces.Right,
        FaceOrientation.Front => Faces.Bottom,
        FaceOrientation.Back => Faces.Top,
        _ => throw new ArgumentOutOfRangeException(nameof(f), f, null)
    };

}