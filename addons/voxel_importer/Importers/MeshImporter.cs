using System;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;
using Godot.Collections;
using VoxelImporter.addons.voxel_importer.Data;
using VoxelImporter.addons.voxel_importer.Functions;

namespace VoxelImporter.addons.voxel_importer.Importers;

[Tool]
public partial class MeshImporter : EditorImportPlugin {

    private readonly Regex _frameRegex = new("Frame (\\d+)");

    // Constants tm
    public override int _GetPresetCount() => 0;
    public override string _GetPresetName(int presetIndex) => "Unknown";
    public override int _GetImportOrder() => 0;
    public override float _GetPriority() => 1;
    public override bool _GetOptionVisibility(string path, StringName optionName, Dictionary options) => true;

    // Per plugin unique
    public override string _GetImporterName() => "voxel.import.mesh";
    public override string _GetVisibleName() => "TEST > Mesh";
    public override string[] _GetRecognizedExtensions() => ["vox"];
    public override string _GetResourceType() => "Mesh";
    public override string _GetSaveExtension() => "mesh";

    public override Array<Dictionary> _GetImportOptions(string path, int presetIndex) {
        var vox = Get(path);

        var opts = ImportOptions.Build(
                ImportOptions.Object(vox),
                ImportOptions.Frames(vox)
            )
            .Defaults()
            .RemainingExports(vox, path);

        return opts;
    }

    // Imports all objects in the voxel file into a single mesh instance. 
    // Either use the first 'frame' from each object, or merge all frames together
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
        var objectName = options.GetObject();
        var frameName = options.GetFrame();
        var includeInvisible = options.IncludeInvisible();
        var scale = options.GetScale();
        var applyMaterials = options.ApplyMaterials();
        var groundOrigin = options.GroundOrigin();
        var ignoreTransforms = options.IgnoreTransforms();
        var voxelObjects = vox.GatherObjects(includeInvisible);
        
        ObjectSelector objectSelection = objectName == ImportOptions.MergeAll
            ? new ObjectSelector.MergeAll()
            : new ObjectSelector.ByName(objectName);

        KeyFrameSelector keyFrame =
            frameName == ImportOptions.MergeAll
                ? new KeyFrameSelector.CombinedKeyFrame()
                : new KeyFrameSelector.SpecificKeyFrames(_frameRegex.Match(frameName).Groups[1].Value.ToInt());

        var primary = objectSelection.GetObjects(voxelObjects);
        Resource resource;
        try {
            resource = MeshGenerator.Greedy(
                VoxelImporter.CombineObjects(
                    primary,
                    keyFrame.GetFrames(vox),
                    ignoreTransforms
                ),
                scale,
                groundOrigin,
                applyMaterials
            );
        } catch (Exception e) {
            GD.PushError(e.Message);
            return Error.InvalidData;
        }

        if (options.ExportRemaining()
            && objectSelection is not ObjectSelector.MergeAll
            && voxelObjects.Count > 1) {
            var root = options.OutputPath(sourceFile);
            voxelObjects.RemoveAll(o => primary.Contains(o));
            
            foreach (var secondary in voxelObjects) {
                var mesh = MeshGenerator.Greedy(
                    VoxelImporter.CombineObjects(
                        VoxUtils.ListOf(secondary),
                        keyFrame.GetFrames(vox),
                        ignoreTransforms
                    ),
                    scale,
                    groundOrigin,
                    applyMaterials
                );
                
                ResourceSaver.Save(mesh, Secondary(root, secondary.Name(), _GetSaveExtension()));
            }
        }

        return ResourceSaver.Save(resource, outputPath);
    }

    private VoxFile? Get(string path) {
        if (VoxelImporter.LoadFile(path, out var access) == Error.CantOpen) {
            return null;
        }

        return VoxelImporter.Parse(access);
    }

    private string Secondary(string path, string name, string ext) => $"{path}_{name}.{ext}";
}