using System.Reflection;
using Avalonia;
using ConsoleAppFramework;
using Kokuban;

namespace HavokActorTool;

internal sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0) {
            Console.WriteLine(Chalk.Bold.Gray +
                              $"Havok Actor Tool [Version {Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "???"}]\n");
            Console.WriteLine(Chalk.Underline.Gray +
                              "(c) 2024 ArchLeaders. MIT.\n");

            ConsoleApp.ConsoleAppBuilder app = ConsoleApp.Create();

            app.Add("", Commands.BuildCommand);
            app.Add("config", Commands.ConfigCommand);

            app.Run(args);
            return;
        }
        
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}