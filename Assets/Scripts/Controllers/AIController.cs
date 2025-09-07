using System;
using System.Threading.Tasks;
using Models;
using Presenters;
using UnityEngine;
using Views;

namespace Controllers
{
    public class AIChatController : MonoBehaviour
    {
        public ChatView chatView;

        private ChatModel _chatModel;
        private ChatPresenter _chatPresenter;
        private AIModel _aiModel;

        private async void Awake()
        {
            await InitMVP();
        }

        private async Task InitMVP()
        {
            _chatModel = new ChatModel();
            _aiModel = new AIModel();

            _chatPresenter = new ChatPresenter(_chatModel, chatView, _aiModel);
            await _chatPresenter.Init();
        }

        private void OnDestroy()
        {
            _chatPresenter?.Dispose();
        }
    }
}