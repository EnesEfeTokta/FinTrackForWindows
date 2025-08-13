using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.SettingsDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Services.AppInNotifications;
using FinTrackForWindows.Services.ApplySettings;
using FinTrackForWindows.Services.Users;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrackForWindows.ViewModels
{
    public partial class AppSettingsContentViewModel : ObservableObject
    {
        public ObservableCollection<AppearanceType> AppearanceTypes { get; }
        public ObservableCollection<BaseCurrencyType> CurrencyTypes { get; }
        public ObservableCollection<LanguageType> LanguageTypes { get; }

        [ObservableProperty]
        private AppearanceType _selectedAppearanceType;

        [ObservableProperty]
        private BaseCurrencyType _selectedCurrencyType;

        [ObservableProperty]
        private LanguageType _selectedLanguageType;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveChangesCommand))]
        private bool _isSaving;

        private readonly ILogger<AppSettingsContentViewModel> _logger;
        private readonly IUserStore _userStore;
        private readonly IAppInNotificationService _appInNotificationService;
        private readonly IApplySettingsService _applySettingsService;

        public AppSettingsContentViewModel(ILogger<AppSettingsContentViewModel> logger,
            IUserStore userStore, IAppInNotificationService appInNotificationService,
            IApplySettingsService applySettingsService)
        {
            _logger = logger;
            _userStore = userStore;
            _appInNotificationService = appInNotificationService;
            _applySettingsService = applySettingsService;

            AppearanceTypes = new ObservableCollection<AppearanceType>(Enum.GetValues<AppearanceType>());
            CurrencyTypes = new ObservableCollection<BaseCurrencyType>(Enum.GetValues<BaseCurrencyType>());
            LanguageTypes = new ObservableCollection<LanguageType>(Enum.GetValues<LanguageType>());

            _userStore.UserChanged += OnUserChanged;
            LoadDataFromStore();
        }

        private void OnUserChanged()
        {
            LoadDataFromStore();
        }

        private void LoadDataFromStore()
        {
            IsLoading = true;
            if (_userStore.CurrentUser != null)
            {
                SelectedAppearanceType = _userStore.CurrentUser.Thema;
                SelectedCurrencyType = _userStore.CurrentUser.Currency;
                SelectedLanguageType = _userStore.CurrentUser.Language;
            }
            IsLoading = false;
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
                    Currency = SelectedCurrencyType,
                    Language = SelectedLanguageType,
                };

                bool success = await _userStore.UpdateAppSettingsAsync(settingsToUpdate);
                if (success)
                {
                    _logger.LogInformation("Application settings have been updated by the user.");
                    _appInNotificationService.ShowSuccess("Application settings saved successfully!");

                    _applySettingsService.AppearanceApply();
                    _applySettingsService.BaseCurrencyApply();
                    _applySettingsService.LanguageApply();
                }
                else
                {
                    _logger.LogError("Failed to save application settings via UserStore.");
                    _appInNotificationService.ShowError("An error occurred while saving your settings. Please try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occurred while saving application settings.");
                _appInNotificationService.ShowError("An unexpected error occurred. Please try again.");
            }
            finally
            {
                IsSaving = false;
            }
        }

        private bool CanSaveChanges() => !IsSaving;
    }
}