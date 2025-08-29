using Godot;
using Godot.Collections;

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
    
}