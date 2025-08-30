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

    public override void _EnterTree() {
        _mli = new();
        _mi = new();

        AddImportPlugin(_mi);
        AddImportPlugin(_mli);
    }

    public override void _ExitTree() {
        RemoveImportPlugin(_mi);
        RemoveImportPlugin(_mli);

        _mi = null!;
        _mli = null!;
    }

}

#endif