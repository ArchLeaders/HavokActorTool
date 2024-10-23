using System.Runtime.CompilerServices;
using BymlLibrary;

namespace HavokActorTool.Core.Common;

public sealed class BymlHashComparer : IComparer<Byml>
{
    public static readonly BymlHashComparer Instance = new();
    
    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public int Compare(Byml? x, Byml? y)
    {
        if (y is null) {
            return 1;
        }

        if (x is null) {
            return -1;
        }

        if (x.Type is not (BymlNodeType.Int or BymlNodeType.UInt32) || y.Type is not (BymlNodeType.Int or BymlNodeType.UInt32)) {
            throw new InvalidCastException(
                "The comparing inputs are not BymlNodeType.Int or BymlNodeType.UInt32");
        }
        
        uint xValue = Convert.ToUInt32(x.Value);
        uint yValue = Convert.ToUInt32(y.Value);
        
        return xValue.CompareTo(yValue);
    }
}