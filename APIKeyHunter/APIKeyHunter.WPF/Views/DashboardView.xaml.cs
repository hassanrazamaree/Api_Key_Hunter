using System.Windows.Controls;
using APIKeyHunter.WPF.ViewModels;

namespace APIKeyHunter.WPF.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is DashboardViewModel vm)
                await vm.LoadAsync();
        };
    }
}
