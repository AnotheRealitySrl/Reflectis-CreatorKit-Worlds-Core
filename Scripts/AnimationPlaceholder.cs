using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using static BaseAnimationController;

namespace Virtuademy.Placeholders
{
    public class AnimationPlaceholder : SceneComponentPlaceholderNetwork
    {
        [Header("Animation references")]
        [SerializeField] private bool isLooping;
        [SerializeField] private AnimationStates animationState;
        [SerializeField] private Animator animator;
        [SerializeField] private List<GameObject> connectables;
        [SerializeField] private string animationToTrigger;
        [SerializeField] private string animationToResetTrigger;
        [SerializeField] private string changeAnimStateToPlay;
        [SerializeField] private string changeAnimStateToIdle;
        [SerializeField] private GameObject fatherConnecter;

        public UnityEvent onEndAnim;

        public bool IsLooping => isLooping;
        public AnimationStates AnimationState => animationState;
        public Animator Animator => animator;
        public List<GameObject> Connectables => connectables;
        public string AnimationToTrigger => animationToTrigger;
        public string AnimationToResetTrigger => animationToResetTrigger;
        public string ChangeAnimStateToPlay => changeAnimStateToPlay;
        public string ChangeAnimStateToIdle => changeAnimStateToIdle;
        public GameObject FatherConnecter => fatherConnecter;

        public void AnimationEndEvent()
        {
            if (!isLooping)
            {
                onEndAnim?.Invoke();
            }
        }
    }
}
