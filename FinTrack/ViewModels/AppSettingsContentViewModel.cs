using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class AppSettingsContentViewModel : ObservableObject
    {
        public ObservableCollection<AppearanceType> AppearanceTypes { get; }
        public ObservableCollection<BaseCurrencyType> CurrencyTypes { get; }

        [ObservableProperty]
        private AppearanceType _selectedAppearanceType;

        [ObservableProperty]
        private BaseCurrencyType _selectedCurrencyType;

        [ObservableProperty]
        private bool _startWithWindows;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))]
        private bool _isSaving;

        private readonly ILogger<AppSettingsContentViewModel> _logger;
        private readonly IApiService _apiService;

        public AppSettingsContentViewModel(ILogger<AppSettingsContentViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;

            AppearanceTypes = new ObservableCollection<AppearanceType>(Enum.GetValues<AppearanceType>());
            CurrencyTypes = new ObservableCollection<BaseCurrencyType>(Enum.GetValues<BaseCurrencyType>());

            _ = LoadAppSettings();
        }

        private async Task LoadAppSettings()
        {
            IsLoading = true;
            try
            {
                var settings = await _apiService.GetAsync<UserAppSettingsDto>("UserSettings/AppSettings");
                if (settings != null)
                {
                    SelectedAppearanceType = settings.Appearance;
                    SelectedCurrencyType = settings.Currency;
                    // StartWithWindows = settings.StartWithWindows; // DTO'ya eklendiğinde bu satır açılmalı
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load application settings.");
                MessageBox.Show("Could not load application settings. Default values will be used.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanSaveChanges))]
        private async Task SaveChanges()
        {
            IsSaving = true;
            try
            {
                var settingsToUpdate = new UserAppSettingsUpdateDto
                {
                    Appearance = SelectedAppearanceType,
                    Currency = SelectedCurrencyType
                    // StartWithWindows = this.StartWithWindows; // DTO'ya eklendiğinde bu satır açılmalı
                };

                await _apiService.PostAsync<object>("UserSettings/AppSettings", settingsToUpdate);

                // TODO: StartWithWindows ayarını gerçekten sisteme uygulayacak bir servis çağrısı burada yapılmalı.
                // Örneğin: _startupService.SetStartup(this.StartWithWindows);

                _logger.LogInformation("Application settings have been updated by the user.");
                MessageBox.Show("Settings saved successfully!", "Application Settings", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save application settings.");
                MessageBox.Show("An error occurred while saving your settings. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsSaving = false;
            }
        }

        private bool CanSaveChanges() => !_isSaving;
    }
}