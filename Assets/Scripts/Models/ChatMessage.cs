using System;

namespace Models
{
    [Serializable]
    public class ChatMessage
    {
        public string id;
        public string contents;
        public bool isFromUser;
        public DateTime time;
        public bool isTyping;

        public ChatMessage(string contents, bool isFromUser, bool isTyping = false)
        {
            id = Guid.NewGuid().ToString();
            this.contents = contents;
            this.isFromUser = isFromUser;
            time = DateTime.Now;
            this.isTyping = isTyping;
        }
    }
}