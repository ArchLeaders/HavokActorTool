using Nintendo.Aamp;

namespace HavokActorTool.Core.ActorParams;

public class ModelListBuilder
{
    public static void Build(AampFile modelList, string modelName, string actorName)
    {
        modelList
            .RootNode
            .ChildParams[0]
            .ChildParams[0]
            .ParamObjects[0]
            .ParamEntries[0]
            .Value = new StringEntry(modelName);
        
        modelList
            .RootNode
            .ChildParams[0]
            .ChildParams[0]
            .ChildParams[0]
            .ParamObjects[0]
            .ParamEntries[0]
            .Value = new StringEntry(actorName);
    }
}