using CommunityToolkit.Mvvm.ComponentModel;

namespace HavokActorTool.Core;

public sealed partial class HkActor(
    string hkrbFilePath,
    string name,
    string outputModFolder,
    string? modelName = null,
    string? baseActorName = null,
    bool useCustomModel = false,
    float lifeCondition = 500)
    : ObservableObject
{
    [ObservableProperty]
    private string _hkrbFilePath = hkrbFilePath;

    [ObservableProperty]
    private string _name = name;

    [ObservableProperty]
    private string _outputModFolder = outputModFolder;

    [ObservableProperty]
    private string _modelName = modelName ?? name;

    [ObservableProperty]
    private string? _baseActorName = baseActorName;

    [ObservableProperty]
    private bool _useCustomModel = useCustomModel;

    [ObservableProperty]
    private float _lifeCondition = lifeCondition;

    public HkActor() : this(string.Empty, string.Empty, string.Empty)
    {
    }
}