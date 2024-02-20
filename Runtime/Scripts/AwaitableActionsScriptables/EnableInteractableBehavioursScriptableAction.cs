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
                        if (!activate)
                        {
                            manipulable.CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        else
                        {
                            manipulable.CurrentBlockedState = manipulable.CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                            //manipulable.CurrentBlockedState = InteractableBehaviourBase.EBlockedState.Idle;
                            //remove blockedBySelection
                        }
                        manipulable.enabled = activate;
                    }

                    if (beh is GenericInteractable genericInteractable && interactionsToEnable.HasFlag(EInteractableType.GenericInteractable))
                    {
                        if (!activate)
                        {
                            genericInteractable.CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        else
                        {
                            //remove blockedBySelection
                            genericInteractable.CurrentBlockedState = genericInteractable.CurrentBlockedState &~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                            //genericInteractable.CurrentBlockedState = InteractableBehaviourBase.EBlockedState.Idle;
                        }
                        genericInteractable.enabled = activate;
                        //genericInteractable.CanInteract = activate;
                    }

                    if (beh is ContextualMenuManageable manageable && interactionsToEnable.HasFlag(EInteractableType.ContextualMenuInteractable))
                    {
                        if (!activate)
                        {
                            manageable.CurrentBlockedState |=  InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        else
                        {
                            //remove blockedBySelection
                            manageable.CurrentBlockedState = manageable.CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        manageable.enabled = activate;
                        //manageable.CanInteract = activate;
                    }
                };
            }

            return Task.CompletedTask;
        }
    }
}
