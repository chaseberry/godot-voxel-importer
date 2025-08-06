using System.Collections.Generic;
using VoxelImporter.addons.voxel_importer.Data;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Functions;

public class GlobalSpaceBuilder(VoxelModel model) {

    private List<VoxelInstance> Voxels = model.Voxels;

    public void Transform(Vector3 transform) {
        Voxels.ForEach(voxel => {
            voxel.X += (int)transform.X;
            voxel.Y += (int)transform.Y;
            voxel.Z += (int)transform.Z;
        });
    }

    public void Rotate(Basis basis) {
        Voxels.ForEach(voxel => {
            var rotated = (basis * voxel.ToV3()).Floor();

            voxel.X = (int)rotated.X;
            voxel.Y = (int)rotated.Y;
            voxel.Z = (int)rotated.Z;
        });
    }

    public VoxelModel Build() {
        return new(
            x: model.X,
            y: model.Y,
            z: model.Z,
            voxels: Voxels
        );
    }

}