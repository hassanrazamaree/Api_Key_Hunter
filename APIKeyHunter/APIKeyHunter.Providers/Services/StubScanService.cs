using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Core.Models;

namespace APIKeyHunter.Providers.Services;

/// <summary>
/// Stub scan service — returns no results and accepts cancellation cleanly.
/// Replace with a real implementation when backend scanning is ready.
/// </summary>
public sealed class StubScanService : IScanService
{
    private static readonly ProviderConfig[] KnownProviders =
    [
        new() { Name = "GitHub",    Description = "GitHub personal access tokens", KeyPattern = "ghp_*",  IsEnabled = true },
        new() { Name = "OpenAI",    Description = "OpenAI API keys",                KeyPattern = "sk-*",   IsEnabled = true },
        new() { Name = "Stripe",    Description = "Stripe secret keys",             KeyPattern = "sk_*",   IsEnabled = true },
        new() { Name = "AWS",       Description = "AWS access key IDs",             KeyPattern = "AKIA*",  IsEnabled = true },
        new() { Name = "SendGrid",  Description = "SendGrid API keys",              KeyPattern = "SG.*",   IsEnabled = false },
        new() { Name = "Twilio",    Description = "Twilio auth tokens",             KeyPattern = "SK*",    IsEnabled = false },
    ];

    public Task<IReadOnlyList<ProviderConfig>> GetAvailableProvidersAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<ProviderConfig>>(KnownProviders);

    public async Task StartScanAsync(
        IReadOnlyList<string> providers,
        int maxResults,
        TimeSpan delay,
        IProgress<ScanProgress> progress,
        CancellationToken ct = default)
    {
        // Stub: emits an initial Ready status and then waits for cancellation.
        progress.Report(new ScanProgress
        {
            CurrentStatus = "Ready — connect a real scan provider to begin.",
            Discovered = 0,
            Valid = 0,
            Processed = 0,
            Total = 0
        });

        try
        {
            await Task.Delay(Timeout.Infinite, ct);
        }
        catch (OperationCanceledException)
        {
            // Expected when the user clicks Stop.
        }
    }
}
