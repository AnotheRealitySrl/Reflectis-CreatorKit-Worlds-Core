using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class ChatBotPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private string initialConversationSentence = "Hello!";

        public string InitialConversationSentence => initialConversationSentence;
    }
}
