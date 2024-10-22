using Nintendo.Aamp;

namespace HavokActorTool.Core.ActorParams;

public sealed class PhysicsBuilder
{
    public static AampFile Build(string modelName, string actorName)
    {
        using Stream physicsProduct = typeof(PhysicsBuilder)
            .Assembly
            .GetManifestResourceStream("HavokActorTool.Core.Resources.Physics.Product.aamp")!;

        AampFile physics = AampFile.FromBinary(physicsProduct);
        physics.RootNode
            .ChildParams[0]
            .ChildParams[0]
            .ChildParams[0]
            .ParamObjects[0]
            .ParamEntries[3]
            .Value = new StringEntry($"{modelName}/{actorName}.hkrb");

        return physics;
    }
}