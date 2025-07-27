using CommunityToolkit.Mvvm.ComponentModel;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Currency;
using FinTrackForWindows.Services.Api;
using FinTrackWebApi.Dtos.CurrencyDtos;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.VisualElements;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.Globalization;

namespace FinTrackForWindows.ViewModels
{
    public partial class CurrenciesViewModel : ObservableObject
    {
        private ObservableCollection<CurrencyModel> allCurrencies = new();

        [ObservableProperty]
        private ObservableCollection<CurrencyModel> filteredCurrencies = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsCurrencySelected))]
        private CurrencyModel? selectedCurrency;

        [ObservableProperty]
        private string currencySearch = string.Empty;

        [ObservableProperty]
        private bool isLoadingDetails = false;

        public bool IsCurrencySelected => SelectedCurrency != null;

        [ObservableProperty]
        private ISeries[] series = new ISeries[0];

        [ObservableProperty]
        private Axis[] xAxes = new Axis[0];

        [ObservableProperty]
        private Axis[] yAxes = new Axis[0];

        [ObservableProperty]
        private LabelVisual title = new();

        private readonly ILogger<CurrenciesViewModel> _logger;
        private readonly IApiService _apiService;
        private static readonly SKColor _chartColor = SKColor.Parse("#3498DB");

        public CurrenciesViewModel(ILogger<CurrenciesViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
            InitializeEmptyChart();
            _ = LoadCurrenciesData();
        }

        partial void OnCurrencySearchChanged(string value)
        {
            FilterCurrencies();
        }

        async partial void OnSelectedCurrencyChanged(CurrencyModel? value)
        {
            if (value == null)
            {
                InitializeEmptyChart();
                return;
            }
            await LoadHistoricalDataAsync(value.ToCurrencyCode);
        }

        private async Task LoadCurrenciesData()
        {
            try
            {
                var response = await _apiService.GetAsync<LatestRatesResponseDto>("Currency/latest/USD");
                if (response?.Rates != null)
                {
                    allCurrencies.Clear();
                    foreach (var item in response.Rates)
                    {
                        allCurrencies.Add(new CurrencyModel
                        {
                            Id = item.Id,
                            ToCurrencyCode = item.Code,
                            ToCurrencyName = item.CountryCode ?? "N/A",
                            ToCurrencyFlag = item.IconUrl ?? string.Empty,
                            ToCurrencyPrice = item.Rate
                        });
                    }

                    FilterCurrencies();

                    if (FilteredCurrencies.Any())
                    {
                        SelectedCurrency = FilteredCurrencies.First();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Para birimi listesi yüklenirken hata oluştu.");
                InitializeEmptyChart("Failed to load currency list.");
            }
        }

        private async Task LoadHistoricalDataAsync(string targetCurrencyCode, string period = "1M")
        {
            IsLoadingDetails = true;
            try
            {
                string baseCurrency = "USD";
                string endpoint = $"Currency/{baseCurrency}/history/{targetCurrencyCode}?period={period}";
                var historyData = await _apiService.GetAsync<CurrencyHistoryDto>(endpoint);

                if (historyData == null || SelectedCurrency == null || !historyData.HistoricalRates.Any())
                {
                    InitializeEmptyChart($"No data available for {targetCurrencyCode}.");
                    UpdateDetails(null);
                    return;
                }

                _logger.LogInformation("{Target} için geçmiş veriler yüklendi.", targetCurrencyCode);
                UpdateChart(historyData);
                UpdateDetails(historyData.ChangeSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Target} için geçmiş veriler yüklenirken hata oluştu.", targetCurrencyCode);
                InitializeEmptyChart($"Error loading data for {targetCurrencyCode}.");
                UpdateDetails(null);
            }
            finally
            {
                IsLoadingDetails = false;
            }
        }

        private void UpdateChart(CurrencyHistoryDto historyData)
        {
            var historicalPoints = historyData.HistoricalRates
                .Select(p => new DateTimePoint(p.Date, (double)p.Rate))
                .OrderBy(p => p.DateTime)
                .ToList();

            Series = new ISeries[]
            {
                new LineSeries<DateTimePoint>
                {
                    Values = historicalPoints,
                    Name = $"{historyData.BaseCurrency}/{historyData.TargetCurrency}",
                    Stroke = new SolidColorPaint(_chartColor) { StrokeThickness = 3 },
                    GeometrySize = 8,
                    GeometryStroke = new SolidColorPaint(_chartColor) { StrokeThickness = 3 },
                    GeometryFill = new SolidColorPaint(SKColor.Parse("#1E222D")),
                    Fill = new LinearGradientPaint(_chartColor.WithAlpha(90), SKColors.Transparent, new SKPoint(0.5f, 0), new SKPoint(0.5f, 1))
                }
            };

            Title = new LabelVisual
            {
                Text = $"{historyData.TargetCurrency} Exchange Rate (Last 30 Days)",
                TextSize = 16,
                Padding = new Padding(15),
                Paint = new SolidColorPaint(SKColors.WhiteSmoke),
            };

            XAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value =>
                    {
                        try
                        {
                            var date = DateTime.FromOADate(value);
                            return date.ToString("dd MMM");
                        }
                        catch
                        {
                            return string.Empty;
                        }
                    },
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    UnitWidth = TimeSpan.FromDays(1).Ticks,
                    MinStep = TimeSpan.FromDays(1).Ticks,
                    SeparatorsPaint = new SolidColorPaint(SKColors.Transparent)
                }
            };

            YAxes = new Axis[]
            {
                new Axis
                {
                    Labeler = value => value.ToString("N4", CultureInfo.InvariantCulture),
                    LabelsPaint = new SolidColorPaint(SKColors.LightGray),
                    SeparatorsPaint = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 0.5f, PathEffect = new DashEffect(new float[] { 3, 3 }) }
                }
            };
        }

        private void UpdateDetails(ChangeSummaryDto? summary)
        {
            if (SelectedCurrency == null) return;

            if (summary == null)
            {
                SelectedCurrency.DailyLow = "N/A";
                SelectedCurrency.DailyHigh = "N/A";
                SelectedCurrency.WeeklyChange = "N/A";
                SelectedCurrency.MonthlyChange = "N/A";
                SelectedCurrency.Type = CurrencyConversionType.Neutral;
                SelectedCurrency.WeeklyChangeType = CurrencyConversionType.Neutral;
                SelectedCurrency.MonthlyChangeType = CurrencyConversionType.Neutral;
                return;
            }

            SelectedCurrency.DailyLow = "N/A";
            SelectedCurrency.DailyHigh = "N/A";

            (SelectedCurrency.WeeklyChange, SelectedCurrency.WeeklyChangeType) = FormatChangeValue(summary.WeeklyChangePercentage);
            (SelectedCurrency.MonthlyChange, SelectedCurrency.MonthlyChangeType) = FormatChangeValue(summary.MonthlyChangePercentage);
            (SelectedCurrency.ToCurrencyChange, SelectedCurrency.Type) = FormatChangeValue(summary.DailyChangePercentage);
        }

        private void InitializeEmptyChart(string message = "Select a currency to view its data.")
        {
            Series = new ISeries[0];
            XAxes = new Axis[0];
            YAxes = new Axis[0];
            Title = new LabelVisual
            {
                Text = message,
                TextSize = 18,
                Paint = new SolidColorPaint(SKColors.Gray),
                Padding = new LiveChartsCore.Drawing.Padding(15),
            };
        }

        private (string FormattedValue, CurrencyConversionType Type) FormatChangeValue(decimal? value, bool isPercentage = true, string format = "P2")
        {
            if (!value.HasValue) return ("N/A", CurrencyConversionType.Neutral);

            string formattedValue = (value.Value / (isPercentage ? 1m : 1m)).ToString(format, CultureInfo.InvariantCulture);
            CurrencyConversionType type;

            if (value > 0)
            {
                formattedValue = "+" + formattedValue;
                type = CurrencyConversionType.Increase;
            }
            else if (value < 0)
            {
                type = CurrencyConversionType.Decrease;
            }
            else
            {
                type = CurrencyConversionType.Neutral;
            }
            return (formattedValue, type);
        }

        private void FilterCurrencies()
        {
            if (string.IsNullOrWhiteSpace(CurrencySearch))
            {
                FilteredCurrencies = new ObservableCollection<CurrencyModel>(allCurrencies);
            }
            else
            {
                var searchText = CurrencySearch.ToLowerInvariant();
                var filtered = allCurrencies
                    .Where(c => c.ToCurrencyCode.ToLowerInvariant().Contains(searchText) ||
                                c.ToCurrencyName.ToLowerInvariant().Contains(searchText));
                FilteredCurrencies = new ObservableCollection<CurrencyModel>(filtered);
            }
            _logger.LogInformation("Para birimleri '{SearchText}' metnine göre filtrelendi.", CurrencySearch);
        }
    }
}