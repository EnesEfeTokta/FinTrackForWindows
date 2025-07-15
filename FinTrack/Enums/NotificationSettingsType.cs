using System.ComponentModel;

namespace FinTrackForWindows.Enums
{
    public enum NotificationSettingsType
    {
        [Description("Spending limit warning")]
        SpendingLimitWarning,

        [Description("Expected bill reminder")]
        ExpectedBillReminder,

        [Description("Weekly spending summary")]
        WeeklySpendingSummary,

        [Description("New features and announcements")]
        NewFeaturesAndAnnouncements
    }
}
