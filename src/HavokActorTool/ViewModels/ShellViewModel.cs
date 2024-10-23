using System.Diagnostics;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FluentAvalonia.UI.Controls;
using HavokActorTool.Core;
using Humanizer;

namespace HavokActorTool.ViewModels;

public sealed partial class ShellViewModel : ObservableObject
{
    public static readonly FilePickerFileType HkrbFormat = new("Havok Rigid Body") {
        Patterns = ["*.hkrb"]
    };
    
    [ObservableProperty]
    private HkActor _actor = new();

    [RelayCommand]
    private async Task BrowseHkrb()
    {
        IReadOnlyList<IStorageFile> results = await App.XamlRoot.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
            AllowMultiple = false,
            FileTypeFilter = [
                HkrbFormat,
                FilePickerFileTypes.All
            ]
        });

        if (results.Count < 1 || results[0].TryGetLocalPath() is not string path) {
            return;
        }
        
        Actor.HkrbFilePath = path;

        if (string.IsNullOrWhiteSpace(Actor.Name)) {
            Actor.Name = Path.GetFileNameWithoutExtension(path);
        }
    }

    [RelayCommand]
    private async Task BrowseModFolder()
    {
        IReadOnlyList<IStorageFolder> results = await App.XamlRoot.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions {
            AllowMultiple = false
        });

        if (results.Count < 1 || results[0].TryGetLocalPath() is not string path) {
            return;
        }
        
        Actor.OutputModFolder = path;
    }

    [RelayCommand]
    private static async Task Settings()
    {
        const string gameUpdatePathTip = "Game Update Path (WiiU)";
        TextBox gameUpdatePathTextBox = new() {
            Text = HkConfig.Shared.GameUpdatePath,
            Watermark = gameUpdatePathTip
        };
        
        ToolTip.SetTip(gameUpdatePathTextBox, gameUpdatePathTip);
        
        const string gamePathNxTip = "Game Path (NX)";
        TextBox gamePathNxTextBox = new() {
            Text = HkConfig.Shared.GamePathNx,
            Watermark = gamePathNxTip
        };
        
        ToolTip.SetTip(gamePathNxTextBox, gamePathNxTip);
        
        TaskDialog errorDialog = new() {
            Title = "Settings",
            XamlRoot = App.XamlRoot,
            Content = new StackPanel {
                Spacing = 15,
                Children = {
                    gameUpdatePathTextBox,
                    gamePathNxTextBox
                }
            },
            Buttons = [
                TaskDialogButton.OKButton
            ]
        };
        
        await errorDialog.ShowAsync();

        if (gameUpdatePathTextBox.Text is { } gameUpdatePath) {
            HkConfig.Shared.GameUpdatePath = gameUpdatePath;
        }

        if (gamePathNxTextBox.Text is { } gamePathNx) {
            HkConfig.Shared.GamePathNx = gamePathNx;
        }
        
        HkConfig.Shared.Save();
    }

    [RelayCommand]
    private async Task Build()
    {
        if (Actor.HasErrors) {
            ContentDialog dialog = new() {
                Title = "Invalid inputs",
                Content = "One or more errors were found in the input fields."
            };

            await dialog.ShowAsync();
            return;
        }

        string outputActorPath = Path.Combine(Actor.OutputModFolder, "Actor", "Pack", $"{Actor.Name}.sbactorpack");
        if (!File.Exists(outputActorPath)) {
            goto Build;
        }

        ContentDialog request = new() {
            Title = "Warning",
            Content = "The output actor already exists, would you like to replace it?",
            PrimaryButtonText = "Yes",
            SecondaryButtonText = "No",
            DefaultButton = ContentDialogButton.Secondary
        };

        if (await request.ShowAsync() is not ContentDialogResult.Primary) {
            return;
        }
        
    Build:
        try {
            HkActorBuilder.Build(Actor);
        }
        catch (Exception ex) {
            TaskDialog errorDialog = new() {
                Title = ex.GetType().Name.Humanize(),
                XamlRoot = App.XamlRoot,
                Content = new TextBox {
                    Text = ex.ToString(),
                    TextWrapping = TextWrapping.WrapWithOverflow,
                    IsReadOnly = true
                },
                Buttons = [
                    TaskDialogButton.OKButton
                ]
            };
            
            await errorDialog.ShowAsync();
        }
    }
}