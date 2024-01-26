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
        public override Task Action(IInteractable interactable)
        {
            if (interactable == null || interactable.GameObjectRef == null)
            {
                return Task.CompletedTask;
            }

            TriggerOnOwnerChanged(OnInteractionEvent.graphReferences, interactable.GameObjectRef);

            return Task.CompletedTask;
        }

        private void TriggerOnOwnerChanged(Dictionary<GameObject, List<GraphReference>> graphReferences, GameObject interactableObj)
        {
            foreach (var graphReference in graphReferences)
            {
                if (graphReference.Key == interactableObj)
                {
                    foreach (var graphReference2 in graphReference.Value)
                    {
                        var syncedObject = graphReference2.gameObject.GetComponent<SyncedObject>();

                        foreach (var instance in OnOwnerChangedEventUnit.instances)
                        {
                            if (instance.Key != graphReference2)
                            {
                                continue;
                            }

                            foreach (var instance2 in instance.Value)
                            {
                                EventUnit<SyncedObject> eventUnit = instance2;
                                eventUnit.Trigger(graphReference2, syncedObject);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
