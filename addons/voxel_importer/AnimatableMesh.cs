using Godot;

namespace VoxelImporter.addons.voxel_importer;

[Tool]
[GlobalClass]
[Icon("res://addons/MagicaVoxel_Importer_with_Extensions/framed_mesh_instance.png")]
public partial class AnimatableMesh : MeshInstance3D {

    [Export]
    public MeshLibrary? frames {
        get => _frames;
        set => SetFrames(value);
    }

    private MeshLibrary? _frames;

    [Export]
    public int currentFrame {
        get => _currentFrame;
        set => SetCurrentFrame(value);
    }

    private int _currentFrame;

    public int frameCount { get; private set; }

    public int lastFrame => frameCount - 1;

    private void SetFrames(MeshLibrary? newFrames) {
        _frames = newFrames;

        if (frames == null || frames.GetItemList().IsEmpty()) {
            _currentFrame = 0;
            frameCount = 0;
            Mesh = null;
        } else {
            frameCount = frames.GetItemList().Length;
            SetCurrentFrame(0);
        }
    }

    private void SetCurrentFrame(int newFrame) {
        if (frames == null || newFrame < 0 || newFrame >= frameCount) {
            return;
        }

        _currentFrame = newFrame;
        Mesh = frames.GetItemMesh(newFrame);
    }

}