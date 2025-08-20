using VoxelImporter.addons.voxel_importer.Functions;
using Godot;
using Godot.Collections;

namespace VoxelImporter.addons.voxel_importer.Importers;

public static class ImportOptions {

    public const string ScaleSetting = "voxImporter/defaults/scale";
    public const string IncludeInvisibleSetting = "voxImporter/defaults/includeInvisible";
    public const string OriginAtBottomSetting = "voxImporter/defaults/originAtBottom";
    public const string IgnoreTransformsSetting = "voxImporter/defaults/ignoreTransforms";
    public const string ApplyMaterialsSetting = "voxImporter/defaults/applyMaterials";
    public const string MergeAllFramesSetting = "voxImporter/defaults/mergeAllFrames";
    public const string BuildOutputPathSetting = "voxImporter/defaults/buildOutputPath";
    public const string BuildOutputHeaderSetting = "voxImporter/defaults/buildOutputHeader";
    public const string PackedSceneTypeSetting = "voxImporter/defaults/packedSceneType";
    public const string CollisionGenerationTypeSetting = "voxImporter/defaults/collisionGenerationType";

    public static Array<Dictionary> Build(params Dictionary[] opts) {
        var r = Defaults();

        opts.ForEach(o => r.Add(o));

        return r;
    }

    private static Array<Dictionary> Defaults() => [
        new() {
            ["name"] = "Scale",
            ["default_value"] = GetDefault(ScaleSetting, 1.0),
            ["description"] = "How to Scales the voxel model"
        },
        new() {
            ["name"] = "Include Invisible",
            ["default_value"] = GetDefault(IncludeInvisibleSetting, false),
            ["description"] = "Include invisible models"
        },
        new() {
            ["name"] = "Set Origin At Bottom",
            ["default_value"] = GetDefault(OriginAtBottomSetting, false),
            ["description"] = ""
        },
        new() {
            ["name"] = "Ignore Transforms",
            ["default_value"] = GetDefault(IgnoreTransformsSetting, true),
            ["description"] = ""
        },
        new() {
            ["name"] = "Apply Materials",
            ["default_value"] = GetDefault(ApplyMaterialsSetting, false),
            ["description"] = ""
        }
    ];

    public static Dictionary MergeAllFrames() => new() {
        ["name"] = "Merge All Frames",
        ["default_value"] = GetDefault(MergeAllFramesSetting, false),
        ["description"] = "Merge all key frames into one model"
    };

    public static Dictionary BuildOutputOption(string path) {
        return new() {
            ["name"] = "Output Directory",
            ["default_value"] = GetDefault(BuildOutputPathSetting, path.Replace(path.GetFile(), "")),
            ["description"] = "Output directory for generated meshes"
        };
    }

    public static Dictionary BuildOutputHeader(string path) {
        return new() {
            ["name"] = "Output Header",
            ["default_value"] = GetDefault(BuildOutputHeaderSetting, path.GetFile().Replace(".vox", "")),
            ["description"] = "Output file header name"
        };
    }

    public static Dictionary BuildPackedSceneType() => new() {
        ["name"] = "Packed Scene Logic",
        ["default_value"] = GetDefault(PackedSceneTypeSetting, "Smart Objects"),
        ["description"] = "How to handle making packed scenes",
        ["property_hint"] = (int)PropertyHint.Enum,
        ["hint_string"] = "Smart Objects, First Key Frame, Merge Key Frames"
    };

    public static Dictionary GenerateCollisionType() => new() {
        ["name"] = "Generate Collision Type",
        ["default_value"] = GetDefault(CollisionGenerationTypeSetting, "None"),
        ["description"] = "Generate collision type",
        ["property_hint"] = (int)PropertyHint.Enum,
        ["hint_string"] = "None, Box, Concave Polygon, Simple Convex Polygon, Complex Convex Polygon",
    };

    public static float GetScale(this Dictionary options) {
        if (!options.TryGetValue("Scale", out var scale)) {
            scale = 1f;
        }

        return scale.AsSingle();
    }

    public static bool MergeFrames(this Dictionary options) {
        if (!options.TryGetValue("Merge All Frames", out var merge)) {
            merge = false;
        }

        return merge.AsBool();
    }

    public static bool GroundOrigin(this Dictionary options) {
        if (!options.TryGetValue("Set Origin At Bottom", out var ground)) {
            ground = false;
        }

        return ground.AsBool();
    }

    public static bool IncludeHidden(this Dictionary options) {
        if (!options.TryGetValue("Include Invisible", out var includeHidden)) {
            includeHidden = false;
        }

        return includeHidden.AsBool();
    }

    public static bool IgnoreTransforms(this Dictionary options) {
        if (!options.TryGetValue("Ignore Transforms", out var ignoreTransforms)) {
            ignoreTransforms = true;
        }

        return ignoreTransforms.AsBool();
    }

    public static bool ApplyMaterials(this Dictionary options) {
        if (!options.TryGetValue("Apply Materials", out var applyMaterials)) {
            applyMaterials = false;
        }

        return applyMaterials.AsBool();
    }

    public static string OutputPath(this Dictionary options, string sourcePath) {
        if (!options.TryGetValue("Output Directory", out var dir)) {
            dir = sourcePath.Replace(sourcePath.GetFile(), "");
        }

        if (!options.TryGetValue("Output Header", out var name)) {
            name = sourcePath.GetFile().Replace(".vox", "");
        }

        return dir.AsString() + name.AsString();
    }

    public static KeyFrameSelector? PackedSceneType(this Dictionary options) {
        if (!options.TryGetValue("Packed Scene Logic", out var logic)) {
            logic = 2;
        }

        return logic.AsString() switch {
            "First Key Frame" => new KeyFrameSelector.FirstKeyFrame(),
            "Merge Key Frames" => new KeyFrameSelector.CombinedKeyFrame(),
            _ => null
        };
    }

    public static CollisionGenerationType CollisionGenerationType(this Dictionary options) {
        if (!options.TryGetValue("Generate Collision Type", out var generationType)) {
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

    public static Variant GetDefault(string key, Variant def) {
        if (!ProjectSettings.HasSetting(key)) {
            return def;
        }

        var v = ProjectSettings.GetSetting(key);
        if (v.VariantType == Variant.Type.String && v.AsString().IsNullOrBlank()) {
            return def;
        }

        return v;
    }

}