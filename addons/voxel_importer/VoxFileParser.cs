using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VoxelImporter.addons.voxel_importer.Chunks;
using VoxelImporter.addons.voxel_importer.Data;
using Godot;
using FileAccess = Godot.FileAccess;

namespace VoxelImporter.addons.voxel_importer;

public class VoxFileParser {

    // Replace with FileAccess?
    private FileAccess Stream;

    public VoxFileParser(FileAccess file) {
        Stream = file;
    }

    public void Close() { Stream.Close(); }

    public VoxFile Parse() {
        var id = ReadString(4);
        var version = ReadInt32();

        if (id != "VOX ") {
            throw new ArgumentException("File is not a '.vox' file.");
        }

        var blder = new VoxFileBuilder(version);

        do {
            var c = ReadChunk();
            if (c != null) {
                blder.ConsumeChunk(c);
            }

        } while (Stream.GetPosition() < Stream.GetLength());

        Close();

        return blder.Build();
    }

    private VoxChunk? ReadChunk() {
        var id = ReadString(4);
        var size = ReadInt32();
        _ = ReadInt32(); // Child Count, always 0

        VoxChunk? chunk = id switch {
            "MAIN" => ParseMain(),
            "SIZE" => ParseSize(),
            "XYZI" => ParseXyzi(),
            "RGBA" => ParseRgba(),
            "nTRN" => ParseTransform(),
            "nGRP" => ParseGroup(),
            "nSHP" => ParseShape(),
            "MATL" => ParseMaterial(),
            "LAYR" => ParseLayer(),
            _ => ParseUnknown(size)
        };

        return chunk;
    }

    private VoxMainChunk ParseMain() => new();

    private VoxSizeChunk ParseSize() {
        return new(
            x: ReadInt32(),
            y: ReadInt32(),
            z: ReadInt32()
        );
    }

    private VoxXyziChunk ParseXyzi() {
        var data = new List<VoxelData>();
        VoxUtils.Repeat(ReadInt32(), () => {
            data.Add(
                new(
                    x: ReadByte(),
                    y: ReadByte(),
                    z: ReadByte(),
                    colorIndex: ReadByte() - 1
                )
            );
        });

        return new(data);
    }

    private VoxRgbaChunk ParseRgba() {
        var colors = new List<Color>();
        VoxUtils.Repeat(256, () => {
            var r = ReadByte();
            var g = ReadByte();
            var b = ReadByte();
            var a = ReadByte();
            colors.Add(Color.Color8((byte)r, (byte)g, (byte)b, (byte)a));
        });

        return new(colors);
    }

    private VoxTransformChunk ParseTransform() {
        return new(
            id: ReadInt32(),
            attrs: ReadDict(),
            childId: ReadInt32(),
            reservedId: ReadInt32(),
            layerId: ReadInt32(),
            frames: Collect(ReadInt32(), () => new VoxFrame(ReadDict()))
        );
    }

    private VoxGroupChunk ParseGroup() {
        return new(
            id: ReadInt32(),
            attrs: ReadDict(),
            children: Collect(ReadInt32(), () => new VoxGroupChild(ReadInt32()))
        );
    }

    private VoxShapeChunk ParseShape() {
        return new(
            id: ReadInt32(),
            attrs: ReadDict(),
            models: Collect(ReadInt32(), () => new VoxShapeModel(ReadInt32(), ReadDict()))
        );
    }

    private VoxMaterialChunk ParseMaterial() {
        return new(
            materialId: ReadInt32(),
            attrs: ReadDict()
        );
    }

    private VoxLayerChunk ParseLayer() {
        return new(
            layerId: ReadInt32(),
            attrs: ReadDict(),
            reservedId: ReadInt32()
        );
    }

    private VoxChunk? ParseUnknown(int remaining) {
        ReadBytes(remaining);
        return null;
    }

    private string ReadString(int length) {
        var (data, read) = ReadBytes(length);
        return Encoding.ASCII.GetString(data, 0, read);
    }

    private int ReadInt32() {
        var (data, _) = ReadBytes(4);

        return BitConverter.ToInt32(data, 0);
    }

    private (byte[], int) ReadBytes(int length) {
        var read = Stream.GetBuffer(length);

        return (read, read.Length);
    }

    private int ReadByte() { return Stream.Get8(); }

    private Dictionary<string, string> ReadDict() {
        var d = new Dictionary<string, string>();
        var entries = ReadInt32();

        VoxUtils.Repeat(entries, () => {
            var key = ReadString(ReadInt32());
            var value = ReadString(ReadInt32());
            d[key] = value;
        });

        return d;
    }

    private List<T> Collect<T>(int number, Func<T> f) {
        var items = new List<T>();

        VoxUtils.Repeat(number, () => items.Add(f()));

        return items;
    }

}