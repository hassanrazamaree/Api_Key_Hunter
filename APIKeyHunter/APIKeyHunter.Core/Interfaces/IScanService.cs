using APIKeyHunter.Core.Models;

namespace APIKeyHunter.Core.Interfaces;

/// <summary>Controls scanning operations.</summary>
public interface IScanService
{
    Task StartScanAsync(
        IReadOnlyList<string> providers,
        int maxResults,
        TimeSpan delay,
        IProgress<ScanProgress> progress,
        CancellationToken ct = default);

    Task<IReadOnlyList<ProviderConfig>> GetAvailableProvidersAsync(CancellationToken ct = default);
}
