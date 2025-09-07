using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class ChatView : MonoBehaviour, IChatView
    {
        [Header("UI")]
        public GameObject chatBoxPrefab;
        public Transform chatContents;
        public ScrollRect scrollRect;
        public TMP_InputField inputField;
        public Button sendButton;
        public Button clearButton;
        public GameObject typingIndicator;
        public TextMeshProUGUI statusText;

        [Header("Settings")]
        public float messageSpacing = 10f;

        public event Action<string> OnSendMessage;
        public event Action OnClearChat;

        private Dictionary<string, ChatBoxView> _messageViews = new Dictionary<string, ChatBoxView>();

        private void Awake()
        {
            SetupLayoutComponents();
            SetupEventListeners();
            ShowTyping(false);
        }

        private void SetupLayoutComponents()
        {
            if (chatContents.GetComponent<ContentSizeFitter>() == null)
            {
                ContentSizeFitter fitter = chatContents.gameObject.AddComponent<ContentSizeFitter>();
                fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            }

            if (chatContents.GetComponent<VerticalLayoutGroup>() == null)
            {
                VerticalLayoutGroup layout = chatContents.gameObject.AddComponent<VerticalLayoutGroup>();
                layout.spacing = messageSpacing;
                layout.childAlignment = TextAnchor.LowerLeft;
                layout.childControlHeight = false;
                layout.childControlWidth = false;
                layout.childForceExpandHeight = false;
                layout.childForceExpandWidth = false;
            }
        }

        private void SetupEventListeners()
        {
            sendButton.onClick.AddListener(() =>
            {
                string message = inputField.text.Trim();
                if (!string.IsNullOrEmpty(message))
                {
                    OnSendMessage?.Invoke(message);
                }
            });
            
            inputField.onSubmit.AddListener(text =>
            {
                if (!string.IsNullOrEmpty(text.Trim()))
                {
                    OnSendMessage?.Invoke(text.Trim());
                }
            });

            if (clearButton != null)
            {
                clearButton.onClick.AddListener(() => OnClearChat?.Invoke());
            }
        }

        public void DisplayMessage(ChatMessage message)
        {
            GameObject newChatbox = Instantiate(chatBoxPrefab, chatContents);
            ChatBoxView chatBoxView = newChatbox.GetComponent<ChatBoxView>();

            if (chatBoxView != null)
            {
                chatBoxView.SetMessage(message);
                _messageViews[message.id] = chatBoxView;
            }

            StartCoroutine(ScrollToBottomCoroutine());
        }

        public void UpdateMessage(string messageId, string contents)
        {
            if (_messageViews.TryGetValue(messageId, out ChatBoxView chatBoxView))
            {
                chatBoxView.UpdateContents(contents);
            }
        }

        public void ClearMessages()
        {
            foreach (Transform content in chatContents)
            {
                Destroy(content.gameObject);
            }
            
            _messageViews.Clear();
        }

        public void ScrollToBottom()
        {
            StartCoroutine(ScrollToBottomCoroutine());
        }

        public void SetInputFieldText(string text)
        {
            inputField.text = text;
        }

        public void SetInputFieldEnabled(bool enabled)
        {
            inputField.interactable = enabled;
            sendButton.interactable = enabled;
        }

        public void ShowTyping(bool show)
        {
            if (typingIndicator != null)
            {
                typingIndicator.SetActive(show);
            }
        }

        public void ShowError(string error)
        {
            if (statusText != null)
            {
                statusText.text = error;
                statusText.color = Color.red;
            }
        }

        IEnumerator ScrollToBottomCoroutine()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}