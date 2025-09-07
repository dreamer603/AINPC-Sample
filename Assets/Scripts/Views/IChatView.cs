using System;
using Models;

namespace Views
{
    public interface IChatView
    {
        event Action<string> OnSendMessage;
        event Action OnClearChat;

        void DisplayMessage(ChatMessage message);
        void UpdateMessage(string messageId, string content);
        void ClearMessages();
        
        void ScrollToBottom();
        
        void SetInputFieldText(string text);
        void SetInputFieldEnabled(bool enabled);
        
        void ShowTyping(bool show);
        void ShowError(string error);
    }
}