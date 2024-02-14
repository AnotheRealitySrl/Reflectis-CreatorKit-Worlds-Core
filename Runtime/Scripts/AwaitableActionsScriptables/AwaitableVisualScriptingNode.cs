using Reflectis.SDK.InteractionNew;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/AwaitableVisualScriptingNode", fileName = "AwaitableVisualScriptingNode")]
    public class AwaitableVisualScriptingNode : AwaitableScriptableAction
    {
        [SerializeField]
        private string referenceNodeName;

        public override async Task Action(IInteractable interactable)
        {
            foreach (var graphReference in OnAwaitableEventUnit.graphReferences)
            {
                if (graphReference.Key == interactable.GameObjectRef)
                {
                    foreach (var graphReference2 in graphReference.Value)
                    {
                        var interactablePlaceholder = graphReference2.gameObject.GetComponent<InteractablePlaceholder>();

                        if (interactablePlaceholder)
                        {
                            foreach (var instance in OnAwaitableEventUnit.instances)
                            {
                                if (instance.Key != graphReference2)
                                {
                                    continue;
                                }

                                foreach (var instance2 in instance.Value)
                                {
                                    EventUnit<(InteractablePlaceholder, string)> eventUnit = instance2;
                                    eventUnit.Trigger(graphReference2, (interactablePlaceholder, referenceNodeName));
                                }
                                break;
                            }

                            if (interactablePlaceholder.CurrentEventCount > 0)
                            {
                                while (interactablePlaceholder.CurrentEventCount > 0)
                                {
                                    await Task.Yield();
                                }
                            }
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    break;
                }
            }

            await Task.CompletedTask;
        }
    }
}
