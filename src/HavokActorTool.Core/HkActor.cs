using System.ComponentModel.DataAnnotations;
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
    : ObservableValidator
{
    [ObservableProperty]
    [Required(ErrorMessage = "Required field.")]
    private string _hkrbFilePath = hkrbFilePath;

    [ObservableProperty]
    [Required(ErrorMessage = "Required field.")]
    private string _outputModFolder = outputModFolder;

    [ObservableProperty]
    [Required(ErrorMessage = "Required field.")]
    private string _name = name;

    [ObservableProperty]
    private string _modelName = modelName ?? name;

    [ObservableProperty]
    private string? _baseActorName = baseActorName;

    [ObservableProperty]
    private bool _useCustomModel = useCustomModel;

    [ObservableProperty]
    [Required, Range(100, 100_000)]
    private float _lifeCondition = lifeCondition;

    public HkActor() : this(string.Empty, string.Empty, string.Empty)
    {
    }

    partial void OnNameChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(ModelName)) {
            return;
        }

        ModelName = value.Split('_').LastOrDefault() is string lastNameArg && int.TryParse(lastNameArg, out _)
            ? value[..^(lastNameArg.Length + 1)]
            : value;
    }
}