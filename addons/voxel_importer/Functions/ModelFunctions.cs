using System.Collections.Generic;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Chunks;
using VoxelImporter.addons.voxel_importer.Data;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Functions;

public static class ModelFunctions {

    // TODO I wonder if moving this from an int space to a float space is a good idea
    // Centering a even X even object is fine, but odd sized ranges end up split
    // This causes issues with multi-object models. if one object is 4x4 and another is 4x3, it becomes offset
    // Maybe use an option?
    
    public static VoxelModel Center(VoxelModel model) {
        var xOffset = model.X / 2;
        var yOffset = model.Y / 2;
        var zOffset = model.Z / 2;

        return new(
            x: model.X,
            y: model.Y,
            z: model.Z,
            voxels: model.Voxels.Select(s => new VoxelInstance(s.X - xOffset, s.Y - yOffset, s.Z - zOffset, s.Color))
                .ToList()
        );
    }

    public static VoxelModel MoveToGlobalSpace(VoxelModel model, bool ignoreTransforms, List<VoxFrame> chain) {
        var bld = new GlobalSpaceBuilder(model);
        chain.ForEach(c => {
                bld.Rotate(c.GetRotation());
                if (!ignoreTransforms) {
                    bld.Transform(c.GetTranslation());
                }
            }
        );
        return bld.Build();
    }

}