using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Core.Models;

namespace APIKeyHunter.WPF.ViewModels;

public sealed partial class ValidatorViewModel : ObservableObject
{
    private readonly IValidationService _validationService;
    private CancellationTokenSource? _cts;
    private bool _isPaused;

    [ObservableProperty] private int   _targetCount  = 50;
    [ObservableProperty] private int   _queueCount;
    [ObservableProperty] private int   _processedCount;
    [ObservableProperty] private string _currentProvider = "—";
    [ObservableProperty] private string _currentStatus   = "Idle";
    [ObservableProperty] private double _progressValue;
    [ObservableProperty] private bool  _isRunning;
    [ObservableProperty] private bool  _isPausedState;
    [ObservableProperty] private string? _errorMessage;

    public ObservableCollection<ValidationQueueItem> History { get; } = [];

    public ValidatorViewModel(IValidationService validationService)
    {
        _validationService = validationService;
    }

    public async Task LoadAsync(CancellationToken ct = default)
    {
        QueueCount = await _validationService.GetQueueCountAsync(ct);
    }

    [RelayCommand(CanExecute = nameof(CanStart))]
    private async Task StartAsync()
    {
        _cts = new CancellationTokenSource();
        IsRunning = true;
        IsPausedState = false;
        CurrentStatus = "Starting…";
        ErrorMessage = null;
        StartCommand.NotifyCanExecuteChanged();
        PauseCommand.NotifyCanExecuteChanged();
        StopCommand.NotifyCanExecuteChanged();

        var progress = new Progress<ValidationQueueItem>(OnItemValidated);

        try
        {
            await _validationService.StartValidationAsync(TargetCount, progress, _cts.Token);
            CurrentStatus = "Complete";
        }
        catch (OperationCanceledException)
        {
            CurrentStatus = _isPaused ? "Paused" : "Stopped";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Validation error: {ex.Message}";
            CurrentStatus = "Error";
        }
        finally
        {
            if (!_isPaused)
            {
                IsRunning = false;
                _cts?.Dispose();
                _cts = null;
            }
            StartCommand.NotifyCanExecuteChanged();
            PauseCommand.NotifyCanExecuteChanged();
            StopCommand.NotifyCanExecuteChanged();
        }
    }

    private bool CanStart() => !IsRunning;

    [RelayCommand(CanExecute = nameof(CanPauseOrResume))]
    private async Task PauseOrResumeAsync()
    {
        if (!IsPausedState)
        {
            // Pause
            _isPaused = true;
            IsPausedState = true;
            _cts?.Cancel();
            CurrentStatus = "Paused";
            PauseOrResumeCommand.NotifyCanExecuteChanged();
        }
        else
        {
            // Resume
            _isPaused = false;
            IsPausedState = false;
            IsRunning = false; // Allow CanStart to pass
            await StartAsync();
        }
    }

    private bool CanPauseOrResume() => IsRunning || IsPausedState;

    [RelayCommand(CanExecute = nameof(CanStop))]
    private void Stop()
    {
        _isPaused = false;
        _cts?.Cancel();
        IsRunning = false;
        CurrentStatus = "Stopped";
    }

    private bool CanStop() => IsRunning;

    private void OnItemValidated(ValidationQueueItem item)
    {
        CurrentProvider = item.Provider;
        ProcessedCount++;
        if (TargetCount > 0)
            ProgressValue = (double)ProcessedCount / TargetCount * 100;

        System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
        {
            History.Insert(0, item);
            if (History.Count > 200)
                History.RemoveAt(History.Count - 1);
        });
    }
}
