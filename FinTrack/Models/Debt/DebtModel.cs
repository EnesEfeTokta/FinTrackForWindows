using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;
using System.Windows.Media;

namespace FinTrackForWindows.Models.Debt
{
    public partial class DebtModel : ObservableObject
    {
        [ObservableProperty]
        private string lenderName = string.Empty;

        [ObservableProperty]
        private string borrowerName = string.Empty;

        [ObservableProperty]
        private string borrowerEmail = string.Empty;

        [ObservableProperty]
        private decimal amount;

        [ObservableProperty]
        private DateTime dueDate;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(StatusText))]
        [NotifyPropertyChangedFor(nameof(StatusBrush))]
        [NotifyPropertyChangedFor(nameof(IsActionRequired))]
        [NotifyPropertyChangedFor(nameof(IsRejected))]
        private DebtStatusType status;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DebtTitle))]
        [NotifyPropertyChangedFor(nameof(UserIconPath))]
        [NotifyPropertyChangedFor(nameof(IsCurrentUserTheBorrower))]
        private string currentUserName = string.Empty;

        public string DebtTitle => IsCurrentUserTheBorrower ? $"Your debt: {LenderName}" : $"Debt owed to you: {BorrowerName}";

        public string UserIconPath => IsCurrentUserTheBorrower ? "/Assets/Images/Icons/user-red.png" : "/Assets/Images/Icons/user-green.png";

        public bool IsCurrentUserTheBorrower => BorrowerName == CurrentUserName;

        [ObservableProperty]
        private string? rejectionReason;

        [ObservableProperty]
        private DateTime createdDate = DateTime.Now;

        private static readonly Brush GreenBrush = new SolidColorBrush(Colors.Green);
        private static readonly Brush RedBrush = new SolidColorBrush(Colors.Red);
        private static readonly Brush BlueBrush = new SolidColorBrush(Colors.Blue);
        private static readonly Brush OrangeBrush = new SolidColorBrush(Colors.Orange);
        private static readonly Brush GrayBrush = new SolidColorBrush(Colors.Gray);

        public Brush StatusBrush => Status switch
        {
            DebtStatusType.Active => GreenBrush,
            DebtStatusType.PendingBorrowerAcceptance => BlueBrush,
            DebtStatusType.PendingOperatorApproval => OrangeBrush,
            DebtStatusType.RejectedByOperator => RedBrush,
            DebtStatusType.RejectedByBorrower => RedBrush,
            _ => GrayBrush
        };

        public string StatusText => Status switch
        {
            DebtStatusType.PendingBorrowerAcceptance => "Status: Awaiting Video Approval",
            DebtStatusType.PendingOperatorApproval => "Status: FinTrack Operator Approval Pending",
            DebtStatusType.Active => "Status: Active - In force",
            DebtStatusType.RejectedByOperator => "Status: Rejected by Operator",
            DebtStatusType.RejectedByBorrower => "Status: Rejected by Borrower",
            _ => "Status: Unknown"
        };

        public bool IsActionRequired => Status == DebtStatusType.PendingBorrowerAcceptance;

        public bool IsRejected => Status == DebtStatusType.RejectedByBorrower || Status == DebtStatusType.RejectedByOperator;

        public string InfoText => Status switch
        {
            DebtStatusType.Active => $"Final Payment: {DueDate:dd.MM.yyyy}",
            DebtStatusType.PendingOperatorApproval => "Video uploaded",
            DebtStatusType.RejectedByOperator => $"Reason: {RejectionReason}",
            _ => string.Empty
        };
    }
}
