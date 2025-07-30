using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.NotificationDtos;
using FinTrackForWindows.Models.Notification;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class NotificationViewModel : ObservableObject
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsListVisible))]
        [NotifyPropertyChangedFor(nameof(ShowEmptyMessage))]
        private ObservableCollection<NotificationModel> _notifications = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsListVisible))]
        [NotifyPropertyChangedFor(nameof(ShowEmptyMessage))]
        private bool _isLoading;

        public bool IsListVisible => !IsLoading && Notifications.Any();
        public bool ShowEmptyMessage => !IsLoading && !Notifications.Any();

        private readonly ILogger<NotificationViewModel> _logger;
        private readonly IApiService _apiService;

        public NotificationViewModel(ILogger<NotificationViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;

            _ = LoadNotifications();
        }

        [RelayCommand]
        private async Task LoadNotifications()
        {
            IsLoading = true;
            try
            {
                var notificationDtos = await _apiService.GetAsync<List<NotificationDto>>("Notification");

                if (notificationDtos != null)
                {
                    var models = notificationDtos.Select(dto => new NotificationModel(
                        dto.Id,
                        dto.MessageHead,
                        dto.MessageBody,
                        dto.CreatedAt.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                        dto.NotificationType,
                        dto.IsRead
                    ));
                    Notifications = new ObservableCollection<NotificationModel>(models);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load notifications.");
                MessageBox.Show("Could not load notifications. Please check your connection and try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Notifications.Clear();
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task MarkAllAsRead()
        {
            if (!Notifications.Any(n => n.IsUnread)) return;

            try
            {
                await _apiService.PostAsync<object>("Notification/mark-all-as-read", null);

                foreach (var notification in Notifications)
                {
                    if (notification.IsUnread)
                    {
                        notification.IsRead = true;
                    }
                }
                _logger.LogInformation("All notifications marked as read on server and client.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark all notifications as read.");
                MessageBox.Show("An error occurred. Could not mark all notifications as read.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task ClearAll()
        {
            if (!Notifications.Any()) return;

            try
            {
                await _apiService.DeleteAsync<object>("Notification/clear-all");
                Notifications.Clear();
                _logger.LogInformation("All notifications cleared from server and client.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to clear all notifications.");
                MessageBox.Show("An error occurred. Could not clear all notifications.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task MarkAsRead(NotificationModel? notification)
        {
            if (notification == null || notification.IsRead) return;

            try
            {
                await _apiService.PostAsync<object>($"Notification/mark-as-read/{notification.Id}", null);
                notification.IsRead = true;
                _logger.LogInformation($"Notification '{notification.Title}' marked as read on server and client.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark notification {NotificationId} as read.", notification.Id);
                MessageBox.Show("An error occurred. The notification could not be marked as read.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        [RelayCommand]
        private async Task DeleteNotification(NotificationModel? notification)
        {
            if (notification == null) return;

            try
            {
                await _apiService.DeleteAsync<object>($"Notification/{notification.Id}");
                Notifications.Remove(notification);
                _logger.LogInformation($"Notification '{notification.Title}' deleted from server and client.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete notification {NotificationId}.", notification.Id);
                MessageBox.Show("An error occurred. The notification could not be deleted.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}