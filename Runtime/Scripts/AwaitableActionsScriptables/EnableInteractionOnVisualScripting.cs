using Reflectis.SDK.InteractionNew;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/TriggerEventOnVisualScripting", fileName = "TriggerEventOnVisualScripting")]
    public class EnableInteractionOnVisualScripting : AwaitableScriptableAction
    {
        [SerializeField] List<string> eventsName;

        public override Task Action(IInteractable interactable)
        {
            if (interactable == null || interactable.GameObjectRef == null)
            {
                return Task.CompletedTask;
            }

            if (eventsName.Count != 0 && eventsName != null)
            {
                foreach (var eventName in eventsName)
                {
                    if (!string.IsNullOrEmpty(eventName) && eventName != "")
                        CustomEvent.Trigger(interactable.GameObjectRef, eventName, eventName);
                }
            }

            return Task.CompletedTask;
        }
    }
}
