using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.AI;
using Models;
using UnityEngine;

namespace Models
{
    public class AIResponse
    {
        public string userMessage;
        public string summary;
    }
    
    public class AIModel
    {
        private FirebaseAI _ai;
        private GenerativeModel _model;
        private bool _isInit = false;
        
        public event Action<string> OnInitError;

        public async Task<bool> Init()
        {
            try
            {
                _ai = FirebaseAI.GetInstance(FirebaseAI.Backend.GoogleAI());
                _model = _ai.GetGenerativeModel(modelName: "gemini-2.5-flash");

                var prompt = "";

                var response = await _model.GenerateContentAsync(prompt);

                _isInit = true;
                return true;
            } catch (Exception e) {
                Debug.LogError(e.Message);
                OnInitError?.Invoke(e.Message);
                return false;
            }
        }

        public async Task<AIResponse> SendMessageAsync(string message, AIPersona persona, ConversationMemory memory)
        {
            if (!_isInit)
            {
                throw new InvalidOperationException("Ai Model is not init.");
            }

            try
            {
                string prompt = BuildFullPrompt(message, persona, memory);
                var response = await _model.GenerateContentAsync(prompt);
                var aiResponse = ParseAIResponse(response.Text);
                return aiResponse;
            } catch (Exception e) {
                Debug.LogError(e.Message);
                throw;
            }
        }

        public void ClearChatHistory()
        {
            if (_isInit && _model != null)
            {
                
            }
        }

        private string BuildFullPrompt(string userMessage, AIPersona persona, ConversationMemory memory)
        {
            return $@"{persona.GetPersonaPrompt()}
{memory.GetMemoryContext()}
사용자 메시지: {userMessage}
응답형식:
답변: [사용자에게 보여줄 응답]
요약: [이 대화의 핵심 내용을 1-2문장 요약]
위 형식을 반드시 지킬 것.";
        }

        private AIResponse ParseAIResponse(string rawResponse)
        {
            var response = new AIResponse();

            try
            {
                string[] lines = rawResponse.Split('\n');
                bool isAnswer = false;
                bool isSummary = false;

                List<string> answerLines = new List<string>();
                List<string> summaryLines = new List<string>();

                foreach (string line in lines)
                {
                    string trimLines = line.Trim();

                    if (trimLines.StartsWith("답변:"))
                    {
                        isAnswer = true;
                        isSummary = false;
                        string answerContent = trimLines.Substring(3).Trim();
                        if (!string.IsNullOrEmpty(answerContent))
                        {
                            answerLines.Add(answerContent);
                        }
                    }
                    else if (trimLines.StartsWith("요약:"))
                    {
                        isAnswer = false;
                        isSummary = true;
                        string summaryContent = trimLines.Substring(3).Trim();
                        if (!string.IsNullOrEmpty(summaryContent))
                        {
                            summaryLines.Add(summaryContent);
                        }
                    }
                    else if (isAnswer && !string.IsNullOrEmpty(trimLines))
                    {
                        answerLines.Add(trimLines);
                    }
                    else if (isSummary && !string.IsNullOrEmpty(trimLines))
                    {
                        summaryLines.Add(trimLines);
                    }
                }

                response.userMessage = string.Join("\n", answerLines);
                response.summary = string.Join("\n", summaryLines);

                if (string.IsNullOrEmpty(response.userMessage))
                {
                    response.userMessage = rawResponse;
                    response.summary = "일반적인 대화";
                }
            } catch (Exception e) {
                Debug.LogError(e.Message);
                response.userMessage = rawResponse;
                response.summary = "대화내용";
            }

            return response;
        } 
    }
}