using System.Collections.Generic;

namespace VoxelImporter.addons.voxel_importer.Chunks;

public class VoxGroupChunk(
    int id,
    Dictionary<string, string> attrs,
    List<VoxGroupChild> children
) : VoxChunk {

    public int Id = id;
    public Dictionary<string, string> Attributes = attrs;
    public List<VoxGroupChild> Children = children;

    public override VoxChunkType GetVoxChunkType() => VoxChunkType.Ngrp;

    public override string ToString() => $"nGRP({Id}, {Attributes.Dbg()}, {Children.Dbg()})";

}