using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using FinTrack.Models.FinBot;
using Microsoft.Extensions.Logging;
using FinTrack.Enums;
using CommunityToolkit.Mvvm.Input;

namespace FinTrack.ViewModels
{
    public partial class FinBotViewModel : ObservableObject
    {
        public ObservableCollection<ChatMessageModel> Messages { get; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
        private string messageInput = string.Empty;

        private readonly ILogger<FinBotViewModel> _logger;

        public FinBotViewModel(ILogger<FinBotViewModel> logger)
        {
            _logger = logger;
            Messages = new ObservableCollection<ChatMessageModel>();

            LoadInitialMessage();
        }

        private void LoadInitialMessage()
        {
            var initialMessage = new ChatMessageModel("Welcome to FinBot! How can I assist you today?", MessageAuthor.Bot)
            {
                QuickActions = new ObservableCollection<string>
                {
                    "View Account Balance",
                    "Recent Transactions",
                    "Budget Overview"
                }
            };
            Messages.Add(initialMessage);
            _logger.LogInformation("Initial message loaded: {MessageText}", initialMessage.Text);
        }

        [RelayCommand(CanExecute = nameof(CanSendMessage))]
        private async Task SendMessageAsync()
        {
            if (!CanSendMessage) return;

            var userMessage = new ChatMessageModel(MessageInput, MessageAuthor.User);
            Messages.Add(userMessage);

            MessageInput = string.Empty;

            _logger.LogInformation("Kullanıcı mesaj yazdı. {MessageText}", userMessage.Text);

            await ProcessBotResponseAsync(userMessage.Text);
        }

        private bool CanSendMessage => !string.IsNullOrWhiteSpace(MessageInput);

        [RelayCommand]
        private async Task SendQuickActionAsync(string actionText)
        {
            if (string.IsNullOrWhiteSpace(actionText)) return;

            var userMessage = new ChatMessageModel(actionText, MessageAuthor.User);
            Messages.Add(userMessage);

            _logger.LogInformation("Kullanıcı hızlı eylem seçti: {ActionText}", actionText);

            await ProcessBotResponseAsync(actionText);
        }






        // TODO: [TEST]
        private async Task ProcessBotResponseAsync(string userMessage)
        {
            // Bot "yazıyor..." efekti için küçük bir bekleme
            await Task.Delay(1000);

            ChatMessageModel botResponse;
            string lowerUserMessage = userMessage.ToLower();

            if (lowerUserMessage.Contains("bakiye"))
            {
                botResponse = new ChatMessageModel("Toplam bakiyeniz tüm hesaplarınızda $12,450.75.", MessageAuthor.Bot);
            }
            else if (lowerUserMessage.Contains("teşekkür"))
            {
                botResponse = new ChatMessageModel("Rica ederim! Başka bir konuda yardımcı olabilir miyim?", MessageAuthor.Bot);
            }
            else
            {
                botResponse = new ChatMessageModel("Üzgünüm, bu isteği anlayamadım. Lütfen farklı bir şekilde sormayı deneyin.", MessageAuthor.Bot)
                {
                    QuickActions = new ObservableCollection<string> { "Yeni Bütçe Oluştur", "Hesap Bakiyem", "Rapor Oluştur" }
                };
            }

            Messages.Add(botResponse);
        }
    }
}
