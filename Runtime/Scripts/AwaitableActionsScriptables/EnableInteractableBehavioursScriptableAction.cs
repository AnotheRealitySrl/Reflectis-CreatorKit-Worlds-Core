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
                        if (!activate)
                        {
                            manipulable.CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        else
                        {
                            manipulable.CurrentBlockedState = manipulable.CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        //manipulable.enabled = activate;
                    }

                    if (beh is GenericInteractable genericInteractable && interactionsToEnable.HasFlag(EInteractableType.GenericInteractable))
                    {
                        if (!activate)
                        {
                            genericInteractable.CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        else
                        {
                            genericInteractable.CurrentBlockedState = genericInteractable.CurrentBlockedState &~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        //genericInteractable.enabled = activate;
                    }

                    if (beh is ContextualMenuManageable manageable && interactionsToEnable.HasFlag(EInteractableType.ContextualMenuInteractable))
                    {
                        if (!activate)
                        {
                            manageable.CurrentBlockedState |=  InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        else
                        {
                            manageable.CurrentBlockedState = manageable.CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                        }
                        //manageable.enabled = activate;
                    }
                };
            }

            return Task.CompletedTask;
        }
    }
}
