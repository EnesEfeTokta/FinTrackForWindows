using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Core;
using Microsoft.Extensions.Logging;

namespace FinTrackForWindows.ViewModels
{
    public partial class ApplicationRecognizeSlideViewModel : ObservableObject
    {
        [ObservableProperty]
        private string currentTitle_ApplicationRecognizeSlideView_TextBlock = string.Empty;

        [ObservableProperty]
        private string currentDescription_ApplicationRecognizeSlideView_TextBlock = string.Empty;

        [ObservableProperty]
        private string currentImagePath_ApplicationRecognizeSlideView_Image = string.Empty;

        public event Action? NavigateToLoginRequested;

        private List<ApplicationRecognizeSlide> applicationRecognizeSlides = new List<ApplicationRecognizeSlide>
        {
            new ApplicationRecognizeSlide
            {
                Title = "Welcome to FinTrack: Your First Step to Financial Freedom",
                Description = """
                Discover that achieving your financial goals doesn't have to be complicated. 
                FinTrack unifies your entire financial life—from budgeting and investing to expense tracking and detailed analysis—into one powerful, intuitive platform. Get ready to take control of your financial health.
                """,
                Icon = "/Assets/Images/hello.png"
            },
            new ApplicationRecognizeSlide
            {
                Title = "Data-Driven Decisions: Understand Your Financial Landscape",
                Description = """
                Uncover the "why" behind your financial habits. 
                Our advanced analytics tools present your income streams, spending categories, and budget performance in visually rich, easy-to-understand charts. Shape your future more consciously with clear, actionable reports.
                """,
                Icon = "/Assets/Images/analytics.png"
            },
            new ApplicationRecognizeSlide
            {
                Title = "Your Data, Your Control: Export Financial Reports Freely",
                Description = """
                Your financial data shouldn't be confined to the app. 
                With a single click, export your financial summaries, account statements, or budget reports in professional formats like PDF, Excel, and Word. Your data is always at your fingertips for accounting, archiving, or personal analysis.
                """,
                Icon = "/Assets/Images/document.png"
            },
            new ApplicationRecognizeSlide
            {
                Title = "Every Penny Accounted For: Effortless Expense Tracking",
                Description = """
                Financial awareness begins with recording every income and expense. 
                With smart categorization and quick-entry features, easily log your daily spending and income sources. 
                See exactly where your money is going to eliminate waste and maximize savings.
                """,
                Icon = "/Assets/Images/increase.png"
            },
            new ApplicationRecognizeSlide
            {
                Title = "Achieve Your Dreams: Automate Your Savings Goals",
                Description = """
                Saving is no longer a chore, but a motivating journey. 
                Whether it's for a vacation or a new home, create your goals and let FinTrack monitor your progress for you. 
                Gaining financial discipline has never been this straightforward.
                """,
                Icon = "/Assets/Images/investment.png"
            },
            new ApplicationRecognizeSlide
            {
                Title = "Premium Features, Accessible Price: An Investment in Your Financial Health",
                Description = """
                We believe powerful financial management tools should be accessible to everyone. 
                We are proud to offer our comprehensive feature set through a transparent and competitive membership model that won't strain your budget. No hidden fees, just a service focused on your financial growth.
                """,
                Icon = "/Assets/Images/payment.png"
            },
            new ApplicationRecognizeSlide
            {
                Title = "Global Markets at Your Fingertips: Track Exchange Rates in Real-Time",
                Description = """
                Stay informed while managing your investments or international spending. 
                Track real-time exchange rates for dozens of fiat and digital currencies, access historical data, and analyze market trends to make the most accurate decisions for your portfolio.
                """,
                Icon = "/Assets/Images/statistic.png"
            },
            new ApplicationRecognizeSlide
            {
                Title = "Your Smart Financial Assistant: Unleash Your Potential with AI",
                Description = """
                Don't get lost in piles of data. Our AI-powered assistant analyzes your financial records to provide personalized savings tips, detect unusual spending, and identify potential areas for improvement in your budget. 
                Your financial advisor is now available 24/7.
                """,
                Icon = "/Assets/Images/chatbot.png"
            }
        };

        private readonly ILogger<ApplicationRecognizeSlideViewModel> _logger;

        private readonly ISecureTokenStorage _secureToken;

        private int currentSlideIndex = 0;

        public ApplicationRecognizeSlideViewModel(ILogger<ApplicationRecognizeSlideViewModel> logger, ISecureTokenStorage secureToken)
        {
            _logger = logger;
            _secureToken = secureToken;

            UpdateCurrentSlide(CurrentSlide);
        }

        [RelayCommand]
        public void Back_ApplicationRecognizeSlideView_Button()
        {
            if (currentSlideIndex > 0)
            {
                currentSlideIndex--;
                UpdateCurrentSlide(CurrentSlide);
                _logger.LogInformation("Kullanıcı geri düğmesine bastı. Şu anki slayt: {CurrentSlideTitle}", CurrentSlide.Title);
            }
        }

        [RelayCommand]
        public void Next_ApplicationRecognizeSlideView_Button()
        {
            if (currentSlideIndex < applicationRecognizeSlides.Count - 1)
            {
                currentSlideIndex++;
                _logger.LogInformation("Kullanıcı ileri düğmesine bastı. Şu anki slayt: {CurrentSlideTitle}", CurrentSlide.Title);
                UpdateCurrentSlide(CurrentSlide);
            }
        }

        [RelayCommand]
        public void Skip_ApplicationRecognizeSlideView_Button()
        {
            _logger.LogInformation("Kullanıcı uygulama tanıtımını atladı. Giriş ekranına yönlendiriliyor.");
            NavigateToLoginRequested?.Invoke();
        }

        public ApplicationRecognizeSlide CurrentSlide => applicationRecognizeSlides[currentSlideIndex];

        private void UpdateCurrentSlide(ApplicationRecognizeSlide slide)
        {
            CurrentTitle_ApplicationRecognizeSlideView_TextBlock = slide.Title;
            CurrentDescription_ApplicationRecognizeSlideView_TextBlock = slide.Description;
            CurrentImagePath_ApplicationRecognizeSlideView_Image = slide.Icon;
        }
    }

    public class ApplicationRecognizeSlide
    {
        public string Title { get; set; } = "Application Recognize Slide";
        public string Description { get; set; } =
            """
            This slide is used to recognize applications in the FinTrack application. 
            It provides a user-friendly interface for users to identify and manage their applications effectively.
            """;
        public string Icon { get; set; } = "icon.png";
    }
}
