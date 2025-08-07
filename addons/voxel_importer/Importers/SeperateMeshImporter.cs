using System;
using VoxelImporter.addons.voxel_importer.Constants;
using Godot;
using Godot.Collections;

namespace VoxelImporter.addons.voxel_importer.Importers;

[Tool]
public partial class SeperateMeshImporter : EditorImportPlugin {

    // Constants tm
    public override int _GetPresetCount() => 0;
    public override string _GetPresetName(int presetIndex) => "Unknown";
    public override int _GetImportOrder() => 0;
    public override float _GetPriority() => 1;
    public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options) => true;

    // Per plugin unique
    public override string _GetImporterName() => "voxel.separate.meshes";
    public override string _GetVisibleName() => "Objects as Meshes";
    public override string[] _GetRecognizedExtensions() => ["vox"];
    public override string _GetResourceType() => "Mesh";
    public override string _GetSaveExtension() => "mesh";

    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex) => ImportOptions.Build(
        ImportOptions.MergeAllFrames(),
        ImportOptions.BuildOutputOption(path),
        ImportOptions.BuildOutputHeader(path)
    );

    private string Secondary(string path, string name, string ext) => $"{path}_{name}.{ext}";

    // Imports each object in the voxel model as a separate mesh, optionally combing frames
    // The first mesh is saved as the imported resource, all other meshes are saved as meshes in the specified output dir
    public override Error _Import(
        string sourceFile,
        string savePath,
        Dictionary options,
        Array<string> platformVariants,
        Array<string> genFiles
    ) {
        var outputPath = $"{savePath}.{_GetSaveExtension()}";
        if (VoxelImporter.LoadFile(sourceFile, out var access) == Error.CantOpen) {
            return FileAccess.GetOpenError();
        }

        var root = options.OutputPath(sourceFile);

        MeshLibrary meshes;
        try {
            meshes = (MeshLibrary)VoxelImporter.Import(2, access, options);
        } catch (Exception e) {
            GD.PushError(e.Message);
            return Error.InvalidData;
        }

        var idList = meshes.GetItemList();
        var primary = idList[0];
        foreach (var id in idList) {
            if (id == primary) {
                continue;
            }

            var mesh = meshes.GetItemMesh(id);
            var name = meshes.GetItemName(id) ?? id.ToString();
            ResourceSaver.Save(mesh, Secondary(root, name, _GetSaveExtension()));
        }

        return ResourceSaver.Save(meshes.GetItemMesh(primary), outputPath);
    }

}