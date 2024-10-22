using Nintendo.Aamp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavokActorTool.Actor
{
    public class BXml
    {
        public static string MethodHeader { get; set; } = $"[{nameof(BXml)}]";
        public static Task Create(ref Dictionary<string, byte[]> dict, string key, string actorName, string landmark, bool newModel)
        {
            // Notify interface
            Print($"{MethodHeader} Modify binary actor links (BXML) . . .");

            // Parse bxml
            AampFile bxml = new(dict[key.Split('|')[0]]);

            // Update actor link data
            ParamEntry[] links = bxml.RootNode.ParamObjects[0].ParamEntries;

            foreach (ParamEntry link in links) {
                switch (link.HashString) {
                    case "PhysicsUser":
                        link.Value = new StringEntry(actorName);
                        break;
                    case "ModelUser":
                        if (newModel) link.Value = new StringEntry(actorName);
                        break;
                    case "LifeConditionUser":
                        link.Value = new StringEntry($"Landmark{landmark}");
                        break;
                    case "ProfileUser":
                        link.Value = new StringEntry($"MapDynamicActive");
                        break;
                }
            }

            // Update ActorLink
            dict[key] = bxml.ToBinary();

            // Return completed
            return Task.CompletedTask;
        }
    }
}
