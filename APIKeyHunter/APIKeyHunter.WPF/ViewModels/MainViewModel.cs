using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using APIKeyHunter.WPF.Services;

namespace APIKeyHunter.WPF.ViewModels;

public sealed partial class MainViewModel : ObservableObject
{
    private readonly NavigationService _navigation;

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private bool _isIdle = true;

    [ObservableProperty]
    private NavItem _selectedNavItem = NavItem.Dashboard;

    public MainViewModel(NavigationService navigation)
    {
        _navigation = navigation;
        _navigation.Navigated += vm =>
        {
            CurrentViewModel = vm;
        };

        // Start on Dashboard
        NavigateToDashboard();
    }

    [RelayCommand]
    private void NavigateToDashboard()
    {
        SelectedNavItem = NavItem.Dashboard;
        _navigation.NavigateTo<DashboardViewModel>();
    }

    [RelayCommand]
    private void NavigateToTokenManager()
    {
        SelectedNavItem = NavItem.TokenManager;
        _navigation.NavigateTo<TokenManagerViewModel>();
    }

    [RelayCommand]
    private void NavigateToScanner()
    {
        SelectedNavItem = NavItem.Scanner;
        _navigation.NavigateTo<ScannerViewModel>();
    }

    [RelayCommand]
    private void NavigateToValidator()
    {
        SelectedNavItem = NavItem.Validator;
        _navigation.NavigateTo<ValidatorViewModel>();
    }

    [RelayCommand]
    private void NavigateToResults()
    {
        SelectedNavItem = NavItem.Results;
        _navigation.NavigateTo<ResultsViewModel>();
    }

    public void SetStatus(string text, bool idle = true)
    {
        StatusText = text;
        IsIdle = idle;
    }
}

public enum NavItem
{
    Dashboard,
    TokenManager,
    Scanner,
    Validator,
    Results
}
