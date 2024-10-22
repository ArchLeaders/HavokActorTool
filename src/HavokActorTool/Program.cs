using System.Reflection;
using ConsoleAppFramework;
using HavokActorTool;
using Kokuban;

Console.WriteLine(Chalk.Bold.Gray +
                  $"Havok Actor Tool [Version {Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "???"}]\n");
Console.WriteLine(Chalk.Underline.Gray +
                  "(c) 2024 ArchLeaders. MIT.\n");

ConsoleApp.ConsoleAppBuilder app = ConsoleApp.Create();

app.Add("", Commands.BuildCommand);
app.Add("config", Commands.ConfigCommand);

app.Run(args);