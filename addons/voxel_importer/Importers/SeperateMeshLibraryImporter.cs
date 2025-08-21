using System;
using VoxelImporter.addons.voxel_importer.Constants;
using VoxelImporter.addons.voxel_importer.Data;
using Godot;
using Godot.Collections;

namespace VoxelImporter.addons.voxel_importer.Importers;

[Tool]
public partial class SeperateMeshLibraryImporter : EditorImportPlugin {

    // Constants tm
    public override int _GetPresetCount() => 0;
    public override string _GetPresetName(int presetIndex) => "Unknown";
    public override int _GetImportOrder() => 0;
    public override float _GetPriority() => 1;
    public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options) => true;

    // Per plugin unique
    public override string _GetImporterName() => "voxel.separate.meshLibs";
    public override string _GetVisibleName() => "Objects as Mesh Libraries";
    public override string[] _GetRecognizedExtensions() => ["vox"];
    public override string _GetResourceType() => "MeshLibrary";
    public override string _GetSaveExtension() => "meshlib";

    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex) => ImportOptions.Build(
        ImportOptions.BuildOutputOption(path),
        ImportOptions.BuildOutputHeader(path),
        ImportOptions.GenerateCollisionType()
    );

    private string Secondary(string path, string name, string ext) => $"{path}_{name}.{ext}";

    // Imports each object as a mesh library, with each item in the library being the frames for that specific object.
    // The first object is the imported resource, all other objects are imported as mesh libraries to the
    // specified directory
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

        var root = options.OutputPath(sourceFile);

        var objs = VoxelImporter.SeparateMeshLibraries(
            vox,
            options.IncludeHidden(),
            options.IgnoreTransforms()
        );

        for (int z = 1; z < objs.Count; z++) {
            var (name, meshData) = objs[z];

            var mesh = VoxelImporter.BuildMeshLibrary(
                meshData,
                options.GetScale(),
                options.GroundOrigin(),
                options.ApplyMaterials(),
                options.CollisionGenerationType()
            );

            ResourceSaver.Save(mesh, Secondary(root, name ?? z.ToString(), _GetSaveExtension()));
        }

        return ResourceSaver.Save(
            VoxelImporter.BuildMeshLibrary(
                objs[0].Item2,
                options.GetScale(),
                options.GroundOrigin(),
                options.ApplyMaterials(),
                options.CollisionGenerationType()
            ),
            outputPath
        );
    }

}