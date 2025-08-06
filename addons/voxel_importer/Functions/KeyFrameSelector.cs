using System.Collections.Generic;
using VoxelImporter.addons.voxel_importer.Data;

namespace VoxelImporter.addons.voxel_importer.Functions;

public abstract class KeyFrameSelector {

    public abstract List<int> GetFrames(VoxFile file);

    public class FirstKeyFrame() : KeyFrameSelector {

        public override List<int> GetFrames(VoxFile file) => VoxUtils.ListOf(0);

    }

    public class CombinedKeyFrame() : KeyFrameSelector {

        public override List<int> GetFrames(VoxFile file) => file.GetFrameIndexes();

    }

    public class SpecificKeyFrames(List<int> frames) : KeyFrameSelector {

        public override List<int> GetFrames(VoxFile file) => frames;

    }

}