using System;
using System.Collections.Generic;
using System.Linq;
using VoxelImporter.addons.voxel_importer.Chunks;
using VoxelImporter.addons.voxel_importer.Constants;
using VoxelImporter.addons.voxel_importer.Data;
using VoxelImporter.addons.voxel_importer.Functions;
using Godot;
using VoxelImporter.addons.voxel_importer.Importers;

namespace VoxelImporter.addons.voxel_importer;

public static class VoxelImporter {

    // TODO
    // 2. Verify layer ordering

    // Modes
    // 0: combined single object - first key frame or combined or [list of key frames]?
    // 1: combined mesh library - 1 instance per frame index, with all meshes @ that frame combined
    // 2: separate objects - first key frame or combined or [list of key frames]?
    // 3: separate mesh libraries - 1 mesh lib per object per frame index
    // 4: Scene with seperate meshs

    // combines - requires objects to merge in a global space
    // separate - each object can be local

    public static Error LoadFile(string path, out FileAccess access) {
        access = FileAccess.Open(path, FileAccess.ModeFlags.Read);
        return access == null ? Error.CantOpen : Error.Ok;
    }

    public static VoxFile Parse(FileAccess access) => new VoxFileParser(access).Parse();

    public static Resource Import(
        int type,
        FileAccess file,
        Godot.Collections.Dictionary options
    ) {
        var vox = Parse(file);
        GD.Print($"Importing {file.GetPath()} with {options}");

        KeyFrameSelector keySelector =
            options.MergeFrames() ? new KeyFrameSelector.CombinedKeyFrame() : new KeyFrameSelector.FirstKeyFrame();

        return type switch {
            0 => MeshGenerator.Greedy(
                CombinedSingleObject(vox, keySelector, options.IncludeInvisible(), options.IgnoreTransforms()),
                options.GetScale(),
                options.GroundOrigin(),
                options.ApplyMaterials()
            ),
            1 => BuildMeshLibrary(
                CombinedMeshLibrary(vox, options.IncludeInvisible(), options.IgnoreTransforms()),
                options.GetScale(),
                options.GroundOrigin(),
                options.ApplyMaterials(),
                options.CollisionGenerationType()
            ),
            2 => BuildMeshLibrary(
                SeparateMeshes(vox, keySelector, options.IncludeInvisible(), options.IgnoreTransforms()),
                options.GetScale(),
                options.GroundOrigin(),
                options.ApplyMaterials(),
                options.CollisionGenerationType()
            ),
            _ => throw new ArgumentException($"Invalid import type {type}")
        };
    }

    public static MeshLibrary BuildMeshLibrary(
        List<VoxelModel> models,
        float scale,
        bool groundOrigin,
        bool applyMaterials,
        CollisionGenerationType collisionGenerationType
    ) {
        var lib = new MeshLibrary();

        foreach (var mesh in models) {
            var id = lib.GetLastUnusedItemId();
            lib.CreateItem(id);

            var meshInstance = MeshGenerator.Greedy(mesh, scale, groundOrigin, applyMaterials);

            if (mesh.Name != null) {
                lib.SetItemName(id, mesh.Name);
            }

            var collisionShape = ShapeForMesh(meshInstance, collisionGenerationType);
            if (collisionShape != null) {
                lib.SetItemShapes(id, [collisionShape, Transform3D.Identity]);

                // I bet if we have groundOrigin, the 2nd array element needs to be new Vector3(0, y/2, 0);
            }

            lib.SetItemMesh(id, meshInstance);
        }

        return lib;
    }

    public static VoxelModel CombineObjects(
        List<SearchedVoxelObject> objects,
        KeyFrameSelector keySelector,
        bool ignoreTransforms
    ) {
        objects.Sort(SearchedVoxelObject.LayerSorter);
        var combiner = new ModelCombiner();

        objects.ForEach(obj =>
            combiner.AddModel(
                CombineObject(obj, keySelector.GetFrames(vox), ignoreTransforms)
            )
        );

        return combiner.GetResult();
    }

    private static VoxelModel CombinedSingleObject(
        VoxFile vox,
        KeyFrameSelector keySelector,
        bool includeHidden,
        bool ignoreTransforms
    ) {
        var frames = keySelector.GetFrames(vox);

        var allObjects = vox.GatherObjects(includeHidden);
        allObjects.Sort(SearchedVoxelObject.LayerSorter); // Sort by layers, so the last layers get added first
        var combiner = new ModelCombiner();

        allObjects.ForEach(obj =>
            combiner.AddModel(
                CombineObject(obj, frames, ignoreTransforms)
            )
        );

        return combiner.GetResult();
    }

