using Avalonia.Controls;
using TaskManager.Desktop.ViewModels;

namespace TaskManager.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            viewModel.Dispose();
        }

        base.OnClosing(e);
    }
}
