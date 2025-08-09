using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.Currency;
using FinTrackForWindows.Services.Currencies;
using FinTrackWebApi.Dtos.CurrencyDtos;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
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
        private string selectedPeriod = "1M";

        [ObservableProperty]
        private ISeries[] series = new ISeries[0];

        [ObservableProperty]
        private Axis[] xAxes = new Axis[0];

        [ObservableProperty]
        private Axis[] yAxes = new Axis[0];

        [ObservableProperty]
        private LabelVisual title = new();

        [ObservableProperty]
        private ISeries[] gaugeSeries = new ISeries[0];

        [ObservableProperty]
        private IEnumerable<ChartElement<SkiaSharpDrawingContext>> gaugeVisuals = new List<ChartElement<SkiaSharpDrawingContext>>();

        [ObservableProperty]
        private ISeries[] pieSeries = new ISeries[0];

        [ObservableProperty]
        private string periodHighText = "N/A";

        [ObservableProperty]
        private string periodLowText = "N/A";

        [ObservableProperty]
        private string averageRateText = "N/A";

        [ObservableProperty]
        private double periodHighValue = 1;

        [ObservableProperty]
        private double periodLowValue = 0;

        private readonly ILogger<CurrenciesViewModel> _logger;
        private readonly ICurrenciesStore _currenciesStore;
        private static readonly SKColor _chartColor = SKColor.Parse("#3498DB");

        public CurrenciesViewModel(ILogger<CurrenciesViewModel> logger, ICurrenciesStore currenciesStore)
        {
            _logger = logger;
            _currenciesStore = currenciesStore;

            InitializeEmptyChart();
            _ = InitializeDataAsync();
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

        private async Task InitializeDataAsync()
        {
            try
            {
                await _currenciesStore.LoadCurrenciesAsync();
                FilterCurrencies();

                if (FilteredCurrencies.Any())
                {
                    SelectedCurrency = FilteredCurrencies.First();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initializing currency data.");
                InitializeEmptyChart("Failed to load currency list.");
            }
        }

        private async Task LoadHistoricalDataAsync(string targetCurrencyCode)
        {
            IsLoadingDetails = true;
            try
            {
                var historyData = await _currenciesStore.GetHistoricalDataAsync(targetCurrencyCode, SelectedPeriod);

                if (historyData == null || SelectedCurrency == null || !historyData.HistoricalRates.Any())
                {
                    InitializeEmptyChart($"No data available for {targetCurrencyCode}.");
                    UpdateDetails(null);
                    return;
                }

                _logger.LogInformation("Processing historical data for {Target} in ViewModel.", targetCurrencyCode);
                var dailyHistoricalRates = UpdateChart(historyData);
                UpdateAdditionalVisuals(dailyHistoricalRates);
                UpdateDetails(historyData.ChangeSummary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing historical data in ViewModel.");
                InitializeEmptyChart($"Error processing data for {targetCurrencyCode}.");
                UpdateDetails(null);
            }
            finally
            {
                IsLoadingDetails = false;
            }
        }

        [RelayCommand]
        private async Task ChangePeriod(string? newPeriod)
        {
            if (string.IsNullOrWhiteSpace(newPeriod) || SelectedPeriod == newPeriod || SelectedCurrency == null)
            {
                return;
            }

            _logger.LogInformation("Changing chart period to {NewPeriod}.", newPeriod);
            SelectedPeriod = newPeriod;

            await LoadHistoricalDataAsync(SelectedCurrency.ToCurrencyCode);
        }

        private List<HistoricalRatePointDto> UpdateChart(CurrencyHistoryDto historyData)
        {
            var dailyHistoricalRates = historyData.HistoricalRates
                .GroupBy(r => r.Date.Date)
                .Select(g => g.OrderBy(r => r.Date).Last())
                .Select(r => new HistoricalRatePointDto { Date = r.Date.Date, Rate = r.Rate })
                .ToList();

            var historicalPoints = dailyHistoricalRates
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
                Paint = new SolidColorPaint(SKColors.WhiteSmoke)
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

            return dailyHistoricalRates;
        }

        private void UpdateAdditionalVisuals(List<HistoricalRatePointDto> dailyRates)
        {
            if (dailyRates == null || !dailyRates.Any())
            {
                ClearAdditionalVisuals();
                return;
            }

            var rates = dailyRates.Select(r => r.Rate).ToList();
            var currentRate = (double)rates.Last();
            var periodHigh = (double)rates.Max();
            var periodLow = (double)rates.Min();
            var averageRate = (double)rates.Average();

            PeriodHighText = periodHigh.ToString("N4");
            PeriodLowText = periodLow.ToString("N4");
            AverageRateText = averageRate.ToString("N4");

            PeriodHighValue = periodHigh;
            PeriodLowValue = periodLow;

            GaugeSeries = new ISeries[]
            {
                new PieSeries<double> { Values = new double[] { currentRate }, InnerRadius = 50, Fill = new SolidColorPaint(_chartColor), IsHoverable = false, },
                new PieSeries<double> { Values = new double[] { periodHigh - currentRate }, InnerRadius = 50, Fill = new SolidColorPaint(SKColors.DarkGray), IsHoverable = false, }
            };

            GaugeVisuals = Enumerable.Empty<ChartElement<SkiaSharpDrawingContext>>();

            int upDays = 0, downDays = 0, noChangeDays = 0;
            for (int i = 1; i < dailyRates.Count; i++)
            {
                if (dailyRates[i].Rate > dailyRates[i - 1].Rate) upDays++;
                else if (dailyRates[i].Rate < dailyRates[i - 1].Rate) downDays++;
                else noChangeDays++;
            }

            PieSeries = new ISeries[]
            {
                new PieSeries<int> { Values = new [] { upDays }, Name = "Up Days", Fill = new SolidColorPaint(SKColor.Parse("#2ECC71")) },
                new PieSeries<int> { Values = new [] { downDays }, Name = "Down Days", Fill = new SolidColorPaint(SKColor.Parse("#E74C3C")) },
                new PieSeries<int> { Values = new [] { noChangeDays }, Name = "No Change", Fill = new SolidColorPaint(SKColors.Gray) }
            };
        }

        private void ClearAdditionalVisuals()
        {
            GaugeSeries = new ISeries[0];
            GaugeVisuals = new List<ChartElement<SkiaSharpDrawingContext>>();
            PieSeries = new ISeries[0];
            PeriodHighText = "N/A";
            PeriodLowText = "N/A";
            AverageRateText = "N/A";
            PeriodHighValue = 1;
            PeriodLowValue = 0;
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
                Padding = new Padding(15),
                VerticalAlignment = Align.Middle
            };
            ClearAdditionalVisuals();
        }

        private (string FormattedValue, CurrencyConversionType Type) FormatChangeValue(decimal? value, bool isPercentage = true, string format = "P2")
        {
            if (!value.HasValue) return ("N/A", CurrencyConversionType.Neutral);

            string formattedValue = value.Value.ToString(format, CultureInfo.InvariantCulture);
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
            var sourceList = _currenciesStore.Currencies;
            if (string.IsNullOrWhiteSpace(CurrencySearch))
            {
                FilteredCurrencies = new ObservableCollection<CurrencyModel>(sourceList);
            }
            else
            {
                var searchText = CurrencySearch.ToLowerInvariant();
                var filtered = sourceList
                    .Where(c => c.ToCurrencyCode.ToLowerInvariant().Contains(searchText) ||
                                c.ToCurrencyName.ToLowerInvariant().Contains(searchText));
                FilteredCurrencies = new ObservableCollection<CurrencyModel>(filtered);
            }
            _logger.LogInformation("Currencies filtered by search text: '{SearchText}'.", CurrencySearch);
        }
    }
}