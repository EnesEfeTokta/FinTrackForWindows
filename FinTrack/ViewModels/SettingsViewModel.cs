using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Windows;
using System.Windows.Input;

namespace FinTrack.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string freeButtonStatus = string.Empty;

        [ObservableProperty]
        private string plusButtonStatus = string.Empty;

        [ObservableProperty]
        private string proButtonStatus = string.Empty;

        [ObservableProperty]
        private bool isFreeButtonEnabled = false;

        [ObservableProperty]
        private bool isPlusButtonEnabled = true;

        [ObservableProperty]
        private bool isProButtonEnabled = true;

        private readonly ILogger<SettingsViewModel> _logger;

        private readonly ProfileSettingsContentViewModel profileVM;
        private readonly SecuritySettingsContentViewModel securityVM;
        private readonly NotificationSettingsContentViewModel notificationsVM;
        private readonly AppSettingsContentViewModel appVM;

        [ObservableProperty]
        private ObservableObject currentPageViewModel;

        public ICommand ChangePageCommand { get; }

        public SettingsViewModel(
            ILogger<SettingsViewModel> logger,
            ProfileSettingsContentViewModel profileVM,
            SecuritySettingsContentViewModel securityVM,
            NotificationSettingsContentViewModel notificationsVM,
            AppSettingsContentViewModel appVM
            )
        {
            _logger = logger;
            InitializeButtonStatuses();
            this.profileVM = profileVM;
            this.securityVM = securityVM;
            this.notificationsVM = notificationsVM;
            this.appVM = appVM;

            ChangePageCommand = new RelayCommand<string>(ChangePage);

            CurrentPageViewModel = profileVM;
        }

        private void ChangePage(string pageName)
        {
            _logger.LogInformation($"Sayfa değiştiriliyor: {pageName}");
            switch (pageName)
            {
                case "Profile":
                    CurrentPageViewModel = profileVM;
                    break;
                case "Security":
                    CurrentPageViewModel = securityVM;
                    break;
                case "Notifications":
                    CurrentPageViewModel = notificationsVM;
                    break;
                case "App":
                    CurrentPageViewModel = appVM;
                    break;
                default:
                    _logger.LogWarning($"Bilinmeyen sayfa adı: {pageName}");
                    break;
            }
        }

        private void InitializeButtonStatuses()
        {
            FreeButtonStatus = "Seçili";
            PlusButtonStatus = "Yükselt";
            ProButtonStatus = "Yükselt";
            _logger.LogInformation("Üyelik düğmelerinin durumları ayarlandı.");
        }

        [RelayCommand]
        private void SelectFreeMembership()
        {
            _logger.LogInformation("Free üyelik seçildi.");
            MessageBox.Show("Free üyelik seçildi.", "Üyelik Seçimi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void SelectPlusMembership()
        {
            _logger.LogInformation("Plus üyelik seçildi.");
            MessageBox.Show("Plus üyelik seçildi.", "Üyelik Seçimi", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        [RelayCommand]
        private void SelectProMembership()
        {
            _logger.LogInformation("Pro üyelik seçildi.");
            MessageBox.Show("Pro üyelik seçildi.", "Üyelik Seçimi", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
