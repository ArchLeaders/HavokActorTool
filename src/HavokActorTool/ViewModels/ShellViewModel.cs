using CommunityToolkit.Mvvm.ComponentModel;
using HavokActorTool.Core;

namespace HavokActorTool.ViewModels;

public sealed partial class ShellViewModel : ObservableObject
{
    [ObservableProperty]
    private HkActor _actor = new();
}