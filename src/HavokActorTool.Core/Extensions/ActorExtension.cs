using Nintendo.Aamp;
using SarcLibrary;

namespace HavokActorTool.Core.Extensions;

public static class ActorExtension
{
    public static AampFile GetNested(this Sarc pack, string nestedFileName)
    {
        if (!pack.TryGetValue(nestedFileName, out ArraySegment<byte> actorLinkBuffer)) {
            throw new InvalidOperationException(
                $"Failed to locate '{nestedFileName}'.");;
        }
        
        using MemoryStream actorLinkMemoryStream = new(
            actorLinkBuffer.Array!,
            writable: false, index: 0, count: actorLinkBuffer.Count
        );
        
        return AampFile.FromBinary(actorLinkMemoryStream);
    }
}