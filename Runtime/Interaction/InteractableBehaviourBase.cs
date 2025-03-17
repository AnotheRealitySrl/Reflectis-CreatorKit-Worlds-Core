using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Reflectis.CreatorKit.Worlds.Core.Interaction
{
    public abstract class InteractableBehaviourBase : MonoBehaviour, IInteractableBehaviour
    {
        [SerializeField]
        public bool setupOnAwake;

        public IInteractable InteractableRef
        {
            get
            {
                try
                {
                    return GetComponentInParent<IInteractable>(true);
                }
                catch
                {
                    return null;
                }
            }
        }
        protected abstract bool needSubmeshes { get; }

        public abstract bool IsIdleState { get; }

        //bitmask used to know if an interactable is blocked for various reasons
        [System.Flags]
        public enum EBlockedState
        {
            BlockedByOthers = 1, //blocked by player manipolation (like when manipulating with ownership)
            BlockedBySelection = 2, //used in events like pan/unpan and similar --> Never set by ownership
            BlockedByGenericLogic = 4, //the interactions are blocked --> Set by general scripts. When in this state interaction are stopped and the interactable script is usually set to false
            BlockedByPermissions = 8, //interactions blocked by a missing permission
            BlockedByLockObject = 16, //interactions blocked because someone has locked the object
        }

        //set the currentBlockedState to none
        protected EBlockedState currentBlockedState = 0; //set interaction to nothing at the beginning
        public virtual EBlockedState CurrentBlockedState
        {
            get => currentBlockedState;
            set
            {
                currentBlockedState = value;
                OnCurrentBlockedChanged.Invoke(value);
            }
        }

        public UnityEvent<EBlockedState> OnCurrentBlockedChanged { get; set; } = new();

        public abstract Task Setup();

        protected virtual async void Awake()
        {
            if (setupOnAwake)
            {
                await InteractableRef.Setup(needSubmeshes);
                await Setup();
            }
        }



        //public void EnterInteractionState()
        //{
        //    InteractableRef.IsInteracted = true;
        //}

        //public void ExitInteractionState()
        //{
        //    if (InteractableRef != null)
        //    {
        //        InteractableRef.IsInteracted = false;
        //    }
        //}
    }
}