    private static List<VoxelModel> CombinedMeshLibrary(
        VoxFile vox,
        bool includeHidden,
        bool ignoreTransforms
    ) {
        var frames = vox.GetFrameIndexes();

        var allObjects = vox.GatherObjects(includeHidden);
        allObjects.Sort(SearchedVoxelObject.LayerSorter);

        var results = new List<VoxelModel>();

        foreach (var frameIndex in frames) {
            var combiner = new ModelCombiner();
            allObjects.ForEach(o => {
                    var model = o.GetModelAtFrame(frameIndex);

                    var chain = o.Chain.OfType<VoxelTransformNode>()
                        .Select(t => t.GetFrameAtIndex(frameIndex))
                        .OfType<VoxFrame>()
                        .ToList();

                    combiner.AddModel(
                        ModelFunctions.MoveToGlobalSpace(
                            ModelFunctions.Center(model),
                            ignoreTransforms,
                            chain
                        )
                    );
                }
            );

            results.Add(combiner.GetResult());
        }

        return results;
    }

    private static List<VoxelModel> SeparateMeshes(
        VoxFile vox,
        KeyFrameSelector keySelector,
        bool includeHidden,
        bool ignoreTransforms
    ) {
        var frames = keySelector.GetFrames(vox);

        var allObjects = vox.GatherObjects(includeHidden);
        var results = new List<VoxelModel>();
        allObjects.Sort(SearchedVoxelObject.LayerSorter);

        allObjects.ForEach(obj => {
                var combiner = new ModelCombiner();
                var name = obj.Chain.OfType<VoxelTransformNode>().Last().Name;
                foreach (var frameIndex in frames) {
                    var model = obj.VoxelObject.GetModelAtFrame(frameIndex);

                    var chain = obj.Chain.OfType<VoxelTransformNode>()
                        .Select(t => t.GetFrameAtIndex(frameIndex))
                        .OfType<VoxFrame>()
                        .ToList();

                    combiner.AddModel(
                        ModelFunctions.MoveToGlobalSpace(
                            ModelFunctions.Center(model),
                            ignoreTransforms,
                            chain
                        )
                    );
                }

                var m = combiner.GetResult();
                m.Name = name;
                results.Add(m);
            }
        );

        return results;
    }

    public static List<(string?, List<VoxelModel>)> SeparateMeshLibraries(
        VoxFile vox,
        bool includeHidden,
        bool ignoreTransforms
    ) {
        var allObjects = vox.GatherObjects(includeHidden);
        var results = new List<(string?, List<VoxelModel>)>();

        allObjects.ForEach(obj => {
                var frames = new List<VoxelModel>();
                var name = obj.Chain.OfType<VoxelTransformNode>().Last().Name;
                foreach (var frameIndex in obj.VoxelObject.Frames.Keys) {
                    var model = obj.VoxelObject.GetModelAtFrame(frameIndex);
                    var chain = obj.Chain.OfType<VoxelTransformNode>()
                        .Select(t => t.GetFrameAtIndex(frameIndex))
                        .OfType<VoxFrame>()
                        .ToList();

                    frames.Add(ModelFunctions.MoveToGlobalSpace(ModelFunctions.Center(model), ignoreTransforms, chain));
                }


                results.Add((name, frames));
            }
        );

        return results;
    }

    // First Frame of each object
    // Merged Frames of each Object
    // Smart Detection of each object

    public static PackedScene SingleScene(
        string sceneName,
        VoxFile vox,
        KeyFrameSelector? selector,
        float scale,
        bool includeHidden,
        bool groundOrigin,
        bool ignoreTransforms,
        bool applyMaterials,
        CollisionGenerationType collisionGenerationType
    ) {
        var baseNode = vox.Node;
        var frames = selector?.GetFrames(vox);
        Node root;

        if (baseNode == null) {
            // if baseNode == null, we only have a model collection, so there is no frame/transform information
            // We can interp that as separate objects or as one object
            var objs = vox.GatherObjects(includeHidden);
            var combiner = new ModelCombiner();
            foreach (var z in frames ?? VoxUtils.ListOf(0)) {
                foreach (var o in objs) {
                    var model = o.GetModelAtFrame(z);
                    combiner.AddModel(ModelFunctions.Center(model));
                }
            }

            // Already centered because of the combining above
            var mesh = MeshGenerator.Greedy(combiner.GetResult(), scale, groundOrigin, applyMaterials);
            root = new MeshInstance3D().Also(m => m.Mesh = mesh);
        } else {
            var temp = new Node3D();
            BuildTree(
                temp,
                baseNode,
                frames,
                scale,
                includeHidden,
                groundOrigin,
                ignoreTransforms,
                applyMaterials,
                collisionGenerationType
            );
            root = temp.GetChild(0);
        }

        root.Name = sceneName;

        root.RecursiveChildren(c => c.SetOwner(root));
        var s = new PackedScene();
        s.Pack(root);
        return s;
    }

