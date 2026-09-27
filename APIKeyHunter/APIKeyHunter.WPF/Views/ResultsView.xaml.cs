using System.Windows.Controls;
using APIKeyHunter.WPF.ViewModels;

namespace APIKeyHunter.WPF.Views;

public partial class ResultsView : UserControl
{
    public ResultsView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ResultsViewModel vm)
                await vm.LoadAsync();
        };
    }
}
