using CommunityToolkit.Mvvm.Messaging;
using FinTrackForWindows.Core;
using FinTrackForWindows.Services;
using FinTrackForWindows.Services.Api;
using FinTrackForWindows.ViewModels;
using FinTrackForWindows.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Windows;

namespace FinTrackForWindows
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
            services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

            services.AddSingleton<MainWindow>();
            services.AddSingleton<AuthenticatorWindow>();

            services.AddSingleton<MainViewModel>();
            services.AddSingleton<LoginViewModel>();

            services.AddTransient<TopBarViewModel>();
            services.AddTransient<BottomBarViewModel>();

            services.AddTransient<RegisterViewModel>();
            services.AddTransient<OtpVerificationViewModel>();
            services.AddTransient<ForgotPasswordViewModel>();
            services.AddTransient<ApplicationRecognizeSlideViewModel>();
            services.AddTransient<AuthenticatorViewModel>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<BudgetViewModel>();
            services.AddTransient<AccountViewModel>();
            services.AddTransient<TransactionsViewModel>();
            services.AddTransient<ReportsViewModel>();
            services.AddTransient<FinBotViewModel>();
            services.AddTransient<CurrenciesViewModel>();
            services.AddTransient<DebtViewModel>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<FeedbackViewModel>();

            services.AddTransient<SettingsView>();

            services.AddTransient<NotificationViewModel>();
            services.AddTransient<ProfileSettingsContentViewModel>();
            services.AddTransient<SecuritySettingsContentViewModel>();
            services.AddTransient<NotificationSettingsContentViewModel>();
            services.AddTransient<AppSettingsContentViewModel>();

            services.AddTransient<AccountView>();

            services.AddSingleton<IAuthService, AuthService>();
            services.AddSingleton<ISecureTokenStorage, SecureTokenStorage>();
            services.AddSingleton<IApiService, ApiService>();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await _host.StartAsync();

            var logger = _host.Services.GetRequiredService<ILogger<App>>();
            logger.LogInformation("Uygulama başlatıldı ve Host çalışıyor.");

            var window = _host.Services.GetRequiredService<AuthenticatorWindow>();
            window.Show();

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