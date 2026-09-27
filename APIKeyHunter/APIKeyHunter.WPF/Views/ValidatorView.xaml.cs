using System.Windows.Controls;
using APIKeyHunter.WPF.ViewModels;

namespace APIKeyHunter.WPF.Views;

public partial class ValidatorView : UserControl
{
    public ValidatorView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ValidatorViewModel vm)
                await vm.LoadAsync();
        };
    }
}
