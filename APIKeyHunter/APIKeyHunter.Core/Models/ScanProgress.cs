namespace APIKeyHunter.Core.Models;

/// <summary>Progress report emitted during a scan operation.</summary>
public sealed class ScanProgress
{
    public int Discovered { get; set; }
    public int Valid { get; set; }
    public int Processed { get; set; }
    public int Total { get; set; }
    public double PercentComplete => Total > 0 ? (double)Processed / Total * 100 : 0;
    public string CurrentStatus { get; set; } = string.Empty;
    public string? LogMessage { get; set; }
    public bool IsRateLimited { get; set; }
    public TimeSpan? RateLimitReset { get; set; }
}
