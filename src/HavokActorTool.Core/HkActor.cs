using CommunityToolkit.Mvvm.ComponentModel;

namespace HavokActorTool.Core;

public sealed partial class HkActor : ObservableObject
{
    [ObservableProperty]
    private string _hkrbFilePath = string.Empty;
    
    [ObservableProperty]
    private string _name = string.Empty;
    
    [ObservableProperty]
    private string _modelName = string.Empty;
    
    [ObservableProperty]
    private string? _baseActorName = string.Empty;
    
    [ObservableProperty]
    private string? _customModelBfresPath = string.Empty;
    
    [ObservableProperty]
    private string _outputModFolder = string.Empty;
    
    [ObservableProperty]
    private float _lifeCondition = 500;
}