using System.Windows.Controls;
using APIKeyHunter.WPF.ViewModels;

namespace APIKeyHunter.WPF.Views;

public partial class ScannerView : UserControl
{
    public ScannerView()
    {
        InitializeComponent();
        Loaded += async (_, _) =>
        {
            if (DataContext is ScannerViewModel vm)
                await vm.LoadAsync();
        };
    }
}
