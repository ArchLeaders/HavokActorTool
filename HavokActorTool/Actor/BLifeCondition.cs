﻿using Nintendo.Aamp;
using Nintendo.Yaz0;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HavokActorTool.Actor
{
    public class BLifeCondition
    {
        public static string MethodHeader { get; set; } = $"[{nameof(BLifeCondition)}]";
        public static Task Create(ref Dictionary<string, byte[]> dict, string key, string _, float lifeCondition)
        {
            // Notify interface
            Print($"{MethodHeader} Modify binary life condition (BLIFECONDITION) . . .");

            AampFile blifecondition = new(Yaz0.DecompressFast(new Resource("Resources.BLifeCondition.aamp").Data));
            blifecondition.RootNode.ParamObjects[0].ParamEntries[0].Value = lifeCondition;
            dict[key] = blifecondition.ToBinary();

            // Return completed
            return Task.CompletedTask;
        }
    }
}
