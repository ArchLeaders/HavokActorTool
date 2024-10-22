using System.Diagnostics;
using ConsoleAppFramework;
using HavokActorTool.Core;
using Kokuban;

namespace HavokActorTool;

public static class Commands
{
    /// <summary>
    /// Builds a custom havok actor.
    /// </summary>
    /// <param name="hkrb">The absolute path to the input HKRB (havok rigid-body) file.</param>
    /// <param name="actorName">The output actor name.</param>
    /// <param name="outputModPath">The absolute path to the output mod folder.</param>
    /// <param name="modelName">The actor bfres name.</param>
    /// <param name="baseActorName">The name of the vanilla actor to base the new actor on.</param>
    /// <param name="useCustomModel">Update the actor info and model list to use the specified actor and model name.</param>
    /// <param name="lifeCondition">The life condition distance in meters.</param>
    /// <returns></returns>
    public static void BuildCommand(
        [Argument] string hkrb,
        [Argument] string actorName,
        [Argument] string outputModPath,
        string? modelName = null,
        string? baseActorName = null,
        bool useCustomModel = false,
        float lifeCondition = 500)
    {
        string outputActorPath = Path.Combine(outputModPath, "Actor", "Pack", $"{actorName}.sbactorpack");
        if (!File.Exists(outputActorPath)) {
            goto Run;
        }
        
        Console.Write(Chalk.Yellow.Bold +
                      "The output actor already exists, would you like to replace it? [y/N] ");
        if (Console.ReadLine() is not { Length: > 0 } response || response[0] is not ('Y' or 'y')) {
            return;
        }

    Run:
        var watch = Stopwatch.StartNew();
        
        HkActor actor = new(hkrb, actorName, outputModPath, modelName, baseActorName, useCustomModel, lifeCondition);
        HkActorBuildResults? results = HkActorBuilder.Build(actor);
        
        watch.Stop();

        if (results is null) {
            Console.WriteLine(Chalk.BrightRed.Bold.Underline +
                              "The actor failed to build. Please check your input and try again.");
            return;
        }

        Console.WriteLine(Chalk.BrightGreen.Bold +
                          "\nActor built successfully.");
        Console.WriteLine(Chalk.Italic.Underline +
                          results.ActorPath);
        Console.WriteLine(Chalk.BrightGreen.Bold +
                          $"\nElapsed: {watch.ElapsedMilliseconds}ms");
    }
    
    /// <summary>
    /// Update the game paths.
    /// </summary>
    /// <param name="gameUpdatePath">The absolute path to your dumped WiiU BotW game update dump.</param>
    /// <param name="gamePathNx">The absolute path to your dumped Switch (NX) BotW game dump.</param>
    public static void ConfigCommand(string? gameUpdatePath = null, string? gamePathNx = null)
    {
        if (gameUpdatePath is not null) {
            Console.WriteLine(Chalk.Italic + "Updated WiiU game update path.");
            HkConfig.Shared.GameUpdatePath = gameUpdatePath;
        }
        
        if (gamePathNx is not null) {
            Console.WriteLine(Chalk.Italic + "Updated NX game path.");
            HkConfig.Shared.GamePathNx = gamePathNx;
        }

        HkConfig.Shared.Save();
    }
}