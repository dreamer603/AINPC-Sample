using System;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Views;

namespace Presenters
{
    public class ChatPresenter
    {
        private ChatModel _model;
        private IChatView _view;
        private AIModel _aiModel;

        public ChatPresenter(ChatModel model, IChatView view, AIModel aiModel)
        {
            _model = model;
            _view = view;
            _aiModel = aiModel;

            model.OnMessageAdded += HandleMessageAdded;
            model.OnMessageUpdated += HandleMessageUpdated;
            model.OnMessageCleared += HandleMessagesCleared;
            model.OnTypingStatusChanged += HandleTypingStatusChanged;

            view.OnSendMessage += HandleSendMessage;
            view.OnClearChat += HandleClearChat;

            _aiModel.OnInitError += HandleAIError;
        }

        public async Task Init()
        {
            _view.SetInputFieldEnabled(false);

            bool success = await _aiModel.Init();
            
            if (success)
            {
                _view.SetInputFieldEnabled(true);
                _model.AddMessage(_model.CurrentPersona.firstGreeting, false);
            }
            else
            {
                _view.ShowError("Failed init AI");
            }
        }

        private async void HandleSendMessage(string contents)
        {
            if (string.IsNullOrEmpty(contents.Trim()))
            {
                return;
            }
            
            _model.AddMessage(contents, true);
            _view.SetInputFieldText("");
            _view.SetInputFieldEnabled(false);
            
            _model.SetTypingStatus(true);
            _model.AddMessage("", false, true);

            try
            {
                AIResponse aiResponse = await _aiModel.SendMessageAsync(contents, _model.CurrentPersona, _model.Memory);

                _model.SetTypingStatus(false);
                _model.UpdateLastMessage(aiResponse.userMessage);

                if (!string.IsNullOrEmpty(aiResponse.summary))
                {
                    _model.AddMemorySummary(aiResponse.summary);
                }
            } catch (Exception e) {
                _model.SetTypingStatus(false);
                _model.UpdateLastMessage(e.Message);
                _view.ShowError("AI ERROR");
            } finally {
                _view.SetInputFieldEnabled(true);
            }
        }

        private void HandleClearChat()
        {
            _model.ClearMessages();
            _model.AddMessage("기억 초기화", false);
        }

        private void HandleMessageAdded(ChatMessage message)
        {
            _view.DisplayMessage(message);
        }

        private void HandleMessageUpdated(string messageId)
        {
            var message = _model.Messages.FirstOrDefault(m => m.id == messageId);
            if (message != null)
            {
                _view.UpdateMessage(messageId, message.contents);
            }
        }

        private void HandleMessagesCleared()
        {
            _view.ClearMessages();
        }

        private void HandleTypingStatusChanged(bool isTyping)
        {
            _view.ShowTyping(isTyping);
        }

        private void HandleAIError(string error)
        {
            _view.ShowError(error);
        }

        public void Dispose()
        {
            if (_model != null)
            {
                _model.OnMessageAdded -= HandleMessageAdded;
                _model.OnMessageUpdated -= HandleMessageUpdated;
                _model.OnMessageCleared -= HandleMessagesCleared;
                _model.OnTypingStatusChanged -= HandleTypingStatusChanged;
            }

            if (_view != null)
            {
                _view.OnSendMessage -= HandleSendMessage;
                _view.OnClearChat -= HandleClearChat;
            }

            if (_aiModel != null)
            {
                _aiModel.OnInitError -= HandleAIError;
            }
        }
        
    }
}