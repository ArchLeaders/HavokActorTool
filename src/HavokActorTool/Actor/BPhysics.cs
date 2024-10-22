using CsYaz0;
using Nintendo.Aamp;

namespace HavokActorTool.Actor
{
    public class BPhysics
    {
        public static string MethodHeader { get; set; } = $"[{nameof(BPhysics)}]";
        public static Task Create(ref Dictionary<string, byte[]> dict, string key, string actorName, string modelName)
        {
            // Notify interface
            Print($"{MethodHeader} Modify binary physics (BPHYSICS) . . .");

            // Parse bphysics
            AampFile bphysics = new(Yaz0.Decompress(new Resource("Resources.HKX2.sbphysics").Data));

            // Update physics file
            bphysics.RootNode.ChildParams[0].ChildParams[0].ChildParams[0].ParamObjects[0].ParamEntries[3].Value = new StringEntry($"{modelName}/{actorName}.hkrb");
            dict[key] = bphysics.ToBinary();

            // Return completed
            return Task.CompletedTask;
        }
    }
}
