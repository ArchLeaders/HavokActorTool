using System.Text.Json;
using System.Text.Json.Serialization;

namespace HavokActorTool.Core;

public sealed class HkConfig
{
    private static readonly string _path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HavokActorTool", "Config.json");
    
    public static readonly HkConfig Shared = Load();
    
    public static HkConfig Load()
    {
        if (!File.Exists(_path)) {
            return new HkConfig();
        }
        
        using FileStream fs = File.OpenRead(_path);
        return JsonSerializer.Deserialize(fs, HkConfigJsonContext.Default.HkConfig)
               ?? new HkConfig();
    }

    public string GameUpdatePath { get; set; } = string.Empty;
    
    public string GamePathNx { get; set; } = string.Empty;

    public void Save()
    {
        if (Path.GetDirectoryName(_path) is string folder) {
            Directory.CreateDirectory(folder);
        }
        
        using FileStream fs = File.Create(_path);
        JsonSerializer.Serialize(fs, this, HkConfigJsonContext.Default.HkConfig);
    }
}

[JsonSerializable(typeof(HkConfig))]
public sealed partial class HkConfigJsonContext : JsonSerializerContext;