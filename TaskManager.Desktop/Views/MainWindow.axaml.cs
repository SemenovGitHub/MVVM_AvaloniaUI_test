using Avalonia.Controls;
using TaskManager.Desktop.ViewModels;

namespace TaskManager.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        
        Closing += (_, _) =>
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.CancelPending();
            }
        };
    }
}
