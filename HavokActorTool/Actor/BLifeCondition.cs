using CsYaz0;
using Nintendo.Aamp;

namespace HavokActorTool.Actor
{
    public class BLifeCondition
    {
        public static string MethodHeader { get; set; } = $"[{nameof(BLifeCondition)}]";
        public static Task Create(ref Dictionary<string, byte[]> dict, string key, string _, float lifeCondition)
        {
            // Notify interface
            Print($"{MethodHeader} Modify binary life condition (BLIFECONDITION) . . .");

            AampFile blifecondition = new(Yaz0.Decompress(new Resource("Resources.BLifeCondition.aamp").Data));
            blifecondition.RootNode.ParamObjects[0].ParamEntries[0].Value = lifeCondition;
            dict[key] = blifecondition.ToBinary();

            // Return completed
            return Task.CompletedTask;
        }
    }
}
