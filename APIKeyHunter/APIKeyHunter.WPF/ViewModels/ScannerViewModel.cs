using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Core.Models;

namespace APIKeyHunter.WPF.ViewModels;

public sealed partial class ScannerViewModel : ObservableObject
{
    private readonly IScanService _scanService;
    private CancellationTokenSource? _cts;

    // ── Config ──
    [ObservableProperty] private int _maxResults   = 100;
    [ObservableProperty] private int _delayMs      = 500;

    // ── Progress ──
    [ObservableProperty][NotifyPropertyChangedFor(nameof(ProgressPercent))]
    private int _discovered;

    [ObservableProperty] private int _valid;
    [ObservableProperty] private int _processed;

    [ObservableProperty][NotifyPropertyChangedFor(nameof(ProgressPercent))]
    private int _total;

    [ObservableProperty] private double _progressValue;

    [ObservableProperty] private string _currentStatus = "Ready";
    [ObservableProperty] private bool _isScanning;
    [ObservableProperty] private bool _isRateLimited;
    [ObservableProperty] private string? _rateLimitInfo;
    [ObservableProperty] private string? _errorMessage;

    public string ProgressPercent => $"{ProgressValue:F0}%";

    public ObservableCollection<ProviderConfig> AvailableProviders { get; } = [];
    public ObservableCollection<string> LogEntries { get; } = [];

    public ScannerViewModel(IScanService scanService)
    {
        _scanService = scanService;
    }

    public async Task LoadAsync(CancellationToken ct = default)
    {
        try
        {
            var providers = await _scanService.GetAvailableProvidersAsync(ct);
            AvailableProviders.Clear();
            foreach (var p in providers)
                AvailableProviders.Add(p);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load providers: {ex.Message}";
        }
    }

    [RelayCommand(CanExecute = nameof(CanStartScan))]
    private async Task StartScanAsync()
    {
        _cts = new CancellationTokenSource();
        IsScanning = true;
        ErrorMessage = null;
        LogEntries.Clear();
        Discovered = 0;
        Valid      = 0;
        Processed  = 0;
        Total      = 0;
        ProgressValue = 0;
        CurrentStatus = "Initialising scan…";
        StartScanCommand.NotifyCanExecuteChanged();
        StopScanCommand.NotifyCanExecuteChanged();

        var selectedProviders = AvailableProviders
            .Where(p => p.IsEnabled)
            .Select(p => p.Name)
            .ToList();

        var progress = new Progress<ScanProgress>(OnProgress);

        try
        {
            await _scanService.StartScanAsync(
                selectedProviders,
                MaxResults,
                TimeSpan.FromMilliseconds(DelayMs),
                progress,
                _cts.Token);
        }
        catch (OperationCanceledException)
        {
            CurrentStatus = "Scan stopped.";
            AppendLog("Scan cancelled by user.");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Scan error: {ex.Message}";
            CurrentStatus = "Error";
            AppendLog($"ERROR: {ex.Message}");
        }
        finally
        {
            IsScanning = false;
            _cts?.Dispose();
            _cts = null;
            StartScanCommand.NotifyCanExecuteChanged();
            StopScanCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanStartScan() => !IsScanning;

    [RelayCommand(CanExecute = nameof(CanStopScan))]
    private void StopScan()
    {
        _cts?.Cancel();
    }

    private bool CanStopScan() => IsScanning;

    private void OnProgress(ScanProgress p)
    {
        Discovered    = p.Discovered;
        Valid         = p.Valid;
        Processed     = p.Processed;
        Total         = p.Total;
        ProgressValue = p.PercentComplete;
        CurrentStatus = p.CurrentStatus;
        IsRateLimited = p.IsRateLimited;
        RateLimitInfo = p.IsRateLimited && p.RateLimitReset.HasValue
            ? $"Rate limit resets in {p.RateLimitReset.Value.TotalSeconds:F0}s"
            : null;

        if (p.LogMessage is { Length: > 0 })
            AppendLog(p.LogMessage);
    }

    private void AppendLog(string message)
    {
        var entry = $"[{DateTime.Now:HH:mm:ss}] {message}";
        System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            if (LogEntries.Count >= 500)
                LogEntries.RemoveAt(0);
            LogEntries.Add(entry);
        });
    }
}
