using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxShapeModel(
    int modelId,
    Dictionary<string, string> attrs
) {

    public int ModelId = modelId;
    public Dictionary<string, string> Attributes = attrs;

    public override string ToString() => $"ShapeModel({ModelId}, {Attributes.Dbg()})";

    public int GetFrameIndex() {
        if (!int.TryParse(Attributes.GetValueOrDefault("_f"), out var f)) {
            f = 0;
        }

        return f;
    }
}