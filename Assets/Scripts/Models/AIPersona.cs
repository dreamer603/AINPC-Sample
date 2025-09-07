using UnityEngine;

namespace Models
{
    [CreateAssetMenu(fileName = "AIPersona", menuName = "AIChat/AIPersona", order = 1)]
    public class AIPersona : ScriptableObject
    {
        public string personaName;

        public string personality;

        public string background;

        public string speakingStyle;

        public string specialInstructions;

        public float creativityLevel = 0.7f;

        public float formalityLevel = 0.5f;

        public string[] preferredTopics;

        [TextArea(10,10)]
        public string firstGreeting;

        public string GetPersonaPrompt()
        {
            string topicsText = preferredTopics != null && preferredTopics.Length > 0 ? $"\n선호하는 주제 : {string.Join(", ", preferredTopics)}" : "";
            return $@"너는 '{personaName}'이다.
성격: {personality}
배경: {background}
말투 : {speakingStyle}
특별 지시사항: {specialInstructions}
창의성: {creativityLevel * 100}% (0%는 매우 논리적, 100%는 매우 창의적)
격식: {formalityLevel * 100}% (0%는 매우 친근, 100%는 매우 격식적{topicsText}
위의 설정에 맞춰 일관된 캐릭터로 대화하라.";
        }

        public string GetDisplayName()
        {
            return string.IsNullOrEmpty(personaName) ? name : personaName;
        }
    }
}