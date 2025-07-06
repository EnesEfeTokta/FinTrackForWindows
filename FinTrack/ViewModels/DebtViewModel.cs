using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrack.Enums;
using FinTrack.Models.Debt;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrack.ViewModels
{
    public partial class DebtViewModel : ObservableObject
    {
        private const string CurrentUserName = "Siz";

        private readonly ILogger<DebtViewModel> _logger;

        private ObservableCollection<DebtModel> allDebts;

        [ObservableProperty]
        private string? newProposalBorrowerEmail;

        [ObservableProperty]
        private decimal newProposalAmount;

        [ObservableProperty]
        private DateTime newProposalDueDate = DateTime.Now.AddMonths(1);

        [ObservableProperty]
        private ObservableCollection<DebtModel> pendingOffers;

        [ObservableProperty]
        private ObservableCollection<DebtModel> myDebtsList;

        public DebtViewModel(ILogger<DebtViewModel> logger)
        {
            _logger = logger;
            allDebts = new ObservableCollection<DebtModel>();
            pendingOffers = new ObservableCollection<DebtModel>();
            myDebtsList = new ObservableCollection<DebtModel>();

            LoadSampleData();
            RefreshLists();
        }

        [RelayCommand]
        private void SendOffer()
        {
            if (string.IsNullOrWhiteSpace(NewProposalBorrowerEmail) || NewProposalAmount <= 0)
            {
                MessageBox.Show("Lütfen tüm alanları doldurun.", "Doğrulama Hatası");
                return;
            }

            var newDebt = new DebtModel
            {
                LenderName = CurrentUserName,
                BorrowerName = "Unknown",
                Amount = NewProposalAmount,
                DueDate = NewProposalDueDate,
                Status = DebtStatus.PendingProposal,
                CurrentUserName = CurrentUserName
            };

            allDebts.Add(newDebt);
            RefreshLists();

            NewProposalBorrowerEmail = string.Empty;
            NewProposalAmount = 0;
            _logger.LogInformation("Yeni borç teklifi gönderildi.");
        }

        [RelayCommand]
        private void ConfirmOffer(DebtModel debt)
        {
            if (debt == null) return;

            debt.Status = DebtStatus.AwaitingVideoUpload;
            debt.BorrowerName = CurrentUserName;
            RefreshLists();
            _logger.LogInformation("{Amount} TRY tutarındaki borç teklifi onaylandı.", debt.Amount);
        }

        [RelayCommand]
        private void RejectOffer(DebtModel debt)
        {
            if (debt == null) return;

            debt.Status = DebtStatus.RejectedByBorrower;
            RefreshLists();
            _logger.LogWarning("{Amount} TRY tutarındaki borç teklifi reddedildi.", debt.Amount);
        }

        [RelayCommand]
        private void UploadVideo(DebtModel debt)
        {
            if (debt == null) return;

            debt.Status = DebtStatus.AwaitingOperatorApproval;
            RefreshLists();
            _logger.LogInformation("{Amount} TRY borcu için onay videosu yüklendi, operatör onayı bekleniyor.", debt.Amount);
            MessageBox.Show("Video başarıyla yüklendi. Operatör onayı bekleniyor.", "Başarılı");
        }

        private void RefreshLists()
        {
            var pending = allDebts.Where(d => d.Status == DebtStatus.PendingProposal && d.LenderName != CurrentUserName).ToList();
            PendingOffers.Clear();
            foreach (var item in pending) PendingOffers.Add(item);

            var myDebts = allDebts.Where(d => d.Status != DebtStatus.PendingProposal && (d.LenderName == CurrentUserName || d.BorrowerName == CurrentUserName)).ToList();
            MyDebtsList.Clear();
            foreach (var item in myDebts) MyDebtsList.Add(item);
        }

        private void LoadSampleData()
        {
            allDebts = new ObservableCollection<DebtModel>
            {
                new DebtModel 
                { 
                    LenderName = "Ahmet Yılmaz", 
                    BorrowerName = "Unknown", 
                    Amount = 500, 
                    DueDate = new DateTime(2024, 6, 15), 
                    Status = DebtStatus.PendingProposal, 
                    CurrentUserName = CurrentUserName 
                },
                new DebtModel 
                { 
                    LenderName = CurrentUserName, 
                    BorrowerName = "Eylül Korkmaz", 
                    Amount = 2500, 
                    DueDate = new DateTime(2024, 10, 1), 
                    Status = DebtStatus.AwaitingVideoUpload, 
                    CurrentUserName = CurrentUserName 
                },
                new DebtModel 
                { 
                    LenderName = "Sinem Berçem", 
                    BorrowerName = CurrentUserName, 
                    Amount = 30000, 
                    DueDate = new DateTime(2024, 8, 31), 
                    Status = DebtStatus.Active, 
                    CurrentUserName = CurrentUserName 
                },
                new DebtModel 
                { 
                    LenderName = CurrentUserName, 
                    BorrowerName = "Ali Veli", 
                    Amount = 800000, 
                    DueDate = new DateTime(2025, 1, 1), 
                    Status = DebtStatus.AwaitingOperatorApproval, 
                    CurrentUserName = CurrentUserName 
                },
                new DebtModel 
                { 
                    LenderName = CurrentUserName, 
                    BorrowerName = "Canan Aslan", 
                    Amount = 1000, 
                    DueDate = new DateTime(2024, 9, 20), 
                    Status = DebtStatus.RejectedByOperator, 
                    RejectionReason = "Insufficient video", 
                    CurrentUserName = CurrentUserName 
                }
            };
            _logger.LogInformation("Örnek borç verileri yüklendi.");
        }
    }
}