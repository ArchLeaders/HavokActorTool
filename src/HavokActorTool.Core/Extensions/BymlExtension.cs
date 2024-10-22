using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using BymlLibrary.Nodes.Containers.HashMap;

namespace HavokActorTool.Core.Extensions;

public static class BymlExtension
{
    public static Byml HardCopy(this Byml byml)
    {
        return byml.Value switch {
            BymlArray array => HardCopy(array),
            BymlMap map => HardCopy<BymlMap, string>(map),
            BymlHashMap32 map => HardCopy<BymlHashMap32, uint>(map),
            BymlHashMap64 map => HardCopy<BymlHashMap64, ulong>(map),
            _ => byml
        };
    }

    public static T HardCopy<T, TKey>(this T map) where T : IDictionary<TKey, Byml>, new() where TKey : notnull
    {
        T copy = new();

        foreach ((TKey key, Byml value) in map) {
            copy[key] = value;
        }

        return copy;
    }

    public static BymlArray HardCopy(this BymlArray array)
    {
        return [
            .. array.Select(byml => byml.HardCopy())
        ];
    }
}