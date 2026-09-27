using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Core.Models;

namespace APIKeyHunter.WPF.ViewModels;

public sealed partial class TokenManagerViewModel : ObservableObject
{
    private readonly ITokenService _tokenService;

    // ── Form fields ──
    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(SaveTokenCommand))]
    private string _tokenName = string.Empty;

    [ObservableProperty][NotifyCanExecuteChangedFor(nameof(SaveTokenCommand))]
    private string _rawToken = string.Empty;

    [ObservableProperty]
    private string _selectedProvider = "GitHub";

    [ObservableProperty]
    private bool _isTokenVisible;

    // ── State ──
    [ObservableProperty] private bool _isLoading;
    [ObservableProperty] private bool _isSaving;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string? _successMessage;

    [ObservableProperty]
    private TokenModel? _selectedToken;

    public ObservableCollection<TokenModel> Tokens { get; } = [];

    public IReadOnlyList<string> Providers { get; } =
        ["GitHub", "GitLab", "Bitbucket", "Azure DevOps"];

    public TokenManagerViewModel(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    public async Task LoadAsync(CancellationToken ct = default)
    {
        IsLoading = true;
        ErrorMessage = null;
        try
        {
            var tokens = await _tokenService.GetTokensAsync(ct);
            Tokens.Clear();
            foreach (var t in tokens)
                Tokens.Add(t);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load tokens: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanSaveToken))]
    private async Task SaveTokenAsync(CancellationToken ct)
    {
        IsSaving = true;
        ErrorMessage = null;
        SuccessMessage = null;
        try
        {
            var model = await _tokenService.SaveTokenAsync(
                TokenName.Trim(), RawToken.Trim(), SelectedProvider, ct);
            Tokens.Add(model);
            TokenName = string.Empty;
            RawToken  = string.Empty;
            SuccessMessage = "Token saved successfully.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to save token: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    private bool CanSaveToken()
        => !string.IsNullOrWhiteSpace(TokenName) &&
           !string.IsNullOrWhiteSpace(RawToken) &&
           !IsSaving;

    [RelayCommand]
    private async Task TestTokenAsync(CancellationToken ct)
    {
        if (SelectedToken is null) return;
        ErrorMessage = null;
        SuccessMessage = null;
        try
        {
            var status = await _tokenService.TestTokenAsync(SelectedToken.Id, ct);
            SelectedToken.TestStatus = status;
            // Force UI update
            var idx = Tokens.IndexOf(SelectedToken);
            if (idx >= 0)
            {
                Tokens.RemoveAt(idx);
                Tokens.Insert(idx, SelectedToken);
            }
            SuccessMessage = $"Test complete: {status}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Token test failed: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task DeleteTokenAsync(TokenModel? token)
    {
        if (token is null) return;
        ErrorMessage = null;
        try
        {
            await _tokenService.DeleteTokenAsync(token.Id);
            Tokens.Remove(token);
            if (SelectedToken == token) SelectedToken = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to delete token: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ToggleTokenVisibility() => IsTokenVisible = !IsTokenVisible;
}
