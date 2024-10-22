using Nintendo.Aamp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavokActorTool.Actor
{
    public class BModelList
    {
        public static string MethodHeader { get; set; } = $"[{nameof(BModelList)}]";
        public static Task Create(ref Dictionary<string, byte[]> dict, string key, string actorName, string modelName)
        {
            // Notify interface
            Print($"{MethodHeader} Modify binary model list (BMODELLIST) . . .");

            // Parse bmodellist
            AampFile bmodellist = new(dict[key.Split('|')[0]]);

            // Set model list data
            bmodellist.RootNode.ChildParams[0].ChildParams[0].ChildParams[0].ParamObjects[0].ParamEntries[0].Value = new StringEntry(actorName);
            bmodellist.RootNode.ChildParams[0].ChildParams[0].ParamObjects[0].ParamEntries[0].Value = new StringEntry(modelName);

            // Update ref
            dict[key] = bmodellist.ToBinary();

            // Return completed
            return Task.CompletedTask;
        }
    }
}
