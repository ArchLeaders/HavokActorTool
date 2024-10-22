using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using CommunityToolkit.HighPerformance.Buffers;
using Revrs.Extensions;

namespace HavokActorTool.Core;

public static class HkActorCache
{
    private static readonly uint[] _instSizeValues;
    private static readonly string[] _actors;

    static HkActorCache()
    {
        using Stream stream = typeof(HkActorCache).Assembly.GetManifestResourceStream("HavokActorTool.Core.Resources.HkActorCache.bin")!;
        int count = stream.Read<int>();
        
        _instSizeValues = new uint[count];
        _actors = new string[count];
        
        for (int i = 0; i < count; i++) {
            _instSizeValues[i] = stream.Read<uint>();
        }
        
        for (int i = 0; i < count; i++) {
            int actorNameLength = stream.Read<byte>();
            using SpanOwner<byte> nameUtf8 = SpanOwner<byte>.Allocate(actorNameLength); 
            int read = stream.Read(nameUtf8.Span);
            Debug.Assert(read == actorNameLength);
            int stringLength = Encoding.UTF8.GetCharCount(nameUtf8.Span);
            char[] actorChars = new char[stringLength];
            Encoding.UTF8.GetChars(nameUtf8.Span, actorChars);
            _actors[i] = new string(actorChars);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    public static string GetNearestActor(long instSize)
    {
        ReadOnlySpan<uint> values = _instSizeValues.AsSpan();

        int mid = 0, min = 0, max = values.Length - 1;
        
        while (min <= max) {
            mid = (min + max) / 2;

            if (instSize > values[mid]) {
                max = mid - 1;
            }
            else {
                min = mid + 1;
            }
        }
        
        return _actors[mid];
    }
}