using System.Collections.Generic;
using VoxelImporter.addons.voxel_importer.Data;
using VoxelImporter.addons.voxel_importer.Functions;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxMaterialChunk(
    int materialId,
    Dictionary<string, string> attrs
) : VoxChunk {

    public int MaterialId = materialId;
    public Dictionary<string, string> Attributes = attrs;

    public override VoxChunkType GetVoxChunkType() => VoxChunkType.Matl;

    public override string ToString() => $"MATL({MaterialId}, {Attributes.Dbg()})";

    public VoxelMaterialType MaterialType() => VoxMaterialTypeFunctions.From(Attributes.GetValueOrDefault("_type"));

    private float Value(string key) {
        return float.TryParse((Attributes.GetValueOrDefault(key) ?? "0"), out float f) ? f : 0f;
    }

    public float Alpha() => Value("_alpha");
    public float Transparency() => Value("_trans");
    public float Roughness() => Value("_rough");
    public float Ior() => Value("_ior");
    public float Metalic() => Value("_metal");
    public float IndexOfRefraction() => Value("_ior");
    public float RefractiveIndex() => Value("_ri");
    public float Density() => Value("_d");
    public float Phase() => Value("_g");
    public float Specular() => Value("_sp");
    public float Emission() => Value("_emit");
    public float Power() => Value("_flux");
    public float Ldr() => Value("_ldr");

    private Material? _material = null;

    public Material GodotMaterial(Color color) => _material ??= ToGodotMaterial(color);

    private Material ToGodotMaterial(Color color) {
        var material = new StandardMaterial3D();
        material.VertexColorIsSrgb = true;
        material.VertexColorUseAsAlbedo = true;

        switch (MaterialType()) {
            case VoxelMaterialType.Diffuse:
                return MeshGenerator.DefaultMaterial;
            case VoxelMaterialType.Emit:
                material.Emission = color;
                material.EmissionEnabled = true;
                material.EmissionEnergyMultiplier = Power();

                // Not sure what to use Emission and Ldr for
                break;
            case VoxelMaterialType.Blend:
                // rough, ior, specular, metalic, transperency, density, phase
                material.Metallic = Metalic();
                material.MetallicSpecular = Specular();
                material.Roughness = Roughness();
                material.Transparency = Transparency() <= 0
                    ? BaseMaterial3D.TransparencyEnum.Disabled
                    : BaseMaterial3D.TransparencyEnum.Alpha;
                material.SetMeta("alpha", Transparency());

                // No idea where to put ior, density, phase
                break;
            case VoxelMaterialType.Metal:
                // rough, ior, specilar, metalic
                material.Metallic = Metalic();
                material.MetallicSpecular = Specular();
                material.Roughness = Roughness();

                break;
            case VoxelMaterialType.Glass:
                // rough, ior, transperency, density, phase
                material.Roughness = Roughness();
                material.Transparency = Transparency() <= 0
                    ? BaseMaterial3D.TransparencyEnum.Disabled
                    : BaseMaterial3D.TransparencyEnum.Alpha;
                material.SetMeta("alpha", Transparency());
                break;
            case VoxelMaterialType.Cloud:
                // density, phase
                break;
        }

        return material;
    }

}