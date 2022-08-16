using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HavokActorTool
{
    public class BcmlConfig
    {
        public static JsonElement? Get(string config)
        {
            var json = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(File.ReadAllText($"{Environment.GetEnvironmentVariable("LOCALAPPDATA")}\\bcml\\settings.json"));

            if (json != null) {
                if (json.ContainsKey(config.ToLower())) {
                    return (JsonElement)json[config.ToLower()];
                }
            }

            return null;
        }
    }
}
