namespace APIKeyHunter.Core.Models;

/// <summary>Represents a discovered API key record.</summary>
public sealed class ApiKeyModel
{
    public int Id { get; set; }
    public string Provider { get; set; } = string.Empty;

    /// <summary>Masked preview, e.g. ghp_••••••••abc</summary>
    public string KeyPreview { get; set; } = string.Empty;

    public KeyStatus Status { get; set; } = KeyStatus.Unknown;
    public DateTime DiscoveredAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastCheckedAt { get; set; }
    public string? SourceUrl { get; set; }
    public string? Notes { get; set; }
}

public enum KeyStatus
{
    Unknown,
    Valid,
    Invalid,
    RateLimited,
    Expired
}
