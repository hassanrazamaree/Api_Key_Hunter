using System.Windows;
using APIKeyHunter.Core.Interfaces;
using APIKeyHunter.Data.Context;
using APIKeyHunter.Data.Services;
using APIKeyHunter.Providers.Services;
using APIKeyHunter.WPF.Services;
using APIKeyHunter.WPF.ViewModels;
using APIKeyHunter.WPF.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace APIKeyHunter.WPF;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();

        // Ensure database is created
        using (var scope = _serviceProvider.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureCreated();
        }

        var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Logging
        services.AddLogging(b =>
        {
            b.AddDebug();
            b.SetMinimumLevel(LogLevel.Information);
        });

        // Database
        services.AddDbContextFactory<AppDbContext>(opts =>
            opts.UseSqlite("Data Source=apikeyresults.db"));

        // Service implementations
        services.AddSingleton<IDatabaseService, DatabaseService>();
        services.AddSingleton<ITokenService, StubTokenService>();
        services.AddSingleton<IScanService, StubScanService>();
        services.AddSingleton<IValidationService, StubValidationService>();
        services.AddSingleton<IExportService, ExportService>();

        // ViewModels — transient so each navigation creates a fresh instance
        services.AddTransient<MainViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<TokenManagerViewModel>();
        services.AddTransient<ScannerViewModel>();
        services.AddTransient<ValidatorViewModel>();
        services.AddTransient<ResultsViewModel>();

        // Navigation — uses a factory that creates fresh ViewModel instances from the container
        services.AddSingleton<NavigationService>(sp =>
            new NavigationService(type =>
                (ObservableObject)sp.GetRequiredService(type)));

        // Windows
        services.AddSingleton<MainWindow>();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }
}
