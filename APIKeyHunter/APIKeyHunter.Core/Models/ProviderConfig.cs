namespace APIKeyHunter.Core.Models;

/// <summary>Configuration for a scan provider.</summary>
public sealed class ProviderConfig
{
    public string Name { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public string Description { get; set; } = string.Empty;
    public string KeyPattern { get; set; } = string.Empty;
}
