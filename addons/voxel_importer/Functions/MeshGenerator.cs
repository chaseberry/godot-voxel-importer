using System;
using System.Collections.Generic;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Constants;
using VoxelImporter.addons.voxel_importer.Data;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Functions;

public static class MeshGenerator {

    private static Basis VoxToGodot = new(Vector3.Right, Vector3.Forward, Vector3.Up);

    public readonly static StandardMaterial3D DefaultMaterial = new()
    {
        Roughness = 1,
        VertexColorIsSrgb = true,
        VertexColorUseAsAlbedo = true
    };

    public static Mesh Greedy(VoxelModel model, float scale, bool groundOrigin, bool applyMaterials) {
        // Set bounds of the voxel
        var minValues = new Vector3(
            model.Voxels.MinBy(v => v.X)!.X,
            model.Voxels.MinBy(v => v.Y)!.Y,
            model.Voxels.MinBy(v => v.Z)!.Z
        );
        var maxValues = new Vector3(
            model.Voxels.MaxBy(v => v.X)!.X,
            model.Voxels.MaxBy(v => v.Y)!.Y,
            model.Voxels.MaxBy(v => v.Z)!.Z
        );

        if (!applyMaterials) {
            return BuildMesh(model, DefaultMaterial, null, minValues, maxValues, scale, groundOrigin);
        }

        var data = new Dictionary<Material, List<VoxelInstance>>();

        foreach (var voxel in model.Voxels) {
            var mat = voxel.Color.Material?.GodotMaterial(voxel.Color.Color) ?? DefaultMaterial;

            if (!data.ContainsKey(mat)) {
                data[mat] = new();
            }

            data[mat].Add(voxel);
        }

        if (data.Count == 0) {
            return new SurfaceTool().Commit();
        }

        ArrayMesh? output = null;

        foreach (var (mat, voxels) in data) {
            var m = new VoxelModel(model.X, model.Y, model.Z, voxels);

            output = BuildMesh(m, mat, output, minValues, maxValues, scale, groundOrigin);
        }

        return output!;
    }

    private static ArrayMesh BuildMesh(
        VoxelModel model,
        Material mat,
        ArrayMesh? existing,
        Vector3 min,
        Vector3 max,
        float scale,
        bool groundOrigin
    ) {
        var st = new SurfaceTool();

        st.Begin(Mesh.PrimitiveType.Triangles);
        if (model.Voxels.IsEmpty()) {
            return st.Commit();
        }

        var modelDict = model.ToDictionary();

        foreach (var faceOrientation in Enum.GetValues<FaceOrientation>()) {
            GenerateMeshForOrientation(
                faceOrientation,
                modelDict,
                st,
                material: mat,
                min: min,
                max: max,
                scale,
                groundOrigin
            );
        }

        st.SetMaterial(mat);

        return st.Commit(existing);
    }

    private static void GenerateMeshForOrientation(
        FaceOrientation orientation, 
        Dictionary<Vector3, VoxelColor> model,
        SurfaceTool st,
        Material material,
        Vector3 min,
        Vector3 max,
        float scale,
        bool groundOrigin
    ) {
        var primaryAxis = (int)orientation.PrimaryAxis();
        for (float axisCoord = min[primaryAxis]; axisCoord <= max[primaryAxis]; axisCoord++) {
            var slice = BuildSliceForFace(orientation, axisCoord, model);
            if (slice.Count > 0) {
                BuildMeshesForOrientation(orientation, slice, axisCoord, st, material, min, max, scale, groundOrigin);
            }
        }
    }

    private static Dictionary<Vector3, VoxelColor> BuildSliceForFace(
        FaceOrientation orientation,
        float axisCoordinate, Dictionary<Vector3, VoxelColor> model
    ) {
        var axis = (int)orientation.PrimaryAxis();
        return model.Where(v =>
            axisCoordinate.IsApprox(v.Key[axis])
            && IsVoxelVisible(v.Key, model, orientation, null)
        ).ToDictionary();
    }

