using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Desktop.DI;
using TaskManager.Desktop.ViewModels;
using TaskManager.Desktop.Views;

namespace TaskManager.Desktop;

public sealed class App : Application
{
    private readonly IServiceProvider _services;

    public App()
    {
        _services = AppServices.Build();
    }

    public App(IServiceProvider services)
    {
        _services = services;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

#if DEBUG
        this.AttachDeveloperTools();
#endif
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var viewModel = _services.GetRequiredService<MainViewModel>();
            desktop.MainWindow = new MainWindow { DataContext = viewModel };
            viewModel.ReloadCommand.Execute(null);
        }

        base.OnFrameworkInitializationCompleted();
    }
}