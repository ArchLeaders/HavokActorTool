using System.Diagnostics;
using Aamp.Security.Cryptography;
using BymlLibrary;
using BymlLibrary.Nodes.Containers;
using CommunityToolkit.HighPerformance.Buffers;
using CsYaz0;
using HavokActorTool.Core.ActorParams;
using HavokActorTool.Core.Common;
using HavokActorTool.Core.Extensions;
using HavokActorTool.Core.IO;
using Nintendo.Aamp;
using Revrs;
using SarcLibrary;

namespace HavokActorTool.Core;

public static class HkActorBuilder
{
    public static HkActorBuildResults? Build(HkActor actor)
    {
        if (!CheckInputActor(actor, out FileInfo hkrbFile, out FileInfo outputActorFile, out bool isNx)) {
            return null;
        }

        long instSize = isNx ? hkrbFile.Length : hkrbFile.Length * 4;

        Sarc actorPack = GetBaseActor(actor, instSize, isNx);
        RemoveUnusedEntries(actorPack);
        BuildActor(actor, actorPack);

        YzFile.WriteAndCompress(outputActorFile,
            stream => actorPack.Write(stream, actorPack.Endianness)
        );

        Byml actorInfo = GetActorInfo(actor.OutputModFolder, isNx, out FileInfo outputActorInfoFile);
        BuildActorInfo(actor, actorInfo, instSize);

        YzFile.WriteAndCompress(outputActorInfoFile,
            stream => actorInfo.WriteBinary(stream, isNx ? Endianness.Little : Endianness.Big)
        );

        return new HkActorBuildResults(outputActorFile.FullName);
    }

    private static void BuildActor(HkActor actor, Sarc pack)
    {
        string baseActorLinkKey = $"Actor/ActorLink/{actor.BaseActorName}.bxml";
        AampFile actorLink = pack.GetNested(baseActorLinkKey);
        pack.Remove(baseActorLinkKey);

        if (actor.UseCustomModel) {
            string baseModelListKey = $"Actor/ModelList/{actor.BaseActorName}.bmodellist";
            AampFile modelList = pack.GetNested(baseModelListKey);
            pack.Remove(baseModelListKey);

            ModelListBuilder.Build(modelList, actor.ModelName, actor.Name);
            pack[$"Actor/ModelList/{actor.Name}.bmodellist"] = modelList.ToBinary();
        }

        AampFile lifeCondition = LifeConditionBuilder.Build(actor.LifeCondition, out string lifeConditionUser);
        pack[$"Actor/LifeCondition/{lifeConditionUser}.blifecondition"] = lifeCondition.ToBinary();

        AampFile physics = PhysicsBuilder.Build(actor.ModelName, actor.Name);
        pack[$"Actor/Physics/{actor.Name}.bphysics"] = physics.ToBinary();

        ActorLinkBuilder.Build(actorLink, lifeConditionUser, actor.Name, actor.Name,
            updateModelList: actor.UseCustomModel);
        pack[$"Actor/ActorLink/{actor.Name}.bxml"] = actorLink.ToBinary();

        pack[$"Physics/RigidBody/{actor.ModelName}/{actor.Name}.hkrb"]
            = File.ReadAllBytes(actor.HkrbFilePath);
    }

    private static void BuildActorInfo(HkActor actor, Byml actorInfo, long instSize)
    {
        uint baseActorHash = Crc32.Compute(actor.BaseActorName!);
        uint currentActorHash = Crc32.Compute(actor.Name);
        BymlMap root = actorInfo.GetMap();
        BymlArray hashes = root["Hashes"].GetArray();

        int i = -1;
        int currentIndex = -1;
        int baseIndex = -1;

        while ((currentIndex < 0 || baseIndex < 0) && ++i < hashes.Count) {
            uint store = Convert.ToUInt32(hashes[i].Value);
            if (store == currentActorHash) {
                currentIndex = i;
            }
            else if (store == baseActorHash) {
                baseIndex = i;
            }
        }

        if (baseIndex < -1) {
            throw new Exception(
                $"The base actor '{actor.BaseActorName}' could not be found in the provided actor info file.");
        }

        BymlArray actors = root["Actors"].GetArray();
        BymlMap entry = actors[baseIndex]
            .HardCopy()
            .GetMap();

        if (currentIndex < 0) {
            hashes.Add(currentActorHash);
            hashes.Sort(BymlHashComparer.Instance);
            currentIndex = hashes.FindIndex(
                hash => Convert.ToUInt32(hash.Value) == currentActorHash);
            actors.Insert(currentIndex, entry);
        }
        else {
            actors[currentIndex] = entry;
        }

        entry["instSize"] = instSize;
        entry["name"] = actor.Name;
        entry["profile"] = "MapDynamicActive";

        if (actor.UseCustomModel) {
            entry["bfres"] = actor.ModelName;
            entry["mainModel"] = actor.Name;
        }
    }

    private static void RemoveUnusedEntries(Sarc pack)
    {
        string[] physicsFiles = pack.Keys
            .Where(
                static fileName => {
                    ReadOnlySpan<char> key = fileName.AsSpan();
                    return (key.Length > 6 && key[..7] is "Physics") ||
                           (key.Length > 12 && key[..13] is "Actor/Physics") ||
                           (key.Length > 18 && key[..19] is "Actor/LifeCondition");
                })
            .ToArray();

        foreach (string key in physicsFiles) {
            pack.Remove(key);
        }
    }

    private static Byml GetActorInfo(string outputModFolder, bool isNx, out FileInfo outputActorInfoFile)
    {
        outputActorInfoFile = new FileInfo(
            Path.Combine(outputModFolder, "Actor", "ActorInfo.product.sbyml")
        );

        string actorInfoPath = $"{(isNx ? HkConfig.Shared.GamePathNx : HkConfig.Shared.GameUpdatePath)}/Actor/ActorInfo.product.sbyml";
        using SpanOwner<byte> buffer = YzFile.OpenAndDecompress(actorInfoPath);
        return Byml.FromBinary(buffer.Span);
    }

    private static Sarc GetBaseActor(HkActor actor, long instSize, bool isNx)
    {
        actor.BaseActorName ??= HkActorCache.GetNearestActor(instSize);
        string actorPath = $"{(isNx ? HkConfig.Shared.GamePathNx : HkConfig.Shared.GameUpdatePath)}/Actor/Pack/{actor.BaseActorName}.sbactorpack";

        using FileStream fs = File.OpenRead(actorPath);
        int size = (int)fs.Length;
        using SpanOwner<byte> compressedActorPackBuffer = SpanOwner<byte>.Allocate(size);
        int read = fs.Read(compressedActorPackBuffer.Span);
        Debug.Assert(read == size);

        byte[] buffer = Yaz0.Decompress(compressedActorPackBuffer.Span);
        return Sarc.FromBinary(buffer);
    }

    private static bool CheckInputActor(HkActor actor, out FileInfo hkrbFile, out FileInfo outputActorFile, out bool isNx)
    {
        hkrbFile = new FileInfo(actor.HkrbFilePath);
        outputActorFile = new FileInfo(
            Path.Combine(actor.OutputModFolder, "Actor", "Pack", $"{actor.Name}.sbactorpack")
        );

        ReadOnlySpan<char> outputModFolder = actor.OutputModFolder.AsSpan();
        isNx = outputModFolder.Length < 5
               || Path.GetFileName(outputModFolder[^1] is '/' or '\\' ? outputModFolder[..^1] : outputModFolder) is "romfs";

        return hkrbFile.Exists;
    }
}