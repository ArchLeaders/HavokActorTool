#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

using Aamp.Security.Cryptography;
using HavokActorTool.Actor;
using HavokActorTool.Common;
using Microsoft.VisualBasic.FileIO;
using Nintendo.Bfres;
using Nintendo.Byml;
using Nintendo.Sarc;
using Nintendo.Yaz0;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HavokActorTool
{
    public class HavokActor
    {
        public string Name { get; private set; }
        public string Root { get; set; }
        public string ModFolder { get; set; }
        public string HKRBPath { get; set; }
        public string MethodHeader { get; set; }
        public bool IsSwitch { get; set; } = false;
        public bool NewModel { get; set; } = true;
        public bool IsOverwrite { get; set; } = false;

        public string FullName { get; set; } = "FldObj_ActorBuilder_A_01";
        public string PartialName { get; set; } = "FldObj_ActorBuilder_A";
        public string Prefix { get; set; } = "FldObj";
        public string LetterId { get; set; } = "A";
        public string NumaricId { get; set; } = "01";

        public long HKRBSize { get; private set; }
        public string ActorPack { get; private set; }
        public int LifeCondition { get; private set; } = 500;
        public string? BaseActor { get; private set; } = null;

        public HavokActor(string modRoot, string modBase, string hkrbFile, bool formatName, bool newModel = true, string? baseActor = null)
        {
            if (!File.Exists(hkrbFile) || !hkrbFile.EndsWith(".hkrb")) {
                Console.WriteLine($"Error initializing ActorBuilder - Havok Rigid Body '{hkrbFile}' could not be found.");
                return;
            }

            // Get/Set misc
            IsSwitch = modBase.EndsWith("romfs");
            NewModel = newModel;
            MethodHeader = "[HavokActor]";

            // Get/Set filenames/dirs
            Root = modRoot;
            ModFolder = $"{Root}\\Build\\{modBase}";
            BaseActor = baseActor;
            HKRBPath = hkrbFile;
            HKRBSize = IsSwitch ? new FileInfo(HKRBPath).Length : new FileInfo(HKRBPath).Length * 4;
            FullName = Path.GetFileNameWithoutExtension(hkrbFile);

            // Format name with Prefix_Name_LID_NID
            if (formatName) {

                // Load headers
                List<string> headers = JsonSerializer.Deserialize<List<string>>(new Resource("Resources.Headers.json").Data)!;

                string[] indx = FullName.Split('_');
                int len = indx.Length;

                for (int i = 0; i < len; i++) {
                    if (indx[i].Length == 1 && !int.TryParse(indx[i], out _))
                        LetterId = indx[i];
                    else if (indx[i].Length == 2 && int.TryParse(indx[i], out _))
                        NumaricId = indx[i];
                    else if (headers.Contains(indx[i]))
                        Prefix = indx[i];
                    else
                        Name += indx[i] + '_';
                }

                // Remove trailing underscores
                while (Name![^1] == '_')
                    Name = Name.Remove(Name.Length - 1);

                PartialName = $"{Prefix}_{Name}_{LetterId}";
                FullName = $"{PartialName}_{NumaricId}";
            }
            else Name = FullName;

            ActorPack = $"{ModFolder}\\Actor\\Pack\\{FullName}.sbactorpack";
        }

        public async Task<Reporter> Construct()
        {
            // TIMER
            Stopwatch timer = new();
            timer.Start();

            // Create data dirs
            Directory.CreateDirectory(ModFolder);

            // Handle existing files/folders
            if (File.Exists(ActorPack) || Directory.Exists(ActorPack)) {
                timer.Stop();
                if (BoolInput($"The actorpack '{FullName}' already exists. Overwrite it? ")) {
                    IsOverwrite = true;
                    if (File.Exists(ActorPack)) {
                        FileSystem.DeleteFile(ActorPack, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                    else {
                        FileSystem.DeleteDirectory(ActorPack, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
                    }
                }
                timer.Start();
            }

            long nearest = -1;
            if (BaseActor == null) {

                // Find nearest HKRB match
                Print($"{MethodHeader} Searching HKRB file dictionary. . .");

                // Get HKRB Dictionary
                byte[] hkrbCacheBytes = Yaz0.Decompress(new Resource("Resources.HKRBCache.sjson").Data);
                Dictionary<long, string> hkrbDictionary = JsonSerializer.Deserialize<Dictionary<long, string>>(hkrbCacheBytes) ?? new();

                // Get closest match
                nearest = hkrbDictionary.Keys.OrderBy(x => Math.Abs(x - HKRBSize)).First();
                if (nearest == -1) {
                    Print($"!error||Could not find base actor '{BaseActor}' in size dictionary. Terminating process.");
                    return new($"Could not find base actor '{BaseActor}' in size dictionary.Terminating process.", "ActorBuilder Exception");
                }

                BaseActor = hkrbDictionary[nearest];
                Print($"{MethodHeader} Found {hkrbDictionary[nearest]} at {nearest}");
            }

            // Prep mod folder
            string? game;
            string? update;

            if (IsSwitch) {
                game = BcmlConfig.Get("game_dir_nx").ToString();
                update = game;
            }
            else {
                game = BcmlConfig.Get("game_dir").ToString();
                update = BcmlConfig.Get("update_dir").ToString();
            }

            if (game == null)
                return new("The game path returned null.\nThis could mean that BCML is not installed or setup correctly.", "ActorBuilder Exception");
            if (update == null)
                return new("The update path returned null.\nThis could mean that BCML is not installed or setup correctly.", "ActorBuilder Exception");

            Dictionary<string, string> prep = new()
            {
                { "!dir.pck", $"{ModFolder}\\Actor\\Pack" },
                { "!dir.mdl", $"{ModFolder}\\Model" },
                { $"{update}\\Actor\\Pack\\{BaseActor}.sbactorpack", ActorPack },
            };

            foreach (var dataSet in prep) {
                if (dataSet.Key.StartsWith("!dir")) {
                    Directory.CreateDirectory(dataSet.Value);
                    continue;
                }

                File.Copy(dataSet.Key, dataSet.Value, true);
            }

            // Open actorpack
            SarcFile actorpack = new(Yaz0.DecompressFast(File.ReadAllBytes(ActorPack)));

            // Stage actorpack edits
            List<Task> edit = new();

            // Create actorpack files deepcopy
            Dictionary<string, byte[]> files = new();
            foreach (var _file in actorpack.Files) {
                files.Add(_file.Key, _file.Value);
            }

            // Get LifeCondition
            timer.Stop();
            string lifeConditionInput = Input($"{MethodHeader} Enter the LifeCondition distance in meters: ");
            int lifeConditionInt = 0;
            while (!int.TryParse(lifeConditionInput, out lifeConditionInt)) {
                Print($"!error||{MethodHeader} [ERROR] | '{lifeConditionInput}' is invalid.");
                lifeConditionInput = Input($"{MethodHeader} Enter the LifeCondition distance in meters: ");
            }

            timer.Start();
            string lifeCondition = $"{lifeConditionInt}m";

            // Iterate SARC files
            foreach (var file in actorpack.Files.Keys) {

                // Edit BLifeCondition
                edit.Add(BLifeCondition.Create(ref files, $"{file}|Landmark{lifeCondition}.blifecondition", FullName, lifeConditionInt));

                // Edit BMmodelList
                if (file.EndsWith(".bmodellist") && NewModel) {
                    edit.Add(BModelList.Create(ref files, $"{file}|{FullName}.bmodellist", FullName, PartialName));
                }

                // Edit BPhysics
                if (file.EndsWith(".bphysics"))
                    edit.Add(BPhysics.Create(ref files, $"{file}|{FullName}.bphysics", FullName, PartialName));

                // Edit BXML
                if (file.EndsWith(".bxml"))
                    edit.Add(BXml.Create(ref files, $"{file}|{FullName}.bxml", FullName, lifeCondition, NewModel));
            }

            // Modfiy ActorInfo
            edit.Add(Task.Run(() => {
                Print($"{MethodHeader} Adding ActorInfo entry. . .");

                // C# version

                // Load actorinfo file
                string actorinfoFile = $"{ModFolder}\\Actor\\ActorInfo.product.sbyml";
                if (!File.Exists(actorinfoFile)) {
                    File.Copy($"{update}\\Actor\\ActorInfo.product.sbyml", actorinfoFile);
                }

                // Create byml instance
                BymlFile actorinfo = new(Yaz0.DecompressFast($"{ModFolder}\\Actor\\ActorInfo.product.sbyml"));
                BymlNode havokActor = new(new Dictionary<string, BymlNode>() { });
                BymlNode actor = null!;
                foreach (var _actor in actorinfo.RootNode.Hash["Actors"].Array) {
                    if (_actor.Hash["name"].String == (IsOverwrite ? FullName : BaseActor)) {
                        actor = _actor;
                        break;
                    }
                }

                if (havokActor == null) {
                    Input($"!error||Fatal error, the BaseActor '{BaseActor}' was not found in the actor info.");
                    Environment.Exit(3);
                }

                foreach (var prop in actor.Hash) {
                    havokActor.Hash.Add(prop.Key, prop.Value.ShallowCopy());
                }

                havokActor.Hash["instSize"].Int64 = HKRBSize;
                havokActor.Hash["name"].String = FullName;
                havokActor.Hash["profile"].String = "MapDynamicActive";

                if (NewModel) {
                    havokActor.Hash["bfres"].String = PartialName;
                    havokActor.Hash["mainModel"].String = FullName;
                }

                if (!IsOverwrite) {
                    actorinfo.RootNode.Hash["Actors"].Array.Add(havokActor);
                    actorinfo.RootNode.Hash["Hashes"].Array.Add(new BymlNode(Crc32.Compute(FullName)));
                }

                File.WriteAllBytes(actorinfoFile, Yaz0.CompressFast(actorinfo.ToBinary()));

            }));

            // Add HKRB file
            edit.Add(Task.Run(() => {
                Print($"{MethodHeader} Adding Havok RigidBodies. . .");
                files.Add($"Physics/RigidBody/{PartialName}/{FullName}.hkrb", File.ReadAllBytes(HKRBPath));
            }));

            // Edit SBFRES files
            edit.Add(Task.Run(() => {
                Print($"{MethodHeader} Parsing Cafe Resources. . .");

                if (IsSwitch) {
                    Print($"!warn||{MethodHeader} Parsing Cafe Resources is currently WiiU only. . .");
                    return;
                }

                Dictionary<string, KeyValuePair<string, string?>> resources = new()
                {
                    { $"{update}\\Model\\FldObj_HyruleFountain_A.sbfres", new(PartialName, FullName) },
                    { $"{game}\\Model\\FldObj_HyruleFountain_A.Tex1.sbfres", new($"{PartialName}.Tex1", null) },
                    { $"{update}\\Model\\FldObj_HyruleFountain_A.Tex2.sbfres", new($"{PartialName}.Tex2", null) },
                };

                foreach (var resource in resources) {
                    bool copied = false;
                    string destBfres = $"{ModFolder}\\Model\\{resource.Value.Key}.sbfres";

                    if (!File.Exists(destBfres)) {
                        File.Copy(resource.Key, destBfres);
                        copied = true;
                    }

                    BfresFile bfres = new(new MemoryStream(Yaz0.DecompressFast(destBfres))) {
                        Name = resource.Value.Key
                    };

                    foreach (var model in bfres.Models) {

                        bool found = false;
                        if (copied) {
                            model.Value.Name = resource.Value.Value ?? model.Value.Name;
                            break;
                        }
                        else if (model.Value.Name == resource.Value.Value) {
                            found = true;
                        }

                        if (!found) {
                            Print($"!warn||{MethodHeader} WARNING! A model to satisfy the actor {FullName} could not be found in '{destBfres}'");
                        }
                    }

                    using var stream = new MemoryStream();
                    bfres.ToBinary(stream);
                    byte[] bfresBytes = Yaz0.CompressFast(stream.ToArray());

                    File.WriteAllBytes(destBfres, bfresBytes);
                }
            }));

            // Await edit
            await Task.WhenAll(edit);
            foreach (var key in files.Keys) {
                if (key.Contains('|')) {
                    actorpack.Files.Remove(key.Split('|')[0]);
                    var dir = Path.GetDirectoryName(key.Split('|')[0])!.Replace("\\", "/") + "/";
                    actorpack.Files.Add(dir + key.Split('|')[1], files[key]);
                }
                else {
                    actorpack.Files[key] = files[key];
                }
            }

            // Write SARC file
            Print($"{MethodHeader} Writing actorpack. . .");
            File.WriteAllBytes(ActorPack, Yaz0.CompressFast(actorpack.ToBinary(), 7));

            timer.Stop();
            Print($"{MethodHeader} Completed in {timer.ElapsedMilliseconds / 1000.0} seconds");

            return new("success");
        }
    }
}
