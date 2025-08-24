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
    private MeshImporter _mi = null!;

    public override void _EnterTree() {

        _cmi = new();
        _cmli = new();
        _smi = new();
        _smli = new();
        _psi = new();
        _oami = new();
        _mi = new();

        // AddImportPlugin(_cmi);
        // AddImportPlugin(_cmli);
        // AddImportPlugin(_oami);
        // AddImportPlugin(_smi);
        // AddImportPlugin(_smli);
        // AddImportPlugin(_psi);
        AddImportPlugin(_mi);
    }

    public override void _ExitTree() {
        // RemoveImportPlugin(_cmi);
        // RemoveImportPlugin(_cmli);
        // RemoveImportPlugin(_oami);
        // RemoveImportPlugin(_smi);
        // RemoveImportPlugin(_smli);
        // RemoveImportPlugin(_psi);
        RemoveImportPlugin(_mi);

        _cmi = null!;
        _cmli = null!;
        _oami = null!;
        _smi = null!;
        _smli = null!;
        _psi = null!;
        _mi = null!;
    }

}

#endif