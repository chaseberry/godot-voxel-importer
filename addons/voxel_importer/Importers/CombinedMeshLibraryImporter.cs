using System;
using VoxelImporter.addons.voxel_importer.Constants;
using Godot;
using Godot.Collections;

namespace VoxelImporter.addons.voxel_importer.Importers;

[Tool]
public partial class CombinedMeshLibraryImporter : EditorImportPlugin {

    // Constants tm
    public override int _GetPresetCount() => 0;
    public override string _GetPresetName(int presetIndex) => "Unknown";
    public override int _GetImportOrder() => 0;
    public override float _GetPriority() => 1;
    public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options) => true;

    // Per plugin unique
    public override string _GetImporterName() => "voxel.object.frames.import";
    public override string _GetVisibleName() => "Voxel Object Mesh Library";
    public override string[] _GetRecognizedExtensions() => ["vox"];
    public override string _GetResourceType() => "MeshLibrary";
    public override string _GetSaveExtension() => "meshlib";

    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex) => ImportOptions.Build();

    // Creates a mesh library, where each item is a unique 'frame' from the voxel file, and each frame is a combination
    // of all objects at that frame.
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
            resource = VoxelImporter.Import(1, access, options);
        } catch (Exception e) {
            GD.PushError(e.Message);
            return Error.InvalidData;
        }

        return ResourceSaver.Save(resource, outputPath);
    }

}