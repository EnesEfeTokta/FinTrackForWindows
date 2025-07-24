using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.ChatDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.FinBot;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text;

namespace FinTrackForWindows.ViewModels
{
    public partial class FinBotViewModel : ObservableObject
    {
        public ObservableCollection<ChatMessageModel> Messages { get; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
        private string messageInput = string.Empty;

        private readonly ILogger<FinBotViewModel> _logger;

        private readonly IApiService _apiService;

        private const string chars = "qQwWeErRtTyYuUiIoOpPaAsSdDfFgGhHjJkKlLzZxXcCvVbBnNmM0123456789+-*/|<>£&()='!#${[]}";
        private readonly string? clientChatSessionId;

        public FinBotViewModel(ILogger<FinBotViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
            Messages = new ObservableCollection<ChatMessageModel>();

            Random random = new Random();
            StringBuilder result = new StringBuilder(10);
            for (int i = 0; i < 10; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }
            clientChatSessionId = result.ToString();

            LoadInitialMessage();
        }

        private void LoadInitialMessage()
        {
            var initialMessage = new ChatMessageModel("Welcome to FinBot! How can I assist you today?", MessageAuthor.Bot)
            {
                QuickActions = new ObservableCollection<string>
                {
                    "List All My Accounts",
                    "List All My Budgets",
                    "List All My Transacitons",
                    "Total Income Amount",
                    "Total Expense Amount"
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

            var chatResponse = await _apiService.PostAsync<ChatResponseDto>("Chat/send", new ChatRequestDto
            {
                Message = userMessage.Text,
                ClientChatSessionId = clientChatSessionId,
            });

            if (chatResponse != null)
            {
                var botResponse = new ChatMessageModel(chatResponse.Reply ?? "N/A", MessageAuthor.Bot);
                Messages.Add(botResponse);
            }
        }

        private bool CanSendMessage => !string.IsNullOrWhiteSpace(MessageInput);

        [RelayCommand]
        private async Task SendQuickActionAsync(string actionText)
        {
            if (string.IsNullOrWhiteSpace(actionText)) return;

            string actionMessage = string.Empty;
            switch (actionText)
            {
                case "List All My Accounts":
                    actionMessage = "List all my accounts. Don't give too long explanations.";
                    return;
                case "List All My Budgets":
                    actionMessage = "List all my budgets. Don't give too long explanations.";
                    return;
                case "List All My Transacitons":
                    actionMessage = "List all my transacitons. Don't give too long explanations.";
                    return;
                case "Total Income Amount":
                    actionMessage = "The sum of all my income. Don't give too long explanations.";
                    return;
                case "Total Expense Amount":
                    actionMessage = "The sum of all my expense. Don't give too long explanations.";
                    return;
            }

            var userMessage = new ChatMessageModel(actionMessage, MessageAuthor.User);
            Messages.Add(userMessage);

            _logger.LogInformation("Kullanıcı hızlı eylem seçti: {ActionText}", actionText);

            await ProcessBotResponseAsync(actionMessage);
        }

        private async Task ProcessBotResponseAsync(string userMessage)
        {
            await Task.Delay(1000); // Bot "yazıyor..." efekti için küçük bir bekleme

            ChatMessageModel botResponse;
            string lowerUserMessage = userMessage.ToLower();

            botResponse = new ChatMessageModel(lowerUserMessage, MessageAuthor.Bot);

            Messages.Add(botResponse);
        }
    }
}
