using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Models
{
    public class ChatModel
    {
        // Message's history
        private List<ChatMessage> _messages = new List<ChatMessage>();

        // Persona Settings
        private AIPersona _currentPersona;
        private ConversationMemory _conversationMemory;
        [SerializeField] private string _personaFileName = "DefaultPersona";
        [SerializeField] private string _personaResourcePath = "AIPersonas";

        // Message's events
        public event Action<ChatMessage> OnMessageAdded; // Add new message
        public event Action<string> OnMessageUpdated;    // Update message
        public event Action OnMessageCleared;            // Clear all messages 
        public event Action<bool> OnTypingStatusChanged; // Change NPC's typing status

        // Message's history in readonly
        public IReadOnlyList<ChatMessage> Messages
        {
            get { return _messages; }
        }

        public AIPersona CurrentPersona
        {
            get { return _currentPersona; }
            set { _currentPersona = value; }
        }

        public ConversationMemory Memory
        {
            get { return _conversationMemory; }
            set { _conversationMemory = value; }
        }

        public ChatModel()
        {
            _conversationMemory = new ConversationMemory();
            SetDefaultPersona();
        }

        public void SetPersona(AIPersona persona)
        {
            _currentPersona = persona;
        }

        private void SetDefaultPersona()
        {
            string fullPath = string.IsNullOrEmpty(_personaResourcePath) ? _personaFileName : $"{_personaResourcePath}/{_personaFileName}";
            _currentPersona = Resources.Load<AIPersona>(fullPath);

            if (_currentPersona == null)
            {
                Debug.Log("Persona Load Error");
            }
        }

        public void AddMessage(string contents, bool isFromUser, bool isTyping = false)
        {
            var message = new ChatMessage(contents, isFromUser, isTyping);
            _messages.Add(message);
            OnMessageAdded?.Invoke(message);
        }

        public void UpdateLastMessage(string contents)
        {
            if (_messages.Count > 0)
            {
                var lastMessage = _messages[-1];
                lastMessage.contents = contents;
                lastMessage.isTyping = false;
                OnMessageUpdated?.Invoke(lastMessage.id);

            }
        }

        public void AddMemorySummary(string summary)
        {
            _conversationMemory.AddSummary(summary);
        }

        public void SetTypingStatus(bool isTyping)
        {
            OnTypingStatusChanged?.Invoke(isTyping);
        }

        public void ClearMessages()
        {
            _messages.Clear();
            OnMessageCleared?.Invoke();
        }

        public List<ChatMessage> GetRecentMessages(int count = 5)
        {
            var userMessages = _messages.Where(m => !m.isTyping).ToList();
            return userMessages.TakeLast(count).ToList();
        }
    }
}
