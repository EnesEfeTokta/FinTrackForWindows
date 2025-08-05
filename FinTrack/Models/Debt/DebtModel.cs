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
        [NotifyPropertyChangedFor(nameof(CanMarkAsDefaulted))]
        private DateTime dueDate;

        public string borrowerImageUrl = string.Empty;

        public string lenderImageUrl = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusText))]
        [NotifyPropertyChangedFor(nameof(StatusBrush))]
        [NotifyPropertyChangedFor(nameof(IsActionRequiredForBorrower))]
        [NotifyPropertyChangedFor(nameof(IsRejected))]
        [NotifyPropertyChangedFor(nameof(IsVideoViewableForLender))]
        [NotifyPropertyChangedFor(nameof(CanMarkAsDefaulted))]
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

        /// <summary>
        /// "Teminat Videosunu İzle" düğmesinin görünür olup olmayacağını belirler.
        /// </summary>
        public bool IsVideoViewableForLender =>
            Status == DebtStatusType.Defaulted &&    // Durum 'Defaulted' olmalı
            IsCurrentUserTheLender &&                // Kullanıcı borç veren olmalı
            VideoMetadataId.HasValue;                // İlişkili bir video olmalı

        /// <summary>
        /// "Temerrüde Düştü Olarak İşaretle" düğmesinin görünür olup olmayacağını belirleyecek.
        /// </summary>
        public bool CanMarkAsDefaulted =>
            Status == DebtStatusType.Active &&       // Durum 'Active' olmalı
            IsCurrentUserTheLender &&                // Kullanıcı borç veren olmalı
            DueDate < DateTime.Now;                  // Vadesi geçmiş olmalı
    }
}