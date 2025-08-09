using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FinTrackForWindows.Dtos.ChatDtos;
using FinTrackForWindows.Enums;
using FinTrackForWindows.Models.FinBot;
using FinTrackForWindows.Services.Api;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;

namespace FinTrackForWindows.ViewModels
{
    public partial class FinBotViewModel : ObservableObject
    {
        public ObservableCollection<ChatMessageModel> Messages { get; }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
        private string _messageInput = string.Empty;

        [ObservableProperty]
        private bool _isAwaitingBotResponse = false;

        private readonly ILogger<FinBotViewModel> _logger;
        private readonly IApiService _apiService;
        private readonly string _clientChatSessionId;

        public FinBotViewModel(ILogger<FinBotViewModel> logger, IApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
            Messages = new ObservableCollection<ChatMessageModel>();

            const string chars = "qQwWeErRtTyYuUiIoOpPaAsSdDfFgGhHjJkKlLzZxXcCvVbBnNmM0123456789";
            Random random = new Random();
            _clientChatSessionId = new string(Enumerable.Repeat(chars, 10)
                .Select(s => s[random.Next(s.Length)]).ToArray());

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
                    "List All My Transactions",
                    "Total Income Amount",
                    "Total Expense Amount"
                }
            };
            Messages.Add(initialMessage);
        }

        private bool CanSendMessage() => !string.IsNullOrWhiteSpace(MessageInput) && !IsAwaitingBotResponse;

        [RelayCommand(CanExecute = nameof(CanSendMessage))]
        private async Task SendMessageAsync()
        {
            if (!CanSendMessage()) return;

            var userMessageText = MessageInput;
            var userMessage = new ChatMessageModel(userMessageText, MessageAuthor.User);
            Messages.Add(userMessage);
            MessageInput = string.Empty;

            _logger.LogInformation("User sent a message: {MessageText}", userMessage.Text);

            await GetBotResponseAsync(userMessageText);
        }

        [RelayCommand]
        private async Task SendQuickActionAsync(string actionText)
        {
            if (string.IsNullOrWhiteSpace(actionText) || IsAwaitingBotResponse) return;

            string botCommandMessage = actionText switch
            {
                "List All My Accounts" => "List all my accounts. Don't give too long explanations.",
                "List All My Budgets" => "List all my budgets. Don't give too long explanations.",
                "List All My Transactions" => "List all my transacitons. Don't give too long explanations.",
                "Total Income Amount" => "The sum of all my income. Don't give too long explanations.",
                "Total Expense Amount" => "The sum of all my expense. Don't give too long explanations.",
                _ => actionText
            };

            var userMessage = new ChatMessageModel(actionText, MessageAuthor.User);
            Messages.Add(userMessage);

            _logger.LogInformation("User selected a quick action: {ActionText}", actionText);

            await GetBotResponseAsync(botCommandMessage);
        }

        private async Task GetBotResponseAsync(string messageForBot)
        {
            IsAwaitingBotResponse = true;
            SendMessageCommand.NotifyCanExecuteChanged();

            try
            {
                var chatResponse = await _apiService.PostAsync<ChatResponseDto>("Chat/send", new ChatRequestDto
                {
                    Message = messageForBot,
                    ClientChatSessionId = _clientChatSessionId,
                });

                if (chatResponse != null)
                {
                    var botResponse = new ChatMessageModel(chatResponse.Reply ?? "Sorry, I couldn't process that.", MessageAuthor.Bot);
                    Messages.Add(botResponse);
                }
                else
                {
                    var errorResponse = new ChatMessageModel("I seem to be having trouble connecting. Please try again later.", MessageAuthor.Bot);
                    Messages.Add(errorResponse);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting response from Chat API.");
                var errorResponse = new ChatMessageModel("An error occurred. I can't respond right now.", MessageAuthor.Bot);
                Messages.Add(errorResponse);
            }
            finally
            {
                IsAwaitingBotResponse = false;
                SendMessageCommand.NotifyCanExecuteChanged();
            }
        }
    }
}