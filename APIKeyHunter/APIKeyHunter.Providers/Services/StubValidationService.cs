using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Core.Models;

namespace APIKeyHunter.Providers.Services;

/// <summary>
/// Stub validation service. Replace with a legitimate test provider.
/// </summary>
public sealed class StubValidationService : IValidationService
{
    public Task<int> GetQueueCountAsync(CancellationToken ct = default)
        => Task.FromResult(0);

    public async Task StartValidationAsync(
        int targetCount,
        IProgress<ValidationQueueItem> progress,
        CancellationToken ct = default)
    {
        try
        {
            await Task.Delay(Timeout.Infinite, ct);
        }
        catch (OperationCanceledException)
        {
            // Expected.
        }
    }
}
