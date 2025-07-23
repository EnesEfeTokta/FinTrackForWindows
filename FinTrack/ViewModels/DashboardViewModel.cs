using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Core;
using FinTrackForWindows.Dtos.BudgetDtos;
using FinTrackForWindows.Dtos.AccountDtos;
using FinTrackForWindows.Dtos.TransactionDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Dashboard;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace FinTrackForWindows.ViewModels
{
    public partial class DashboardViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<BudgetDashboardModel> _budgets_DashboardView_ItemsControl;

        [ObservableProperty]
        private ObservableCollection<CurrencyRateDashboard> _currencyRates_DashboardView_ItemsControl;

        [ObservableProperty]
        private ObservableCollection<AccountDashboard> _accounts_DashboardView_ItemsControl;

        [ObservableProperty]
        private string _totalBalance_DashboardView_TextBlock;

        [ObservableProperty]
        private ObservableCollection<TransactionDashboard> _transactions_DashboardView_ListView;

        [ObservableProperty]
        private MembershipDashboard _currentMembership_DashboardView_Multiple;

        [ObservableProperty]
        private DebtDashboard _currentDebt_DashboardView_Multiple;

        [ObservableProperty]
        private ObservableCollection<ReportDashboardModel> _reports_DashboardView_ItemsControl;

        [ObservableProperty]
        private string transactionSummary = string.Empty;

        private readonly ILogger<DashboardViewModel> _logger;

        private readonly IServiceProvider _serviceProvider;

        private readonly IApiService _apiService;

        public DashboardViewModel(
            ILogger<DashboardViewModel> logger, 
            IServiceProvider serviceProvider,
            IApiService apiService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _apiService = apiService;

            if (SessionManager.IsLoggedIn)
            {
                _logger.LogInformation("Kullanıcı zaten giriş yapmış. DashboardViewModel verileri yüklüyor.");
                _ = LoadBudgetData();
                _ = LoadAccountData();
                _ = LoadTransactionData();
            }
        }

        private async Task LoadData()
        {
            // [TEST]
            CurrencyRates_DashboardView_ItemsControl = new ObservableCollection<CurrencyRateDashboard>
            {
                new CurrencyRateDashboard
                {
                    FromCurrencyFlagUrl = "https://flagcdn.com/w320/us.png", FromCurrencyCountry = "Amerika Birleşik Devletleri", FromCurrencyName = "Dolar", FromCurrencyAmount = "1.00",
                    ToCurrencyFlagUrl = "https://flagcdn.com/w320/tr.png", ToCurrencyCountry = "Türkiye", ToCurrencyName = "Türk Lirası", ToCurrencyAmount = "27.50", ToCurrencyImageHeight = 20
                },
                new CurrencyRateDashboard
                {
                    FromCurrencyFlagUrl = "https://flagcdn.com/w320/eu.png", FromCurrencyCountry = "Euro Bölgesi", FromCurrencyName = "Euro",FromCurrencyAmount = "1.00",
                    ToCurrencyFlagUrl = "https://flagcdn.com/w320/tr.png", ToCurrencyCountry = "Türkiye", ToCurrencyName = "Türk Lirası", ToCurrencyAmount = "30.00", ToCurrencyImageHeight = 20
                }
            };

            // [TEST]
            CurrentMembership_DashboardView_Multiple = new MembershipDashboard { Level = "Pro | AKTF", StartDate = "01.01.2025", RenewalDate = "01.02.2025", Price = "9.99$" };

            // [TEST]
            CurrentDebt_DashboardView_Multiple = new DebtDashboard
            {
                LenderName = "Ali Veli",
                LenderIconPath = "https://i.pinimg.com/236x/be/a3/49/bea3491915571d34a026753f4a872000.jpg",
                BorrowerName = "Ahmet Mehmet",
                BorrowerIconPath = "https://pbs.twimg.com/profile_images/1144861916734451712/D76C3ugh_400x400.jpg",
                Status = "Ödenmemiş",
                StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush"),
                Amount = "1.000$",
                CreationDate = "01.01.2025",
                DueDate = "01.02.2025",
                ReviewDate = "01.03.2025"
            };

            // [TEST]
            Reports_DashboardView_ItemsControl = new ObservableCollection<ReportDashboardModel>
            {
                CreateReport("2025 Yılı Finansal Raporu"),
                CreateReport("2024 Yılı Tasarruf Raporu"),
                CreateReport("2023 Yılı Gelir-Gider Raporu"),
                CreateReport("2022 Yılı Bütçe Raporu")
            };
        }

        private ReportDashboardModel CreateReport(string name)
        {
            var reportLogger = _serviceProvider.GetRequiredService<ILogger<ReportDashboardModel>>();
            var report = new ReportDashboardModel(reportLogger)
            {
                Name = name,
            };
            return report;
        }

        private async Task LoadBudgetData()
        {
            var budgets = await _apiService.GetAsync<List<BudgetDto>>("Budgets");
            Budgets_DashboardView_ItemsControl = new ObservableCollection<BudgetDashboardModel>();
            if (budgets != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    Budgets_DashboardView_ItemsControl.Add(new BudgetDashboardModel
                    {
                        Name = budgets[i].Name,
                        DueDate = budgets[i].EndDate.ToString("dd.MM.yyyy"),
                        Amount = $"{budgets[i].AllocatedAmount} {budgets[i].Currency}",
                        RemainingTime = $"Son {(budgets[i].EndDate - budgets[i].StartDate).Days} gün kaldı.",
                        StatusBrush = (Brush)Application.Current.FindResource("StatusGreenBrush")
                    });
                }
            }
        }

        private async Task LoadAccountData()
        {
            var accounts = await _apiService.GetAsync<List<AccountDto>>("Account");
            Accounts_DashboardView_ItemsControl = new ObservableCollection<AccountDashboard>();
            if (accounts != null)
            {
                decimal totalBalance = 0;
                for (int i = 0; i < 2; i++)
                {
                    Accounts_DashboardView_ItemsControl.Add(new AccountDashboard 
                    {
                        Name = accounts[i].Name,
                        Balance = $"{accounts[i].Balance} {accounts[i].Currency}",
                        Percentage = 70, // Burası yüzdelik bar olduğu için daha iyi bir hesaplama yapılmalı.
                        ProgressBarBrush = (Brush)Application.Current.FindResource("StatusGreenBrush")
                    });

                    totalBalance += accounts[i].Balance;
                }

                TotalBalance_DashboardView_TextBlock = $"{totalBalance} {accounts[0].Currency}";
            }
        }

        private async Task LoadTransactionData()
        {
            var transactions = await _apiService.GetAsync<List<TransactionDto>>("Transactions");
            Transactions_DashboardView_ListView = new ObservableCollection<TransactionDashboard>();
            if (transactions != null)
            {
                foreach (var item in transactions)
                {
                    Transactions_DashboardView_ListView.Add(new TransactionDashboard
                    {
                        DateText = item.TransactionDateUtc.ToString("dd.MM.yyyy"),
                        Description = item.Description ?? "N/A",
                        Amount = $"{item.Amount} {item.Currency}",
                        Category = item.Category.Name,
                        Type = item.Category.Type
                    });
                }

                double totalIncome = Transactions_DashboardView_ListView
                    .Where(t => t.Type == TransactionType.Income)
                    .Sum(t =>
                    {
                        var cleaned = t.Amount.Replace("+", string.Empty).Replace("$", string.Empty).Trim();
                        return double.TryParse(cleaned, out var value) ? value : 0;
                    });
                double totalExpense = Transactions_DashboardView_ListView
                    .Where(t => t.Type == TransactionType.Expense)
                    .Sum(t =>
                    {
                        var cleaned = t.Amount.Replace("-", string.Empty).Replace("$", string.Empty).Trim();
                        return double.TryParse(cleaned, out var value) ? value : 0;
                    });
                double remainingBalance = totalIncome - totalExpense;

                TransactionSummary = $"Toplam {Transactions_DashboardView_ListView.Count} işlem bulundu. Gelir: +{totalIncome} {transactions[0].Currency}, Gider: -{totalExpense} {transactions[0].Currency} Kalan: {remainingBalance} {transactions[0].Currency}";
            }
        }
    }
}
