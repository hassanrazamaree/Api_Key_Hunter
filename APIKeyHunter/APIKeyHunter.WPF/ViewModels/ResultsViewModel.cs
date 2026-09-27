using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Core.Models;
using Microsoft.Win32;

namespace APIKeyHunter.WPF.ViewModels;

public sealed partial class ResultsViewModel : ObservableObject
{
    private readonly IDatabaseService _db;
    private readonly IExportService   _export;

    [ObservableProperty] private string  _searchText    = string.Empty;
    [ObservableProperty] private string? _providerFilter;
    [ObservableProperty] private string? _statusFilter;
    [ObservableProperty] private bool    _validOnly;
    [ObservableProperty] private bool    _isLoading;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string? _successMessage;
    [ObservableProperty] private ApiKeyModel? _selectedKey;

    private readonly ObservableCollection<ApiKeyModel> _allKeys = [];
    public ICollectionView KeysView { get; }

    public ObservableCollection<string> AvailableProviders { get; } = [];

    public IReadOnlyList<string?> StatusOptions { get; } =
        [null, "Valid", "Invalid", "RateLimited", "Expired", "Unknown"];

    public ResultsViewModel(IDatabaseService db, IExportService export)
    {
        _db     = db;
        _export = export;

        KeysView = CollectionViewSource.GetDefaultView(_allKeys);
        KeysView.Filter = ApplyFilter;
    }

    public async Task LoadAsync(CancellationToken ct = default)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var keys = await _db.GetKeysAsync(ct: ct);
            _allKeys.Clear();
            AvailableProviders.Clear();
            AvailableProviders.Add(string.Empty); // "All"

            foreach (var k in keys)
            {
                _allKeys.Add(k);
                if (!AvailableProviders.Contains(k.Provider))
                    AvailableProviders.Add(k.Provider);
            }
            KeysView.Refresh();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load results: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string _)   => KeysView.Refresh();
    partial void OnProviderFilterChanged(string? _) => KeysView.Refresh();
    partial void OnStatusFilterChanged(string? _)   => KeysView.Refresh();
    partial void OnValidOnlyChanged(bool _)         => KeysView.Refresh();

    private bool ApplyFilter(object obj)
    {
        if (obj is not ApiKeyModel key) return false;

        if (ValidOnly && key.Status != KeyStatus.Valid)
            return false;

        if (!string.IsNullOrEmpty(ProviderFilter) && key.Provider != ProviderFilter)
            return false;

        if (!string.IsNullOrEmpty(StatusFilter) &&
            Enum.TryParse<KeyStatus>(StatusFilter, true, out var s) &&
            key.Status != s)
            return false;

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var q = SearchText.Trim();
            return key.Provider.Contains(q, StringComparison.OrdinalIgnoreCase) ||
                   key.KeyPreview.Contains(q, StringComparison.OrdinalIgnoreCase);
        }

        return true;
    }

    [RelayCommand]
    private async Task DeleteSelectedAsync()
    {
        if (SelectedKey is null) return;
        ErrorMessage = null;
        try
        {
            await _db.DeleteKeyAsync(SelectedKey.Id);
            _allKeys.Remove(SelectedKey);
            SelectedKey = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Delete failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ExportCsvAsync()
    {
        var dlg = new SaveFileDialog
        {
            Title      = "Export CSV",
            Filter     = "CSV files|*.csv",
            FileName   = $"apikeyresults_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
        };
        if (dlg.ShowDialog() != true) return;

        try
        {
            var visible = KeysView.Cast<ApiKeyModel>().ToList();
            await _export.ExportToCsvAsync(visible, dlg.FileName);
            SuccessMessage = $"Exported {visible.Count} records to CSV.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Export failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task ExportJsonAsync()
    {
        var dlg = new SaveFileDialog
        {
            Title    = "Export JSON",
            Filter   = "JSON files|*.json",
            FileName = $"apikeyresults_{DateTime.Now:yyyyMMdd_HHmmss}.json"
        };
        if (dlg.ShowDialog() != true) return;

        try
        {
            var visible = KeysView.Cast<ApiKeyModel>().ToList();
            await _export.ExportToJsonAsync(visible, dlg.FileName);
            SuccessMessage = $"Exported {visible.Count} records to JSON.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Export failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private void RevalidatePlaceholder()
    {
        // Placeholder — wire up to IValidationService when ready.
        SuccessMessage = "Revalidation queued (connect backend to activate).";
    }
}
