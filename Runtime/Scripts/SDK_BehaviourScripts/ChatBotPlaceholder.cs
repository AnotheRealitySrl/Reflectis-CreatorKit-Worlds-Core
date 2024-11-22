using Reflectis.SDK.Utilities;

using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.SDK.CreatorKit
{
    public enum EChatBotVoice
    {
        alloy,
        echo,
        shimmer,
        amuch,
        dan,
        elan,
        marilyn,
        meadow,
        breeze,
        cove,
        ember,
        jupiter
    }

    public class ChatBotPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Chatbot structure. Do not change the references,\nunless you need to do heavy customizations.")]
        [SerializeField] private Transform avatarContainer;
        [SerializeField] private RectTransform chatPanel;

        [Tooltip("Specify the animator in the case the avatar in use has multiple animators in the hierarchy.")]
        [SerializeField] private Animator animator;


        [HelpBox("How to configure a chatbot" +
            "First of all, drag&drop one of the avatar templates inside the 'Avatar container' transform. " +
            "Move ore resize it transform, if needed. " +
            "IMPORTANT: to make the interaction work properly, " +
            "Add a reference to the collider of the avatar to the 'InteractablePlaceholder''s collider list. " +
            "Check that the avatar has a collider, needed for the interaction with the chatbot. " +
            "If the avatar is a RPM avatar and you want to add eye-blink and lip-sync behaviors, " +
            "add respectively the 'Eye Animation Handler' and 'Voice Handler' components to the avatar template root. " +
            "Reference the audio source of the avatar inside the 'Voice Handler' component. " +
            "If you want to move the audio source, where the audio of the avatar comes from, " +
            "move it into a transform, keeping its reference to 'Voice Handler' component. " +
            "Add an animator to the avatar, in which there can be multiple states, " +
            "but there must be only one bool trigger named 'Speak'.")]

        [Header("Chatbot configuration")]

        [Header("This will be displayed in the UI panel.")]
        [SerializeField] private string chatbotName = "ChatBot";

        [Tooltip("Select this if the conversation with the chatbot should start automatically, " +
            "without waiting for the user to send the first input.")]
        [SerializeField] private bool startTheConversation = true;

        [DrawIf(nameof(startTheConversation), true)]
        [Tooltip("The initial sentence that is used to start the conversation.")]
        [SerializeField] private string initialConversationSentence;

        [SerializeField, TextArea(10, 30)]
        [Tooltip("Specify here the behaviour of the chatbot, in natural language.")]
        private string instructions;

        [SerializeField] private EChatBotVoice voice = EChatBotVoice.alloy;

        public string ChatbotName => chatbotName;
        public bool StartTheConversation => startTheConversation;
        public string InitialConversationSentence => initialConversationSentence;
        public string Instructions => instructions;
        public EChatBotVoice Voice => voice;

        public Transform AvatarContainer => avatarContainer;
        public RectTransform ChatPanel => chatPanel;
        public Animator Animator => animator;

        public UnityEvent OnChatBotSelect { get; } = new();
        public UnityEvent OnChatBotUnselected { get; } = new();
    }
}
