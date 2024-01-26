using Reflectis.SDK.InteractionNew;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/TriggerEventOnVisualScripting", fileName = "TriggerEventOnVisualScripting")]
    public class EnableInteractionOnVisualScripting : AwaitableScriptableAction
    {
        [SerializeField] string eventName;

        public override Task Action(IInteractable interactable)
        {
            if (interactable == null || interactable.GameObjectRef == null)
            {
                return Task.CompletedTask;
            }

            CustomEvent.Trigger(interactable.GameObjectRef, eventName, eventName);

            return Task.CompletedTask;
        }
    }
}
