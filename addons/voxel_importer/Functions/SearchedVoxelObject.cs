using System;
using System.Collections.Generic;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Chunks;
using VoxelImporter.addons.voxel_importer.Data;

namespace VoxelImporter.addons.voxel_importer.Functions;

public class SearchedVoxelObject {

    public required VoxelObject VoxelObject;
    public required List<VoxelNode> Chain;

    public VoxelModel GetModelAtFrame(int frame) => VoxelObject.GetModelAtFrame(frame);

    public int GetLayerIndex() => Chain.OfType<VoxelTransformNode>().LastOrDefault()?.Layer.LayerId ?? 0;

    public string Name() => Chain.OfType<VoxelTransformNode>().LastOrDefault()?.Name ?? $"Object {VoxelObject.Id}";
    
    public static Comparison<SearchedVoxelObject> LayerSorter => (a, b) => {
        var aLayer = a.GetLayerIndex();
        var bLayer = b.GetLayerIndex();
        
        return aLayer.CompareTo(bLayer);
    };

}