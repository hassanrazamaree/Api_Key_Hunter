using CommunityToolkit.Mvvm.ComponentModel;
using APIKeyHunter.Core.Interfaces;

namespace APIKeyHunter.WPF.ViewModels;

public sealed partial class DashboardViewModel : ObservableObject
{
    private readonly IDatabaseService _db;

    [ObservableProperty] private int _totalDiscovered;
    [ObservableProperty] private int _totalValid;
    [ObservableProperty] private int _totalInvalid;
    [ObservableProperty] private int _totalTokens;
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private string? _errorMessage;

    public DashboardViewModel(IDatabaseService db)
    {
        _db = db;
    }

    public async Task LoadAsync(CancellationToken ct = default)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var all     = await _db.GetKeysAsync(ct: ct);
            TotalDiscovered = all.Count;
            TotalValid      = all.Count(k => k.Status == Core.Models.KeyStatus.Valid);
            TotalInvalid    = all.Count(k => k.Status == Core.Models.KeyStatus.Invalid);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not load statistics: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
