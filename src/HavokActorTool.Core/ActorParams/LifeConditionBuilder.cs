using Nintendo.Aamp;

namespace HavokActorTool.Core.ActorParams;

public class LifeConditionBuilder()
{
    public static AampFile Build(float lifeConditionDistance, out string lifeConditionUser)
    {
        lifeConditionUser = $"Landmark{lifeConditionDistance}m";
        
        var result = AampFile.New(2);
        result.RootNode.ParamObjects = [
            new ParamObject {
                HashString = "DisplayDistance",
                ParamEntries = [
                    new ParamEntry {
                        HashString = "Item",
                        Value = lifeConditionDistance,
                        ParamType = ParamType.Float
                    }
                ]
            },
            new ParamObject {
                HashString = "AutoDisplayDistanceAlgorithm",
                ParamEntries = [
                    new ParamEntry {
                        HashString = "Item",
                        Value = new StringEntry("Bounding.Y"),
                        ParamType = ParamType.StringRef
                    }
                ]
            },
            new ParamObject {
                HashString = "YLimitAlgorithm",
                ParamEntries = [
                    new ParamEntry {
                        HashString = "Item",
                        Value = new StringEntry("NoLimit"),
                        ParamType = ParamType.StringRef
                    }
                ]
            }
        ];

        return result;
    }
}