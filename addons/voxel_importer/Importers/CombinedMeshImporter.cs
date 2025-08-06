using System;
using VoxelImporter.addons.voxel_importer.Constants;
using Godot;
using Godot.Collections;

namespace VoxelImporter.addons.voxel_importer.Importers;

[Tool]
public partial class CombinedMeshImporter : EditorImportPlugin {

    // Constants tm
    public override int _GetPresetCount() => 0;
    public override string _GetPresetName(int presetIndex) => "Unknown";
    public override int _GetImportOrder() => 0;
    public override float _GetPriority() => 1;
    public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options) => true;

    // Per plugin unique
    public override string _GetImporterName() => "voxel.object.import";
    public override string _GetVisibleName() => "Voxel Object";
    public override string[] _GetRecognizedExtensions() => ["vox"];
    public override string _GetResourceType() => "Mesh";
    public override string _GetSaveExtension() => "mesh";

    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex) => ImportOptions.Build(
        ImportOptions.MergeAllFrames()
    );

    // Imports all objects in the voxel file into a single mesh instance. 
    // Either use the first 'frame' from each object, or merge all frames together
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

        Resource resource;
        try {
            resource = VoxelImporter.Import(0, access, options);
        } catch (Exception e) {
            GD.PushError(e.Message);
            return Error.InvalidData;
        }
        
        return ResourceSaver.Save(resource, outputPath);
    }

}