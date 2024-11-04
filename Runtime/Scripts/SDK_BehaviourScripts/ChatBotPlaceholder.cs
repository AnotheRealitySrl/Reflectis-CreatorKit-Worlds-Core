using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.SDK.CreatorKit
{
    public class ChatBotPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private string initialConversationSentence = "Hello!";
        [SerializeField] private Transform panTarget;
        [SerializeField] private RectTransform chatPanel;

        public string InitialConversationSentence => initialConversationSentence;
        public Transform PanTarget => panTarget;
        public RectTransform ChatPanel => chatPanel;

        public UnityEvent OnSelectedEvent { get; } = new();
        public UnityEvent OnDeselectedEvent { get; } = new();
    }
}
