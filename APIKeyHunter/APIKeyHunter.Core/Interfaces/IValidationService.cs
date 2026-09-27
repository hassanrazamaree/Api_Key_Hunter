using APIKeyHunter.Core.Models;

namespace APIKeyHunter.Core.Interfaces;

/// <summary>Validates API keys against their respective providers.</summary>
public interface IValidationService
{
    Task StartValidationAsync(
        int targetCount,
        IProgress<ValidationQueueItem> progress,
        CancellationToken ct = default);

    Task<int> GetQueueCountAsync(CancellationToken ct = default);
}
