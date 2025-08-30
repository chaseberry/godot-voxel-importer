#if TOOLS
using Godot;
using Godot.Collections;
using VoxelImporter.addons.voxel_importer.Importers;
using static VoxelImporter.addons.voxel_importer.Importers.ImportOptions;

namespace VoxelImporter.addons.voxel_importer;

[Tool]
public partial class VoxelImporterPlugin : EditorPlugin {

    private MeshImporter _mi = null!;
    private MeshLibraryImporter _mli = null!;
    private PackedSceneImporter _psi = null!;

    public override void _EnterTree() {
        _mli = new();
        _mi = new();
        _psi = new();

        AddImportPlugin(_mi);
        AddImportPlugin(_mli);
        AddImportPlugin(_psi);
    }

    public override void _ExitTree() {
        RemoveImportPlugin(_mi);
        RemoveImportPlugin(_mli);
        RemoveImportPlugin(_psi);

        _mi = null!;
        _mli = null!;
        _psi = null!;
    }

}

#endif