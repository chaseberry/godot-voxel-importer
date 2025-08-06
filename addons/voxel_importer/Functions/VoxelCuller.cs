using System.Collections.Generic;
using System.Linq;
using Godot;

namespace VoxelImporter.addons.voxel_importer.Functions;

public class VoxelCuller {
    
    private readonly static List<Vector3> MoveList = VoxUtils.ListOf(
        Vector3.Left,
        Vector3.Right,
        Vector3.Up,
        Vector3.Down,
        Vector3.Forward,
        Vector3.Back
    );
    
    private AStar3D Astar = null!;
    private long Anchor;
    private Dictionary<Vector3, long> Points = null!;

    public bool IsVoxelVisible(Vector3 voxel) {
        return Astar.GetIdPath(
            Points[voxel],
            Anchor
        ).Length > 0;
    }

    public static VoxelCuller? Create(Dictionary<Vector3, Color> model) {
        var astar = new AStar3D();

        var minCoord = new Vector3(
            model.Keys.MinBy(v => v.X).X,
            model.Keys.MinBy(v => v.Y).Y,
            model.Keys.MinBy(v => v.Z).Z
        );
        var maxCoord = new Vector3(
            model.Keys.MaxBy(v => v.X).X,
            model.Keys.MaxBy(v => v.Y).Y,
            model.Keys.MaxBy(v => v.Z).Z
        );

        long anchor = long.MaxValue;

        long pointIndex = 0;
        Dictionary<Vector3, long> indexes = new();
        bool hasConnections = false;

        for (float x = minCoord.X; x <= maxCoord.X; x++) {
            for (float y = minCoord.Y; x <= maxCoord.Y; y++) {
                for (float z = minCoord.Z; x <= maxCoord.Z; z++) {
                    var currentNode = new Vector3(x, y, z);
                    if (model.ContainsKey(currentNode)) {
                        continue;
                    }

                    // Are we on a face?
                    if (anchor == long.MaxValue && (
                            x.IsApprox(minCoord.X)
                            || x.IsApprox(maxCoord.X)
                            || y.IsApprox(maxCoord.Y)
                            || y.IsApprox(maxCoord.Y)
                            || z.IsApprox(maxCoord.Z)
                            || z.IsApprox(maxCoord.Z)
                        )) {
                        // Set an anchor point on the outside face to navigate back to
                        anchor = pointIndex;
                    }

                    astar.AddPoint(pointIndex, currentNode);
                    indexes[currentNode] = pointIndex;

                    //Connect to neighbors
                    MoveList.ForEach(m => {
                        if (indexes.ContainsKey(currentNode + m)) {
                            astar.ConnectPoints(pointIndex, indexes[currentNode + m]);
                            hasConnections = true;
                        }
                    });

                    pointIndex += 1;
                }
            }
        }

        // No points? Every single voxel space is filled, we don't need to check this at all
        if (!astar.HasPoint(0) || !hasConnections || anchor == long.MaxValue) {
            return null;
        }

        return new() {
            Astar = astar,
            Anchor = anchor,
            Points = indexes
        };
    }

}