using System;
using VoxelImporter.addons.voxel_importer.Constants;
using VoxelImporter.addons.voxel_importer.Data;
using Godot;
using Godot.Collections;

namespace VoxelImporter.addons.voxel_importer.Importers;

[Tool]
public partial class PackedSceneImporter : EditorImportPlugin {

    // Constants tm
    public override int _GetPresetCount() => 0;
    public override string _GetPresetName(int presetIndex) => "Unknown";
    public override int _GetImportOrder() => 0;
    public override float _GetPriority() => 1;
    public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options) => true;

    // Per plugin unique
    public override string _GetImporterName() => "voxel.scene";
    public override string _GetVisibleName() => "Packed Scene";
    public override string[] _GetRecognizedExtensions() => ["vox"];
    public override string _GetResourceType() => "PackedScene";
    public override string _GetSaveExtension() => "scn";

    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex) => ImportOptions.Build(
        ImportOptions.BuildPackedSceneType(),
        ImportOptions.GenerateCollisionType()
    );

    // Imports the mesh as a packed scene. Unique objects are imported as meshes or mesh libraries if they have
    // multiple frames. 
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

        VoxFile vox;
        try {
            vox = VoxelImporter.Parse(access);
        } catch (Exception e) {
            GD.PushError(e.Message);
            return Error.InvalidData;
        }

        var fileName = sourceFile.GetFile().Replace(".vox", "");
        var scene = VoxelImporter.SingleScene(
            sceneName: fileName,
            vox: vox,
            selector: options.PackedSceneType(),
            scale: options.GetScale(),
            includeHidden: options.IncludeHidden(),
            groundOrigin: options.GroundOrigin(),
            ignoreTransforms: options.IgnoreTransforms(),
            applyMaterials: options.ApplyMaterials(),
            collisionGenerationType: options.CollisionGenerationType()
        );

        return ResourceSaver.Save(scene, outputPath);
    }

}