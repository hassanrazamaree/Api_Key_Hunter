using System.Windows.Controls;
using APIKeyHunter.WPF.ViewModels;

namespace APIKeyHunter.WPF.Views;

public partial class TokenManagerView : UserControl
{
    public TokenManagerView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is TokenManagerViewModel vm)
                await vm.LoadAsync();
        };

        // Sync PasswordBox → ViewModel (PasswordBox doesn't support two-way binding directly)
        TokenPasswordBox.PasswordChanged += (_, _) =>
        {
            if (DataContext is TokenManagerViewModel vm)
                vm.RawToken = TokenPasswordBox.Password;
        };
    }
}
