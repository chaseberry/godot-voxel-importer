using System.Collections.Generic;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Data;

namespace VoxelImporter.addons.voxel_importer.Functions;

public abstract class ObjectSelector {

    public abstract List<SearchedVoxelObject> GetObjects(List<SearchedVoxelObject> objects);

    public class MergeAll() : ObjectSelector {

        public override List<SearchedVoxelObject> GetObjects(List<SearchedVoxelObject> objects) => objects;

    }

    public class ByName(string name) : ObjectSelector {

        public override List<SearchedVoxelObject> GetObjects(List<SearchedVoxelObject> objects) =>
            objects.Where(o => o.Name() == name).ToList();

    }

}