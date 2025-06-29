using FinTrack.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Windows;

namespace FinTrack
{
    public partial class App : Application
    {
        private readonly IHost _host;

        public App()
        {
            _host = Host.CreateDefaultBuilder()
                .UseSerilog((context, services, configuration) =>
                {
                    configuration.ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureServices((context, services) =>
                {
                    ConfigureServices(services);
                })
                .Build();

            SetupGlobalExceptionHandling();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddTransient<AuthenticatorWindow>();

            // Diğer servisleriniz ve ViewModel'larınız da buraya eklenecek
            // services.AddTransient<MainViewModel>();
            // services.AddSingleton<IApiService, ApiService>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("Uygulama başlatıldı ve Host çalışıyor.");

            var mainWindow = _host.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        private void SetupGlobalExceptionHandling()
        {
            DispatcherUnhandledException += (sender, e) =>
            {
                var logger = _host.Services.GetRequiredService<ILogger<App>>();
                logger.LogCritical(e.Exception, "YAKALANAMAYAN UI HATASI!");

                MessageBox.Show("Beklenmedik bir hata oluştu. Uygulama kapanacak. Detaylar log dosyasına yazıldı.", "Kritik Hata", MessageBoxButton.OK, MessageBoxImage.Error);
                e.Handled = true;
                Shutdown();
            };

            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var logger = _host.Services.GetRequiredService<ILogger<App>>();
                logger.LogCritical(e.ExceptionObject as Exception, "YAKALANAMAYAN ARKA PLAN HATASI!");
            };
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            using (_host)
            {
                await _host.StopAsync(TimeSpan.FromSeconds(5));
            }
            base.OnExit(e);
        }
    }
}