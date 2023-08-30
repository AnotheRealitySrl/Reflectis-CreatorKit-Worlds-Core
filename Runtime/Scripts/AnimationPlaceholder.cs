using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.SDK.CreatorKit
{
    public class AnimationPlaceholder : SceneComponentPlaceholderNetwork
    {
        public enum AnimationStates
        {
            Playing = 1,
            Paused = 2,
            Stopped = 3
        }

        [Header("Animation references")]
        [SerializeField] private bool isLooping;
        [SerializeField] private AnimationStates animationState;
        [SerializeField] private Animator animator;
        [SerializeField] private List<GameObject> connectables;
        [SerializeField] private string animationToTrigger;
        [SerializeField] private string animationToResetTrigger;
        [SerializeField] private string changeAnimStateToPlay;
        [SerializeField] private string changeAnimStateToIdle;
        [SerializeField] private bool isInteractable = true;

        [HideInInspector]
        public UnityEvent onEndAnim;

        public bool IsLooping => isLooping;
        public AnimationStates AnimationState => animationState;
        public Animator Animator => animator;
        public List<GameObject> Connectables => connectables;
        public string AnimationToTrigger => animationToTrigger;
        public string AnimationToResetTrigger => animationToResetTrigger;
        public string ChangeAnimStateToPlay => changeAnimStateToPlay;
        public string ChangeAnimStateToIdle => changeAnimStateToIdle;
        public bool IsInteractable => isInteractable;

        public void AnimationEndEvent()
        {
            if (!isLooping)
            {
                onEndAnim?.Invoke();
            }
        }
    }
}
