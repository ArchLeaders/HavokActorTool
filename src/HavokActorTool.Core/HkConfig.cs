using CommunityToolkit.Mvvm.ComponentModel;
using ConfigFactory.Core;
using ConfigFactory.Core.Attributes;

namespace HavokActorTool.Core;

public sealed partial class HkConfig : ConfigModule<HkConfig>
{
    public override string Name => nameof(HavokActorTool);
    
    [ObservableProperty]
    [property: Config(
        Header = "Game Update Path",
        Description = "The absolute path to your dumped WiiU BotW game update dump.")]
    private string _gameUpdatePath = string.Empty;
    
    [ObservableProperty]
    [property: Config(
        Header = "Game Path (NX)",
        Description = "The absolute path to your dumped Switch (NX) BotW game dump.")]
    private string _gamePathNx = string.Empty;
}