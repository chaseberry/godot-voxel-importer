namespace VoxelImporter.addons.voxel_importer.Data;

public enum VoxelMaterialType {

    Diffuse,
    Emit,
    Blend,
    Metal,
    Glass,
    Cloud

}

public static class VoxMaterialTypeFunctions {

    public static VoxelMaterialType From(string? str) {
        return str switch {
            "_emit" => VoxelMaterialType.Emit,
            "_blend" => VoxelMaterialType.Blend,
            "_media" => VoxelMaterialType.Cloud, // ???????
            "_glass" => VoxelMaterialType.Glass,
            "_metal" => VoxelMaterialType.Metal,
            _ => VoxelMaterialType.Diffuse, // default case
        };
    }

}