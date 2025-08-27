using System;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Functions;
using Godot;
using Godot.Collections;
using VoxelImporter.addons.voxel_importer.Data;

namespace VoxelImporter.addons.voxel_importer.Importers;

public static class ImportOptions {

    public const string ObjectOption = "object";
    public const string FrameOption = "frame";
    public const string ScaleOption = "common/scale";
    public const string IncludeInvisibleOption = "common/include_invisible";
    public const string SetOriginAtBottomOption = "common/set_origin_at_bottom";
    public const string IgnoreTransformsOption = "common/ignore_transforms";
    public const string ApplyMaterialsOption = "common/apply_materials";
    public const string ExportRemainingObjectsOption = "remaining_objects/export";
    public const string OutputDirectoryOption = "remaining_objects/output_directory";
    public const string OutputHeaderOption = "remaining_objects/output_header";
    public const string PackedSceneLogicOption = "packed_scene_logic";
    public const string GenerateCollisionTypeOption = "generate_collision_type";
    public const string MergeAll = "Merge All";

    enum PackedSceneValues {

        SmartObjects,
        FirstKeyFrame,
        MergeKeyFrames

    }

    public static Array<Dictionary> Build(params Dictionary?[] opts) {
        var r = new Array<Dictionary>();

        opts.OfType<Dictionary>().ForEach(o => r.Add(o));

        return r;
    }

    public static Dictionary Option(
        string name,
        Variant defaultValue,
        string description,
        Variant propertyHint = default,
        Variant hintString = default
    ) => new() {
        ["name"] = name,
        ["default_value"] = defaultValue,
        ["description"] = description,
        ["property_hint"] = propertyHint,
        ["hint_string"] = hintString
    };

    public static Array<Dictionary> Defaults(this Array<Dictionary> opts) {
        opts.AddRange(
            [
                Option(ScaleOption, 1.0f, "How to Scales the voxel model"),
                Option(IncludeInvisibleOption, false, "Include invisible models"),
                Option(SetOriginAtBottomOption, false, "Set origin at bottom"),
                Option(IgnoreTransformsOption, true, "Ignore transforms"),
                Option(ApplyMaterialsOption, false, "Apply materials"),
            ]
        );

        return opts;
    }

    public static Array<Dictionary> RemainingExports(this Array<Dictionary> opts, VoxFile? vox, string path) {
        if (vox == null || vox.GatherObjects(true).Count <= 1) {
            return opts;
        }

        opts.AddRange(
            [
                Option(ExportRemainingObjectsOption, false, ""),
                Option(OutputDirectoryOption, path.Replace(path.GetFile(), ""), ""),
                Option(OutputHeaderOption, path.GetFile().Replace(".vox", ""), ""),
            ]
        );
        return opts;
    }

    public static Dictionary MergeAllFrames() => new() {
        ["name"] = "Merge All Frames",
        ["default_value"] = false,
        ["description"] = "Merge all key frames into one model"
    };

    public static Dictionary BuildOutputOption(string path) {
        return new() {
            ["name"] = OutputDirectoryOption,
            ["default_value"] = path.Replace(path.GetFile(), ""),
            ["description"] = "Output directory for generated meshes"
        };
    }

    public static Dictionary BuildOutputHeader(string path) {
        return new() {
            ["name"] = OutputHeaderOption,
            ["default_value"] = path.GetFile().Replace(".vox", ""),
            ["description"] = "Output file header name"
        };
    }

    public static Dictionary BuildPackedSceneType() => new() {
        ["name"] = PackedSceneLogicOption,
        ["default_value"] = "Smart Objects",
        ["description"] = "How to handle making packed scenes",
        ["property_hint"] = (int)PropertyHint.Enum,
        ["hint_string"] = string.Join(",", Enum.GetValues(typeof(PackedSceneValues)))
    };

    public static Dictionary GenerateCollisionType() => new() {
        ["name"] = GenerateCollisionTypeOption,
        ["default_value"] = "None",
        ["description"] = "Generate collision type",
        ["property_hint"] = (int)PropertyHint.Enum,
        ["hint_string"] = string.Join(",", Enum.GetValues(typeof(CollisionGenerationType)))
    };

