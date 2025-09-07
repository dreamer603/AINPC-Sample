using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class ChatBoxView : MonoBehaviour
    {
        [Header("UI")]
        public RectTransform chatBoxRect;
        public TextMeshProUGUI chatText;
        public Image backgroundImage;
        public GameObject typingAnimation;
        
        [Header("Style Settings")]
        public Color userMessageColor = Color.green;
        public Color aiMessageColor = Color.blue;

        [Header("Size Settings")]
        public Vector2 padding = new Vector2(20f, 15f);
        public float minWidth = 100f;
        public float maxWidth = 400f;
        public float minHeight = 50f;

        private ChatMessage _currentMessage;

        private void Awake()
        {
            if (chatBoxRect == null)
            {
                chatBoxRect = GetComponent<RectTransform>();
            }

            if (chatText == null)
            {
                chatText = GetComponentInChildren<TextMeshProUGUI>();
            }

            if (backgroundImage == null)
            {
                backgroundImage = GetComponent<Image>();
            }
        }

        public void SetMessage(ChatMessage message)
        {
            _currentMessage = message;
            chatText.text = message.contents;
            SetupAlignment(message.isFromUser);
            ShowTypingAnimation(message.isTyping);
            ResizeChatBox();
        }

        public void UpdateContents(string content)
        {
            chatText.text = content;
            ShowTypingAnimation(false);
            ResizeChatBox();
        }

        private void SetupAlignment(bool isFromUser)
        {
            RectTransform rect = GetComponent<RectTransform>();
            int x = isFromUser ? 1 : 0;
            
            rect.anchorMin = new Vector2(x, 0);
            rect.anchorMax = new Vector2(x, 0);
            rect.pivot = new Vector2(x, 0);
            backgroundImage.color = isFromUser ? userMessageColor : aiMessageColor;
        }

        private void ShowTypingAnimation(bool show)
        {
            if (typingAnimation != null)
            {
                typingAnimation.SetActive(show);
            }
            
            chatText.gameObject.SetActive(!show);
        }

        private void ResizeChatBox()
        {
            chatText.ForceMeshUpdate();
            Vector2 textSize = chatText.GetRenderedValues(false);

            float newWidth = Mathf.Clamp(textSize.x + padding.x * 2, minWidth, maxWidth);
            float newHeight = Mathf.Max(textSize.y + padding.y * 2, minHeight);

            chatBoxRect.sizeDelta = new Vector2(newWidth, newHeight);
        }
    }
}