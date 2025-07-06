using CommunityToolkit.Mvvm.ComponentModel;
using FinTrack.Enums;
using System.Collections.ObjectModel;

namespace FinTrack.Models.FinBot
{
    public partial class ChatMessageModel : ObservableObject
    {
        [ObservableProperty]
        private string text;

        [ObservableProperty]
        private MessageAuthor author;

        [ObservableProperty]
        private DateTime timestamp = DateTime.Now;

        public ObservableCollection<string>? QuickActions { get; set; }

        public bool IsSentByCurrentUser => Author == MessageAuthor.User;

        public ChatMessageModel(string _text, MessageAuthor _author)
        {
            text = _text;
            author = _author;
        }
    }
}