    public static Dictionary? Object(VoxFile? vox) {
        if (vox == null) {
            return null;
        }

        var objects = vox.GatherObjects(true);
        if (objects.Count <= 1) {
            return null;
        }

        return Option(
            name: ObjectOption,
            description: "",
            defaultValue: MergeAll,
            propertyHint: (int)PropertyHint.Enum,
            hintString: string.Join(
                ",",
                objects.Select((o, idx) => o.Chain.OfType<VoxelTransformNode>().Last().Name ?? $"Object {idx}")
            ) + $",{MergeAll}"
        );
    }

    public static Dictionary? Frames(VoxFile? vox) {
        if (vox == null) {
            return null;
        }

        var objects = vox.GatherObjects(true);
        var frameIndexes = objects.SelectMany(o => o.VoxelObject.Frames.Keys).Distinct().ToList();
        if (frameIndexes.Count <= 1) {
            return null;
        }

        return Option(
            name: FrameOption,
            description: "",
            defaultValue: MergeAll,
            propertyHint: (int)PropertyHint.Enum,
            hintString: string.Join(
                ",",
                frameIndexes.Select(o => $"Frame {o}")
            ) + $",{MergeAll}"
        );
    }

    public static float GetFloat(Dictionary options, string key, float defaultValue) {
        if (!options.TryGetValue(key, out var floatValue)) {
            floatValue = defaultValue;
        }

        return floatValue.AsSingle();
    }

    public static bool GetBool(Dictionary options, string key, bool defaultValue) {
        if (!options.TryGetValue(key, out var boolValue)) {
            boolValue = defaultValue;
        }

        return boolValue.AsBool();
    }

    public static string GetString(Dictionary options, string key, string defaultValue) {
        if (!options.TryGetValue(key, out var stringValue)) {
            stringValue = defaultValue;
        }

        return stringValue.AsString();
    }


    public static bool MergeFrames(this Dictionary options) {
        if (!options.TryGetValue("Merge All Frames", out var merge)) {
            merge = false;
        }

        return merge.AsBool();
    }

    public static string GetObject(this Dictionary options) => GetString(options, ObjectOption, MergeAll);

    public static string GetFrame(this Dictionary options) => GetString(options, FrameOption, MergeAll);

    public static float GetScale(this Dictionary options) => GetFloat(options, ScaleOption, 1.0f);

    public static bool GroundOrigin(this Dictionary options) => GetBool(options, SetOriginAtBottomOption, false);

    public static bool IncludeInvisible(this Dictionary options) => GetBool(options, IncludeInvisibleOption, false);

    public static bool IgnoreTransforms(this Dictionary options) => GetBool(options, IgnoreTransformsOption, true);

    public static bool ApplyMaterials(this Dictionary options) => GetBool(options, ApplyMaterialsOption, false);

    public static string OutputPath(this Dictionary options, string sourcePath) {
        if (!options.TryGetValue(OutputDirectoryOption, out var dir)) {
            dir = sourcePath.Replace(sourcePath.GetFile(), "");
        }

        if (!options.TryGetValue(OutputHeaderOption, out var name)) {
            name = sourcePath.GetFile().Replace(".vox", "");
        }

        return dir.AsString() + name.AsString();
    }

    public static KeyFrameSelector? PackedSceneType(this Dictionary options) {
        if (!options.TryGetValue(PackedSceneLogicOption, out var logic)) {
            logic = 2;
        }

        return logic.AsString() switch {
            "First Key Frame" => new KeyFrameSelector.FirstKeyFrame(),
            "Merge Key Frames" => new KeyFrameSelector.CombinedKeyFrame(),
            _ => null
        };
    }

    public static CollisionGenerationType CollisionGenerationType(this Dictionary options) {
        if (!options.TryGetValue(GenerateCollisionTypeOption, out var generationType)) {
            generationType = "None";
        }

        return generationType.AsString() switch {
            "Box" => Importers.CollisionGenerationType.Box,
            "Concave Polygon" => Importers.CollisionGenerationType.ConcavePolygon,
            "Simple Convex Polygon" => Importers.CollisionGenerationType.SimpleConvexPolygon,
            "Complex Convex Polygon" => Importers.CollisionGenerationType.ComplexConvexPolygon,
            _ => Importers.CollisionGenerationType.None
        };
    }

}