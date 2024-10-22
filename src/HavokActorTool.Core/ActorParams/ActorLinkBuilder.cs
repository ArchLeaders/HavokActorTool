using Nintendo.Aamp;
using SarcLibrary;

namespace HavokActorTool.Core.ActorParams;

public class ActorLinkBuilder
{
    private const uint LIFE_CONDITION_USER_KEY = 0x18af6b57;
    private const uint MODEL_USER_KEY = 0xb7f888d1;
    private const uint PHYSICS_USER_KEY = 0x8d0f8307;
    private const uint PROFILE_USER_KEY = 0x48f44665;
    
    public static void Build(AampFile actorLink, string lifeConditionUser, string modelUser, string physicsUser, bool updateModelList)
    {
        foreach (ParamEntry link in actorLink.RootNode.ParamObjects[0].ParamEntries) {
            link.Value = link.Hash switch {
                LIFE_CONDITION_USER_KEY => new StringEntry(lifeConditionUser),
                MODEL_USER_KEY when updateModelList => new StringEntry(modelUser),
                PHYSICS_USER_KEY => new StringEntry(physicsUser),
                PROFILE_USER_KEY => new StringEntry("MapDynamicActive"),
                _ => link.Value
            };
        }
    }
}