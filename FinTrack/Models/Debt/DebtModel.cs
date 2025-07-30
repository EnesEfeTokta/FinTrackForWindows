using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;
using System.Windows.Media;

namespace FinTrackForWindows.Models.Debt
{
    public partial class DebtModel : ObservableObject
    {
        public int Id { get; set; }
        public int LenderId { get; set; }
        public int BorrowerId { get; set; }
        public int CurrentUserId { get; set; }

        [ObservableProperty]
        private string lenderName = string.Empty;

        [ObservableProperty]
        private string borrowerName = string.Empty;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private DateTime dueDate;

        public string borrowerImageUrl = string.Empty;

        public string lenderImageUrl = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusText), nameof(StatusBrush), nameof(IsActionRequiredForBorrower), nameof(IsRejected))]
        private DebtStatusType status;

        public bool IsCurrentUserTheBorrower => BorrowerId == CurrentUserId;
        public bool IsCurrentUserTheLender => LenderId == CurrentUserId;

        public string DebtTitle => IsCurrentUserTheBorrower
            ? $"Alacaklı: {LenderName}"
            : $"Borçlu: {BorrowerName}";

        public string UserIconPath => IsCurrentUserTheBorrower
            ? "/Assets/Images/Icons/user-red.png"
            : "/Assets/Images/Icons/user-green.png";

        public Brush StatusBrush => Status switch
        {
            DebtStatusType.Active => new SolidColorBrush(Colors.Green),
            DebtStatusType.Defaulted => new SolidColorBrush(Colors.DarkRed),
            DebtStatusType.AcceptedPendingVideoUpload => new SolidColorBrush(Colors.CornflowerBlue),
            DebtStatusType.PendingBorrowerAcceptance => new SolidColorBrush(Colors.DodgerBlue),
            DebtStatusType.PendingOperatorApproval => new SolidColorBrush(Colors.Orange),
            DebtStatusType.RejectedByOperator => new SolidColorBrush(Colors.Red),
            DebtStatusType.RejectedByBorrower => new SolidColorBrush(Colors.Red),
            _ => new SolidColorBrush(Colors.Gray)
        };

        public string StatusText => Status switch
        {
            DebtStatusType.PendingBorrowerAcceptance => "Onayınız Bekleniyor",
            DebtStatusType.AcceptedPendingVideoUpload => "Video Yüklemesi Bekleniyor",
            DebtStatusType.PendingOperatorApproval => "Operatör Onayı Bekleniyor",
            DebtStatusType.Active => "Aktif",
            DebtStatusType.Paid => "Ödendi",
            DebtStatusType.Defaulted => "Vadesi Geçmiş",
            DebtStatusType.RejectedByBorrower => "Tarafınızdan Reddedildi",
            DebtStatusType.RejectedByOperator => "Operatör Tarafından Reddedildi",
            _ => "Bilinmeyen Durum"
        };

        // Borçlunun video yüklemesi gerekip gerekmediğini kontrol eder.
        public bool IsActionRequiredForBorrower =>
            Status == DebtStatusType.AcceptedPendingVideoUpload && IsCurrentUserTheBorrower;

        public bool IsRejected =>
            Status == DebtStatusType.RejectedByBorrower || Status == DebtStatusType.RejectedByOperator;
    }
}