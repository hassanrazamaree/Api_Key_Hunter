namespace APIKeyHunter.Core.Models;

/// <summary>Stored GitHub (or other provider) authentication token.</summary>
public sealed class TokenModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Masked preview only — never the raw value.</summary>
    public string Preview { get; set; } = string.Empty;

    public string Provider { get; set; } = "GitHub";
    public bool IsActive { get; set; } = true;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastTestedAt { get; set; }
    public TokenTestStatus TestStatus { get; set; } = TokenTestStatus.NotTested;
}

public enum TokenTestStatus
{
    NotTested,
    Valid,
    Invalid,
    Testing
}
