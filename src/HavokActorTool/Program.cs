using System.Reflection;
using ConsoleAppFramework;
using HavokActorTool;
using Kokuban;

Console.WriteLine(Chalk.Bold.Gray +
                  $"Havok Actor Tool [Version {Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "???"}]\n");
Console.WriteLine(Chalk.Underline.Gray +
                  "(c) 2024 ArchLeaders. MIT.\n");

unsafe {
    ConsoleApp.Run(args, &Commands.BuildCommand);
}