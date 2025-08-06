using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace VoxelImporter.addons.voxel_importer;

public static class VoxUtils {

    public static List<T> ListOf<T>(params T[] items) {
        return [..items];
    }
    
    public static void Repeat(int times, Action func) {
        for (int z = 0; z < times; z++) {
            func.Invoke();
        }
    }
    
    public static List<T> EmptyList<T>() {
        return [];
    }

    public static string Dbg<T>(this List<T> list) => $"[{string.Join(", ", list)}]";

    public static string Dbg<TK, TV>(this Dictionary<TK, TV> d) where TK : notnull {
        return "{" + string.Join(", ", d) + "}";
    }

    public static string Dbg(this Array array) => array.Cast<object>().ToList().Dbg();
    
    public static bool IsApprox(this float f1, float f2, float tolerance = 0.0001f) {
        return Math.Abs(f1 - f2) < tolerance;
    }
    
    public static T Also<T>(this T any, Action<T> f) {
        f(any);
        return any;
    }
    
    public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action) {
        foreach (var e in ie) action(e);
    }

    public static bool IsNullOrBlank(this string? str) {
        return string.IsNullOrEmpty(str) || str.Trim().Length == 0;
    }
    
    public static bool IsEmpty<T>(this List<T> lst) => lst.Count == 0;

    public static void RecursiveChildren(this Node node, Action<Node> action) {
        for (var z = 0; z < node.GetChildCount(); z++) {
            action.Invoke(node.GetChild(z));
            RecursiveChildren(node.GetChild(z), action);
        }
    }
}