using System;
using Godot;
using Godot.Collections;
using VoxelImporter.addons.voxel_importer.Data;
using VoxelImporter.addons.voxel_importer.Functions;

namespace VoxelImporter.addons.voxel_importer.Importers;

[Tool]
public partial class MeshLibraryImporter : EditorImportPlugin {

    // Constants tm
    public override int _GetPresetCount() => 0;

    public override string _GetPresetName(int presetIndex) => "Unknown";

    public override int _GetImportOrder() => 0;

    public override float _GetPriority() => 1;

    public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options) => true;

    // Per plugin unique
    public override string _GetImporterName() => "voxel.import.meshLibrary";

    public override string _GetVisibleName() => "TEST > MeshLibrary";

    public override string[] _GetRecognizedExtensions() => ["vox"];

    public override string _GetResourceType() => "MeshLibrary";

    public override string _GetSaveExtension() => "meshlib";


    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex) {
        var vox = VoxelImporter.ParseFile(path);
        var opts = ImportOptions.Build(
                ImportOptions.MeshLibraryType(),
                ImportOptions.Object(vox),
                ImportOptions.Frames(vox),
                ImportOptions.GenerateCollisionType()
            )
            .Defaults()
            .RemainingExports(vox, path);

        return opts;
    }

    public override Error _Import(
        string sourceFile,
        string savePath,
        Dictionary options,
        Array<string> platformVariants,
        Array<string> genFiles
    ) {
        if (VoxelImporter.LoadFile(sourceFile, out var access) == Error.CantOpen) {
            return FileAccess.GetOpenError();
        }

        var vox = VoxelImporter.Parse(access);
        var outputPath = $"{savePath}.{_GetSaveExtension()}";

        Resource resource;
        try {
            resource = options.MeshLibraryType() == MeshLibraryTypeEnum.Animation
                ? ImportAsAnimation(vox, options, sourceFile)
                : ImportAsObjectLibrary(vox, options);
        }
        catch (Exception e) {
            GD.PushError(e.Message);
            return Error.InvalidData;
        }

        return ResourceSaver.Save(resource, outputPath);
    }

    private Resource ImportAsAnimation(
        VoxFile vox,
        Dictionary options,
        string sourceFile
    ) {
        var objectName = options.GetObject();

        ObjectSelector objectSelection = objectName == ImportOptions.MergeAll
            ? new ObjectSelector.MergeAll()
            : new ObjectSelector.ByName(objectName);

        var voxelObjects = vox.GatherObjects(options.IncludeInvisible());
        var primary = objectSelection.GetObjects(voxelObjects);

        var meshLib = VoxelImporter.BuildMeshLibrary(
            VoxelImporter.AnimationLibrary(primary, vox.GetFrameIndexes(), options.IgnoreTransforms()),
            options.GetScale(),
            options.GroundOrigin(),
            options.ApplyMaterials(),
            options.CollisionGenerationType()
        );

        if (options.ExportRemaining()
            && objectSelection is not ObjectSelector.MergeAll
            && voxelObjects.Count > 1) {
            var root = options.OutputPath(sourceFile);
            voxelObjects.RemoveAll(o => primary.Contains(o));

            foreach (var secondary in voxelObjects) {
                var lib = VoxelImporter.BuildMeshLibrary(
                    VoxelImporter.AnimationLibrary(
                        VoxUtils.ListOf(secondary),
                        vox.GetFrameIndexes(),
                        options.IgnoreTransforms()
                    ),
                    options.GetScale(),
                    options.GroundOrigin(),
                    options.ApplyMaterials(),
                    options.CollisionGenerationType()
                );
                ResourceSaver.Save(lib, VoxelImporter.SecondarySavePath(root, secondary.Name(), _GetSaveExtension()));
            }
        }

        return meshLib;
    }

    private Resource ImportAsObjectLibrary(
        VoxFile vox,
        Dictionary options
    ) {
        var frameName = options.GetFrame();
        KeyFrameSelector keyFrame =
            frameName == ImportOptions.MergeAll
                ? new KeyFrameSelector.CombinedKeyFrame()
                : new KeyFrameSelector.SpecificKeyFrames(VoxelImporter.FrameRegex.Match(frameName).Groups[1].Value.ToInt());

        return VoxelImporter.BuildMeshLibrary(
            VoxelImporter.SeparateObjects(vox, keyFrame, options.IncludeInvisible(), options.IgnoreTransforms()),
            options.GetScale(),
            options.GroundOrigin(),
            options.ApplyMaterials(),
            options.CollisionGenerationType()
        );
    }

    private VoxFile? Get(string path) {
        if (VoxelImporter.LoadFile(path, out var access) == Error.CantOpen) {
            return null;
        }

        return VoxelImporter.Parse(access);
    }

}