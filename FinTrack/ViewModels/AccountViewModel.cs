﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.AccountDtos;
using FinTrackForWindows.Dtos.TransactionDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Account;
using FinTrackForWindows.Services.Accounts;
using FinTrackForWindows.Services.Api;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Windows;

namespace FinTrackForWindows.ViewModels
{
    public partial class AccountViewModel : ObservableObject
    {
        public ReadOnlyObservableCollection<AccountModel> Accounts => _accountStore.Accounts;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(FormTitle))]
        [NotifyPropertyChangedFor(nameof(SaveButtonText))]
        private AccountModel? selectedAccount;

        public ObservableCollection<BaseCurrencyType> CurrencyTypes { get; }

        public ObservableCollection<AccountType> AccountTypes { get; }

        private readonly ILogger<AccountViewModel> _logger;

        private readonly IAccountStore _accountStore;

        public string FormTitle => IsEditing ? "Hesabı Düzenle" : "Yeni Hesap Ekle";
        public string SaveButtonText => IsEditing ? "GÜNCELLE" : "HESAP OLUŞTUR";

        private bool IsEditing = false;

        private readonly IApiService _apiService;

        [ObservableProperty]
        private ISeries[] series = new ISeries[0];

        [ObservableProperty]
        private Axis[] xAxes = new Axis[0];

        [ObservableProperty]
        private Axis[] yAxes = new Axis[0];

        [ObservableProperty]
        private LabelVisual title = new LabelVisual { /* Başlık için yer tutucu */ };

        public AccountViewModel(ILogger<AccountViewModel> logger, IApiService apiService, IAccountStore accountStore)
        {
            _logger = logger;
            _apiService = apiService;
            _accountStore = accountStore;

            InitializeEmptyChart();
            _ = LoadData();
            _ = InitializeViewModel();
            PrepareForNewAccount();

            CurrencyTypes = new ObservableCollection<BaseCurrencyType>();
            AccountTypes = new ObservableCollection<AccountType>();

            CurrencyTypes.Clear();
            foreach (BaseCurrencyType currencyType in Enum.GetValues(typeof(BaseCurrencyType)))
            {
                CurrencyTypes.Add(currencyType);
            }

            AccountTypes.Clear();
            foreach (AccountType accountType in Enum.GetValues(typeof(AccountType)))
            {
                AccountTypes.Add(accountType);
            }
        }

        private void InitializeEmptyChart()
        {
            Series = new ISeries[0];
            Title = new LabelVisual
            {
                Text = "Select an account to view the data.",
                TextSize = 18,
                Paint = new SolidColorPaint(SKColors.Gray),
                Padding = new LiveChartsCore.Drawing.Padding(15)
            };
        }

        private async Task InitializeViewModel()
        {
            await LoadData();
            if (Accounts.Any())
            {
                var firstAccount = Accounts.First();
                if (firstAccount.Id.HasValue)
                {
                    SelectedAccount = firstAccount;
                }
            }
        }

        partial void OnSelectedAccountChanged(AccountModel? value)
        {
            _logger.LogInformation("Seçilen hesap değişti: {AccountName}", value?.Name ?? "Hiçbiri");
            IsEditing = value != null && value.Id != null;
            OnPropertyChanged(nameof(FormTitle));
            OnPropertyChanged(nameof(SaveButtonText));

            if (value != null && value.Id.HasValue)
            {
                _ = LoadTransactionHistory(value.Id.Value, value.Name);
            }
            else
            {
                InitializeEmptyChart();
            }
        }

        private async Task LoadData()
        {
            await _accountStore.LoadAccountsAsync();
        }

        private async Task LoadTransactionHistory(int accountId, string accountName)
        {
            var transactions = await _apiService.GetAsync<List<TransactionDto>>($"Transactions/account-id/{accountId}");

            if (transactions == null || !transactions.Any())
            {
                Series = new ISeries[0];
                Title = new LabelVisual
                {
                    Text = "No Data",
                    TextSize = 18,
                    Paint = new SolidColorPaint(SKColors.Gray),
                    Padding = new LiveChartsCore.Drawing.Padding(15)
                };
                return;
            }

            var incomePoints = transactions
                .Where(t => t.Category.Type == TransactionType.Income)
                .Select(t => new DateTimePoint(t.TransactionDateUtc, (double)t.Amount))
                .ToList();

            var expensePoints = transactions
                .Where(t => t.Category.Type == TransactionType.Expense)
                .Select(t => new DateTimePoint(t.TransactionDateUtc, (double)t.Amount))
                .ToList();

            Series = new ISeries[]
            {
                new LineSeries<DateTimePoint>
                {
                    Name = "Income",
                    Values = incomePoints,
                    Stroke = new SolidColorPaint(SKColors.MediumSpringGreen) { StrokeThickness = 2 },
                    GeometryStroke = new SolidColorPaint(SKColors.MediumSpringGreen) { StrokeThickness = 4 },
                    Fill = new LinearGradientPaint(SKColors.MediumSpringGreen.WithAlpha(90), SKColors.MediumSpringGreen.WithAlpha(10), new SKPoint(0.5f, 0), new SKPoint(0.5f, 1))
                },
                new LineSeries<DateTimePoint>
                {
                    Name = "Expense",
                    Values = expensePoints,
                    Stroke = new SolidColorPaint(SKColors.IndianRed) { StrokeThickness = 2 },
                    GeometryStroke = new SolidColorPaint(SKColors.IndianRed) { StrokeThickness = 4 },
                    Fill = new LinearGradientPaint(SKColors.IndianRed.WithAlpha(90), SKColors.IndianRed.WithAlpha(10), new SKPoint(0.5f, 0), new SKPoint(0.5f, 1))
                }
            };

            Title = new LabelVisual
            {
                Text = $"{accountName} - Transactions",
                TextSize = 16,
                Paint = new SolidColorPaint(SKColors.WhiteSmoke),
                Padding = new LiveChartsCore.Drawing.Padding(15)
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value =>
                    {
                        try
                        {
                            var ticks = (long)value;
                            if (ticks >= DateTime.MinValue.Ticks && ticks <= DateTime.MaxValue.Ticks)
                            {
                                return new DateTime(ticks).ToString("dd MMM");
                            }
                            return string.Empty;
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    }
,
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    UnitWidth = TimeSpan.FromDays(1).Ticks,
                    MinStep = TimeSpan.FromDays(1).Ticks

                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => value.ToString("C0"),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 0.5f }
                }
            };
        }

        [RelayCommand]
        private async Task SaveAccount()
        {
            if (SelectedAccount == null || string.IsNullOrWhiteSpace(SelectedAccount.Name)) return;

            var accountDto = new AccountCreateDto
            {
                Name = SelectedAccount.Name,
                Type = SelectedAccount.Type,
                Currency = SelectedAccount.Currency,
            };

            if (IsEditing && SelectedAccount.Id.HasValue)
            {
                int accountId = SelectedAccount.Id.Value;
                string accountName = SelectedAccount.Name;

                await _accountStore.UpdateAccountAsync(SelectedAccount.Id ?? -1, accountDto);

                await LoadTransactionHistory(accountId, accountName);
            }
            else
            {
                await _accountStore.AddAccountAsync(accountDto);
            }

            PrepareForNewAccount();
        }

        [RelayCommand]
        private async Task DeleteAccount(AccountModel? accountToDelete)
        {
            if (accountToDelete == null) return;

            var result = MessageBox.Show($"'{accountToDelete.Name}' adlı hesabı silmek istediğinizden emin misiniz?", "Silme Onayı", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No) return;

            if (accountToDelete != null)
            {
                await _accountStore.DeleteAccountAsync(SelectedAccount?.Id ?? -1);

                if (SelectedAccount?.Id == accountToDelete.Id)
                {
                    PrepareForNewAccount();
                }
            }
        }

        [RelayCommand]
        private void PrepareToEditAccount(AccountModel accountToEdit)
        {
            if (accountToEdit == null) return;
            SelectedAccount = accountToEdit;
        }

        private void PrepareForNewAccount()
        {
            SelectedAccount = new AccountModel();
        }
    }
}