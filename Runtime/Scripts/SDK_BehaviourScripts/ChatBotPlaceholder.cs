using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.SDK.CreatorKit
{
    public class ChatBotPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private string initialConversationSentence = "Hello!";
        [SerializeField] private Transform panTarget;
        [SerializeField] private RectTransform chatPanel;
        [SerializeField] private AudioSource audioSource;

        public string InitialConversationSentence => initialConversationSentence;
        public Transform PanTarget => panTarget;
        public RectTransform ChatPanel => chatPanel;
        public AudioSource AudioSource => audioSource;

        public UnityEvent OnChatBotSelect { get; } = new();
        public UnityEvent OnChatBotUnselected { get; } = new();
    }
}