    private static void BuildMeshesForOrientation(
        FaceOrientation orientation, Dictionary<Vector3, VoxelColor> slice,
        float axisCoordinate,
        SurfaceTool st,
        Material material,
        Vector3 min,
        Vector3 max,
        float scale,
        bool groundOrigin
    ) {
        var primaryAxis = (int)orientation.PrimaryAxis();
        var widthAxis = (int)orientation.WidthAxis();
        var heightAxis = (int)orientation.HeightAxis();

        Vector3 currentValue = new()
        {
            [primaryAxis] = axisCoordinate,
            [heightAxis] = min[heightAxis],
            [widthAxis] = min[widthAxis],
        };

        // loop over the "rectangle"
        while (currentValue[heightAxis] <= max[heightAxis]) {
            currentValue[widthAxis] = min[widthAxis];
            while (currentValue[widthAxis] <= max[widthAxis]) {
                if (slice.ContainsKey(currentValue)) {
                    GenerateMeshForFace(orientation, slice, currentValue, st, material, scale, min.Z, groundOrigin);
                }

                currentValue[widthAxis] += 1;
            }

            currentValue[heightAxis] += 1;
        }
    }

    private static void GenerateMeshForFace(
        FaceOrientation orientation, Dictionary<Vector3, VoxelColor> slice,
        Vector3 origin,
        SurfaceTool st,
        Material material,
        float scale,
        float minZ,
        bool groundOrigin
    ) {
        var primaryAxis = (int)orientation.PrimaryAxis();
        var widthAxis = (int)orientation.WidthAxis();
        var heightAxis = (int)orientation.HeightAxis();

        var width = WidthForFace(slice, orientation, origin);
        var height = HeightForFace(slice, orientation, origin, width);

        var scaleMultiplier = Vector3.One;
        scaleMultiplier[widthAxis] *= width;
        scaleMultiplier[heightAxis] *= height;

        var color = slice[origin].Color;
        color.A = material.HasMeta("alpha") ? (float)material.GetMeta("alpha").AsDouble() : 1f;

        st.SetColor(color);
        st.SetNormal(orientation.GetNormal());

        var grounding = new Vector3
        {
            Y = groundOrigin ? minZ * scale : 0
        };

        foreach (Vector3 vertex in orientation.GetFaces()) {
            st.AddVertex((VoxToGodot * (vertex * scaleMultiplier + origin) * scale) - grounding);
        }

        var current = new Vector3
        {
            [primaryAxis] = origin[primaryAxis]
        };
        for (int heightIndex = 0; heightIndex < height; heightIndex += 1) {
            current[heightAxis] = heightIndex + origin[heightAxis];
            for (int widthIndex = 0; widthIndex < width; widthIndex += 1) {
                current[widthAxis] = widthIndex + origin[widthAxis];
                slice.Remove(current);
            }
        }
    }

    private static int WidthForFace(Dictionary<Vector3, VoxelColor> slice,
        FaceOrientation orientation,
        Vector3 origin
    ) {
        var widthAxis = (int)orientation.WidthAxis();
        var color = slice[origin];
        var current = origin;

        while (slice.TryGetValue(current, out var c) && c == color) {
            current[widthAxis] += 1;
        }

        return (int)(current[widthAxis] - origin[widthAxis]);
    }

    private static int HeightForFace(Dictionary<Vector3, VoxelColor> slice,
        FaceOrientation orientation,
        Vector3 origin,
        int width
    ) {
        var heightAxis = (int)orientation.HeightAxis();
        var color = slice[origin];
        var current = origin;

        //start on the next instance of height
        current[heightAxis] += 1;

        while (slice.TryGetValue(current, out var c)
               && c == color
               && WidthForFace(slice, orientation, current) >= width) {
            current[heightAxis] += 1;
        }

        return (int)(current[heightAxis] - origin[heightAxis]);
    }

    private static bool IsVoxelVisible(
        Vector3 voxel, Dictionary<Vector3, VoxelColor> model,
        FaceOrientation orientation,
        VoxelCuller? culler
    ) {
        // Is there a voxel next to us in the chosen orientation?
        if (model.ContainsKey(voxel + orientation.NeighborFor())) {
            return false;
        }

        if (culler == null) {
            return true;
        }

        // Is the square next to us open air and visible to the outer camera
        return culler.IsVoxelVisible(voxel + orientation.NeighborFor());
    }

}