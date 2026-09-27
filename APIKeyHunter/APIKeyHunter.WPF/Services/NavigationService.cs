using CommunityToolkit.Mvvm.ComponentModel;

namespace APIKeyHunter.WPF.Services;

/// <summary>
/// Simple single-window navigation service that swaps the active ViewModel.
/// </summary>
public sealed class NavigationService
{
    private readonly Func<Type, ObservableObject> _viewModelFactory;

    public event Action<ObservableObject>? Navigated;

    public NavigationService(Func<Type, ObservableObject> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
    {
        var vm = _viewModelFactory(typeof(TViewModel));
        Navigated?.Invoke(vm);
    }
}
