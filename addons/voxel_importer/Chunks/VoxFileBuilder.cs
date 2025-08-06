using System;
using System.Collections.Generic;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Data;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxFileBuilder(int version) {

    private VoxFile vox = new(version);

    private VoxSizeChunk? _sizeChunk = null;
    private List<VoxMaterialChunk> _materials = new();

    private List<(VoxSizeChunk, VoxXyziChunk)> Models = new();
    private Dictionary<int, VoxLayerChunk> Layers = new();
    private Dictionary<int, VoxChunk> Nodes = new();

    public void ConsumeChunk(VoxChunk chunk) {
        switch (chunk) {
            case VoxSizeChunk vsc:
                if (_sizeChunk != null) {
                    throw new ArgumentException("Cannot read a SIZE chunk while waiting for a XYZI chunk.");
                }

                _sizeChunk = vsc;
                break;
            case VoxXyziChunk vxc:
                if (_sizeChunk == null) {
                    throw new ArgumentException("Cannot read a XYZI chunk without a SIZE chunk.");
                }

                Models.Add((_sizeChunk, vxc));
                _sizeChunk = null;
                break;
            case VoxRgbaChunk vrc:
                vox.Colors = vrc.Colors.Select(c => new VoxelColor(c)).ToList();
                break;

            case VoxLayerChunk vlc:
                Layers[vlc.LayerId] = vlc;
                break;

            case VoxShapeChunk vsc:
                Nodes[vsc.Id] = vsc;
                break;

            case VoxGroupChunk vgc:
                Nodes[vgc.Id] = vgc;
                break;

            case VoxTransformChunk vtc:
                Nodes[vtc.Id] = vtc;
                break;
            case VoxMaterialChunk vmc:
                _materials.Add(vmc);
                break;
        }
    }

    public VoxFile Build() {
        // inject the root layer
        Layers[-1] = new(-1, new(), -1);
        vox.Models = Models.Select(m => new VoxelModel(m.Item1, m.Item2, vox.Colors)).ToList();

        if (Nodes.Count > 0) {
            vox.Node = BuildNode(Nodes[0]);
        }

        foreach (var mat in _materials) {
            // MaterialId is 1 indexed >.>
            vox.Colors[mat.MaterialId - 1].Material = mat;
        }

        return vox;
    }

    private VoxelNode BuildNode(VoxChunk c) {
        return c switch {
            VoxTransformChunk vtc => new VoxelTransformNode(
                id: vtc.Id,
                name: vtc.GetName(),
                hidden: vtc.IsHidden(),
                child: BuildNode(Nodes[vtc.ChildId]),
                layer: new(Layers[vtc.LayerId]),
                frames: vtc.Frames
            ),
            VoxGroupChunk vgc => new VoxelGroupNode(
                id: vgc.Id,
                children: vgc.Children.Select(c => BuildNode(Nodes[c.NodeId])).ToList()
            ),
            VoxShapeChunk vsc => new VoxelObject(
                id: vsc.Id,
                frames: vsc.Models.ToDictionary(s => s.GetFrameIndex(), s => vox.Models[s.ModelId])
            ),
            _ => throw new ArgumentException("Unsupported Voxel Chunk")
        };
    }

}