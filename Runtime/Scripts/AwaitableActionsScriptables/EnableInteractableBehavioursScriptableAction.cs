using Reflectis.SDK.InteractionNew;

using System.Collections.Generic;
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
            List<IInteractableBehaviour> behaviours = new(interactable.InteractableBehaviours);

            behaviours.ForEach(beh =>
            {
                if (beh is Manipulable manipulable && interactionsToEnable.HasFlag(EInteractableType.Manipulable))
                    manipulable.enabled = activate;

                if (beh is GenericInteractable genericInteractable && interactionsToEnable.HasFlag(EInteractableType.GenericInteractable))
                    genericInteractable.enabled = activate;

                if (beh is ContextualMenuManageable manageable && interactionsToEnable.HasFlag(EInteractableType.ContextualMenuInteractable))
                    manageable.enabled = activate;
            });

            return Task.CompletedTask;
        }
    }
}
