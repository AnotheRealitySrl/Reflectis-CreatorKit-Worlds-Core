using Reflectis.SDK.InteractionNew;

using System.Threading.Tasks;

using UnityEngine;

using static Reflectis.SDK.InteractionNew.IInteractable;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/EnableInteractableBehavioursScriptableAction", fileName = "EnableInteractableBehavioursScriptableAction")]
    public class EnableInteractableBehavioursScriptableAction : AwaitableScriptableAction
    {
        [SerializeField] private bool activate;
        [SerializeField] private EInteractableType interactionsToEnable;

        public override Task Action(IInteractable interactable = null)
        {
            if (interactable != null)
            {
                foreach (var beh in interactable.InteractableBehaviours)
                {
                    if (beh is Manipulable manipulable && interactionsToEnable.HasFlag(EInteractableType.Manipulable))
                    {
                        //TODO Refactor of CanInteract/Ownership/CanManipulate
                        if (manipulable.CurrentInteractionBehaviourState != InteractableBehaviourBase.EInteractionState.Blocked)
                        {
                            if (activate)
                            {
                                manipulable.CurrentInteractionBehaviourState = InteractableBehaviourBase.EInteractionState.Selected;
                            }
                            else
                            {
                                manipulable.CurrentInteractionBehaviourState = InteractableBehaviourBase.EInteractionState.Idle;
                            }
                            manipulable.enabled = activate;
                        }
                    }

                    if (beh is GenericInteractable genericInteractable && interactionsToEnable.HasFlag(EInteractableType.GenericInteractable))
                    {
                        genericInteractable.enabled = activate;
                        genericInteractable.CanInteract = activate;
                    }

                    if (beh is ContextualMenuManageable manageable && interactionsToEnable.HasFlag(EInteractableType.ContextualMenuInteractable))
                    {
                        manageable.enabled = activate;
                        manageable.CanInteract = activate;
                    }
                };
            }

            return Task.CompletedTask;
        }
    }
}
