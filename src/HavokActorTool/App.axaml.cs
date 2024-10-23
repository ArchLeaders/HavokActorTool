using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using HavokActorTool.ViewModels;
using HavokActorTool.Views;

namespace HavokActorTool;

public partial class App : Application
{
    public static TopLevel XamlRoot => ((IClassicDesktopStyleApplicationLifetime)Current?.ApplicationLifetime!)
        .MainWindow!;
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop) {
            return;
        }
        
        desktop.MainWindow = new ShellView {
            DataContext = new ShellViewModel()
        };

        base.OnFrameworkInitializationCompleted();
    }
}