    private static void BuildTree(
        Node3D gdNode,
        VoxelNode voxNode,
        List<int>? frames,
        float scale,
        bool includeHidden,
        bool groundOrigin,
        bool ignoreTransforms,
        bool applyMaterials,
        CollisionGenerationType collisionGenerationType
    ) {
        switch (voxNode) {
            case VoxelTransformNode transform:
                if (!includeHidden && transform.IsHidden()) {
                    return;
                }

                var n = new Node3D();
                var t = transform.GetFrameAtIndex(0)!.GetTranslation() * scale;
                var rotation = transform.GetFrameAtIndex(0)!.GetRotation();

                n.Position = new(t.X, t.Z, t.Y); // swap Z and Y axis because magica voxel formating
                n.Basis = rotation;
                n.Name = transform.Name ?? $"Transform-{transform.Id}";
                BuildTree(
                    n,
                    transform.Child,
                    frames,
                    scale,
                    includeHidden,
                    groundOrigin,
                    ignoreTransforms,
                    applyMaterials,
                    collisionGenerationType
                );
                gdNode.AddChild(n);

                // GD.Print($"{transform.Name} {t} {rotation}");
                // TODO if transform.child is a Shape, make a meshInstance instead of node3d?
                // Maybe not? Might be useful having each mesh with a seperate parent, to then attach stuff to?
                break;
            case VoxelGroupNode group:
                group.Children.ForEach(c =>
                    BuildTree(
                        gdNode,
                        c,
                        frames,
                        scale,
                        includeHidden,
                        groundOrigin,
                        ignoreTransforms,
                        applyMaterials,
                        collisionGenerationType
                    )
                );
                break;
            case VoxelObject shape:
                Node meshNode;
                if (frames != null) {
                    meshNode = new MeshInstance3D().Also(mi => {
                            mi.Mesh = MeshGenerator.Greedy(
                                CombineObject(
                                    new() {
                                        Chain = VoxUtils.EmptyList<VoxelNode>(),
                                        VoxelObject = shape
                                    },
                                    frames,
                                    ignoreTransforms
                                ),
                                scale,
                                groundOrigin,
                                applyMaterials
                            );
                        }
                    );
                } else {
                    var r = SmartProcess(shape, scale, groundOrigin, applyMaterials, collisionGenerationType);
                    meshNode = r switch {
                        Mesh m => new MeshInstance3D().Also(mi => mi.Mesh = m),
                        MeshLibrary ml => new AnimatableMesh().Also(am => am.frames = ml),
                        _ => throw new ArgumentException(),
                    };
                }

                meshNode.Name = $"{gdNode.Name}-Mesh";
                if (meshNode is MeshInstance3D mi3d && collisionGenerationType != CollisionGenerationType.None) {
                    var staticBody = new StaticBody3D();
                    var shapeNode = new CollisionShape3D {
                        Shape = ShapeForMesh(mi3d.Mesh, collisionGenerationType),
                    };

                    staticBody.AddChild(shapeNode);
                    meshNode.AddChild(staticBody);
                }

                // TODO is staticBody a child of the meshNode? the gdNode?
                // Is the mesh a child of the static body?
                gdNode.AddChild(meshNode);

                break;
        }
    }

    private static Resource SmartProcess(
        VoxelObject obj,
        float scale,
        bool groundOrigin,
        bool applyMaterials,
        CollisionGenerationType collisionGenerationType
    ) {
        if (obj.Frames.Count == 1) {
            return BasicMesh(obj.Frames.First().Value, scale, groundOrigin, applyMaterials);
        }

        return BuildMeshLibrary(
            obj.Frames.Select(s => ModelFunctions.Center(s.Value)).ToList(),
            scale,
            groundOrigin,
            applyMaterials,
            collisionGenerationType
        );
    }

    private static Mesh BasicMesh(VoxelModel model, float scale, bool groundOrigin, bool applyMaterials) =>
        MeshGenerator.Greedy(
            ModelFunctions.Center(model),
            scale,
            groundOrigin,
            applyMaterials
        );

    private static VoxelModel CombineObject(SearchedVoxelObject obj, List<int> frames, bool ignoreTransforms) {
        ModelCombiner combiner = new();
        foreach (var frameIndex in frames) {
            var model = obj.VoxelObject.GetModelAtFrame(frameIndex);

            var chain = obj.Chain.OfType<VoxelTransformNode>()
                .Select(t => t.GetFrameAtIndex(frameIndex))
                .OfType<VoxFrame>()
                .ToList();

            // 1. move the model into the center 
            // 2. Move the modal into global space by its transforms
            // 3. add the model to the combination layer

            combiner.AddModel(ModelFunctions.MoveToGlobalSpace(ModelFunctions.Center(model), ignoreTransforms, chain));
        }

        return combiner.GetResult();
    }

    private static Shape3D? ShapeForMesh(Mesh mesh, CollisionGenerationType type) {
        return type switch {
            CollisionGenerationType.Box => new BoxShape3D {
                Size = mesh.GetAabb().Size
            },
            CollisionGenerationType.ConcavePolygon => mesh.CreateTrimeshShape(),
            CollisionGenerationType.SimpleConvexPolygon => mesh.CreateConvexShape(true, true),
            CollisionGenerationType.ComplexConvexPolygon => mesh.CreateConvexShape(),
            _ => null,
        };
    }

    private static Transform3D ShapeOffset(Mesh mesh, bool groundOrigin) {
        return groundOrigin ? new(Basis.Identity, new() { Y = mesh.GetAabb().Size.Y / 2f }) : Transform3D.Identity;
    }

}