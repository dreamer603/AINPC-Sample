using System;
using System.Collections.Generic;

namespace Models
{
    [Serializable]
    public class ConversationMemory
    {
        public List<string> summaries = new List<string>();
        public int maxSummaries = 10;

        public void AddSummary(string summary)
        {
            if (!string.IsNullOrEmpty(summary))
            {
                summaries.Add(summary);

                // Delete old summary
                if (summaries.Count > maxSummaries)
                {
                    summaries.RemoveAt(0);
                }
            }
        }

        public string GetMemoryContext()
        {
            if (summaries.Count == 0)
            {
                return "";
            }

            return $@"이전 대화 요약: {string.Join("\n", summaries)}
위 내용을 참고해 일관성 있는 대화를 이어갈 것.";
        }

        public void ClearMemory()
        {
            summaries.Clear();
        }
    }
}