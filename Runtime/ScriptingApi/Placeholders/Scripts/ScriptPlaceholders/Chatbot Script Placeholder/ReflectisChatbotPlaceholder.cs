using UnityEngine;

namespace Virtuademy.Environments.ScriptingApi.Placeholders
{
    public class ReflectisChatbotPlaceholder : ChatbotPlaceholderBase
    {
        [Tooltip("This is the name of the agent that you create in the section Reflectis AI of the Back Office." +
            "Copy-paste the name here to make this chatbot use such agent.")]
        [SerializeField] private string agent = "minimario";

        public string Agent => agent;
    }
}
