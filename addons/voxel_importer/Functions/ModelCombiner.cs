using System.Collections.Generic;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Data;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Functions;

public class ModelCombiner {

    // The current gd plugin follows the basic flow below
    // 1. Add model to global space, centering it (middle of the model is 0,0,0)
    // 2. Add models from normalized children
    // 3. combines layers data
    // 4. normalize model by moving things based on transforms

    // my plan
    // Create a "Global Space" for all models that need combining
    // Apply transforms down the chain to models
    // Add models to global space, in layer order
    // Return resultent model

    // TODO investigate this and see if this is correct behavior
    // See A, B
    private VoxelModel result = new(0, 0, 0, VoxUtils.EmptyList<VoxelInstance>());

    // B: Assumed global space
    public void AddModel(VoxelModel model) { result = Combine(result, model); }

    private VoxelModel Combine(VoxelModel model1, VoxelModel model2) {
        Dictionary<Vector3, VoxelColor> voxels = new();
        // A: model 1 overwrites model 2
        model2.Voxels.ForEach(v => voxels[v.ToV3()] = v.Color);
        model1.Voxels.ForEach(v => voxels[v.ToV3()] = v.Color);
        var outputData = voxels.Select(s => new VoxelInstance(s.Key, s.Value)).ToList();
        return new(
            x: outputData.MaxBy(v => v.X)!.X - outputData.MinBy(v => v.X)!.X,
            y: outputData.MaxBy(v => v.Y)!.Y - outputData.MinBy(v => v.Y)!.Y,
            z: outputData.MaxBy(v => v.Z)!.Z - outputData.MinBy(v => v.Z)!.Z,
            voxels: outputData
        );
    }

    public VoxelModel GetResult() => result;

}