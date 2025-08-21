#if TOOLS
using Godot;
using Godot.Collections;
using VoxelImporter.addons.voxel_importer.Importers;
using static VoxelImporter.addons.voxel_importer.Importers.ImportOptions;

namespace VoxelImporter.addons.voxel_importer;

[Tool]
public partial class VoxelImporterPlugin : EditorPlugin {

    private CombinedMeshImporter _cmi = null!;
    private CombinedMeshLibraryImporter _cmli = null!;
    private SeperateMeshImporter _smi = null!;
    private SeperateMeshLibraryImporter _smli = null!;
    private ObjectsAsMeshLibImporter _oami = null!;
    private PackedSceneImporter _psi = null!;

    public override void _EnterTree() {
        ConfigureDefaultSettings();

        _cmi = new();
        _cmli = new();
        _smi = new();
        _smli = new();
        _psi = new();
        _oami = new();

        AddImportPlugin(_cmi);
        AddImportPlugin(_cmli);
        AddImportPlugin(_oami);
        AddImportPlugin(_smi);
        AddImportPlugin(_smli);
        AddImportPlugin(_psi);
    }

    public override void _ExitTree() {
        RemoveImportPlugin(_cmi);
        RemoveImportPlugin(_cmli);
        RemoveImportPlugin(_oami);
        RemoveImportPlugin(_smi);
        RemoveImportPlugin(_smli);
        RemoveImportPlugin(_psi);

        _cmi = null!;
        _cmli = null!;
        _oami = null!;
        _smi = null!;
        _smli = null!;
        _psi = null!;
    }

    private void ConfigureDefaultSettings() {
        CheckAndSet(ScaleSetting, 1.0f, (long)Variant.Type.Float, (long)PropertyHint.None);
        CheckAndSet(IncludeInvisibleSetting, false, (long)Variant.Type.Bool, (long)PropertyHint.None);
        CheckAndSet(OriginAtBottomSetting, false, (long)Variant.Type.Bool, (long)PropertyHint.None);
        CheckAndSet(IgnoreTransformsSetting, true, (long)Variant.Type.Bool, (long)PropertyHint.None);
        CheckAndSet(ApplyMaterialsSetting, false, (long)Variant.Type.Bool, (long)PropertyHint.None);
        CheckAndSet(MergeAllFramesSetting, false, (long)Variant.Type.Bool, (long)PropertyHint.None);
        CheckAndSet(BuildOutputPathSetting, "", (long)Variant.Type.String, (long)PropertyHint.None);
        CheckAndSet(BuildOutputHeaderSetting, "", (long)Variant.Type.String, (long)PropertyHint.None);
        CheckAndSet(
            PackedSceneTypeSetting,
            "Smart Objects",
            (long)Variant.Type.Int,
            (long)PropertyHint.Enum,
            "Smart Objects,First Key Frame,Merge Key Frames"
        );
        CheckAndSet(
            CollisionGenerationTypeSetting,
            "None",
            (long)Variant.Type.Int,
            (long)PropertyHint.Enum,
            "None,Box,Concave Polygon,Simple Convex Polygon,Complex Convex Polygon"
        );


        ProjectSettings.Save();
    }

    private static void CheckAndSet(string setting, Variant value, long type, long hint, string? hintString = null) {
        if (!ProjectSettings.HasSetting(setting)) {
            ProjectSettings.SetSetting(setting, value);
            ProjectSettings.AddPropertyInfo(Info(setting, type, hint, hintString));
        }
    }

    private static Dictionary Info(string setting, long type, long hint, string? hintString) {
        Dictionary d = new() {
            ["name"] = setting,
            ["type"] = type,
            ["hint"] = hint,
        };

        if (hintString != null) {
            d["hint_string"] = hintString;
        }

        return d;
    }

}

#endif