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
        public int? VideoMetadataId { get; set; }

        [ObservableProperty]
        private string lenderName = string.Empty;

        [ObservableProperty]
        private string borrowerName = string.Empty;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private BaseCurrencyType currency;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DaysUntilDue))]
        [NotifyPropertyChangedFor(nameof(CanMarkAsDefaulted))]
        private DateTime dueDate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CounterPartyImageUrl))]
        private string lenderImageUrl = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CounterPartyImageUrl))]
        private string borrowerImageUrl = string.Empty;

        [ObservableProperty]
        private string lenderEmail = string.Empty;

        [ObservableProperty]
        private string borrowerEmail = string.Empty;

        [ObservableProperty]
        private string? description;

        [ObservableProperty]
        private DateTime? acceptanceDate;

        [ObservableProperty]
        private DateTime? operatorApprovalDate;

        [ObservableProperty]
        private DateTime? paymentDate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusText))]
        [NotifyPropertyChangedFor(nameof(StatusBrush))]
        [NotifyPropertyChangedFor(nameof(IsActionRequiredForBorrower))]
        [NotifyPropertyChangedFor(nameof(IsRejected))]
        [NotifyPropertyChangedFor(nameof(IsVideoViewableForLender))]
        [NotifyPropertyChangedFor(nameof(CanMarkAsDefaulted))]
        private DebtStatusType status;

        [ObservableProperty]
        private bool isExpanded;

        public bool IsCurrentUserTheBorrower => BorrowerId == CurrentUserId;
        public bool IsCurrentUserTheLender => LenderId == CurrentUserId;

        public int DaysUntilDue => (DueDate.Date - DateTime.Now.Date).Days;

        public string CounterPartyName => IsCurrentUserTheBorrower ? LenderName : BorrowerName;
        public string CounterPartyImageUrl => IsCurrentUserTheBorrower ? LenderImageUrl : BorrowerImageUrl;

        public Brush StatusBrush => Status switch
        {
            DebtStatusType.Active => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4CAF50")), // Green
            DebtStatusType.Defaulted => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#B71C1C")), // Dark Red
            DebtStatusType.AcceptedPendingVideoUpload => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E88E5")), // Blue
            DebtStatusType.PendingBorrowerAcceptance => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#03A9F4")), // Light Blue
            DebtStatusType.PendingOperatorApproval => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FB8C00")), // Orange
            DebtStatusType.RejectedByOperator => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E53935")), // Red
            DebtStatusType.RejectedByBorrower => new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E53935")), // Red
            DebtStatusType.Paid => new SolidColorBrush(Colors.Gray),
            _ => new SolidColorBrush(Colors.Gray)
        };

        public string StatusText => Status switch
        {
            DebtStatusType.PendingBorrowerAcceptance => "Onayınız Bekleniyor",
            DebtStatusType.AcceptedPendingVideoUpload => "Video Yüklemesi Bekleniyor",
            DebtStatusType.PendingOperatorApproval => "Operatör Onayı Bekleniyor",
            DebtStatusType.Active => "Aktif",
            DebtStatusType.Paid => "Ödendi",
            DebtStatusType.Defaulted => "Vadesi Geçmiş - Temerrüt",
            DebtStatusType.RejectedByBorrower => "Tarafınızdan Reddedildi",
            DebtStatusType.RejectedByOperator => "Operatör Tarafından Reddedildi",
            _ => "Bilinmeyen Durum"
        };

        public bool IsActionRequiredForBorrower =>
            Status == DebtStatusType.AcceptedPendingVideoUpload && IsCurrentUserTheBorrower;

        public bool IsRejected =>
            Status == DebtStatusType.RejectedByBorrower || Status == DebtStatusType.RejectedByOperator;

        public bool IsVideoViewableForLender =>
            Status == DebtStatusType.Defaulted &&
            IsCurrentUserTheLender &&
            VideoMetadataId.HasValue;

        public bool CanMarkAsDefaulted =>
            Status == DebtStatusType.Active &&
            IsCurrentUserTheLender &&
            DueDate.Date < DateTime.Now.Date;
    }
}