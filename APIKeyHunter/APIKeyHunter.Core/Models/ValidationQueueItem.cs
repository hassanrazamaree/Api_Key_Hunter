namespace APIKeyHunter.Core.Models;

/// <summary>Represents an item in the validation queue.</summary>
public sealed class ValidationQueueItem
{
    public int Id { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string KeyPreview { get; set; } = string.Empty;
    public ValidationStatus Status { get; set; } = ValidationStatus.Pending;
    public DateTime QueuedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ValidatedAt { get; set; }
    public string? ResultDetail { get; set; }
}

public enum ValidationStatus
{
    Pending,
    InProgress,
    Valid,
    Invalid,
    Error,
    Skipped
}
