using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Interactable : On Selected Enter")]
    [UnitSurtitle("Interactable")]
    [UnitShortTitle("On Selected Enter")]
    [UnitCategory("Events\\Reflectis\\Interactable")]
    [TypeIcon(typeof(Material))]
    public class OnSelectedEnterEventUnit : EventUnit<InteractablePlaceholder>
    {
        public static string eventName = "InteractableOnSelectedEnter";

        public static Dictionary<GraphReference, List<OnSelectedEnterEventUnit>> instances = new Dictionary<GraphReference, List<OnSelectedEnterEventUnit>>();

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput interactableObjectRef { get; private set; }
        protected override bool register => true;

        public static Dictionary<GameObject, List<GraphReference>> graphReferences = new Dictionary<GameObject, List<GraphReference>>();

        public override EventHook GetHook(GraphReference reference)
        {
            if (graphReferences.TryGetValue(reference.gameObject, out List<GraphReference> graphRef))
            {
                if (!graphRef.Contains(reference))
                {
                    graphRef.Add(reference);
                }
            }
            else
            {
                List<GraphReference> graphReferencesList = new List<GraphReference>
                {
                    reference
                };

                graphReferences.Add(reference.gameObject, graphReferencesList);
            }

            if (instances.TryGetValue(reference, out var value))
            {
                if (!value.Contains(this))
                {
                    value.Add(this);
                }
            }
            else
            {
                List<OnSelectedEnterEventUnit> variableList = new List<OnSelectedEnterEventUnit>
                {
                    this
                };

                instances.Add(reference, variableList);
            }

            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            interactableObjectRef = ValueInput<InteractablePlaceholder>(nameof(interactableObjectRef), null).NullMeansSelf();

        }

        protected override bool ShouldTrigger(Flow flow, InteractablePlaceholder args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<InteractablePlaceholder>(interactableObjectRef) == args)
            {
                if (flow.stack != null)
                {
                    args.CurrentEventCount++;
                    args.VisualScriptingFinished = false;
                    flow.stack.component.StartCoroutine(TriggerState(flow, args));
                }
                return true;
            }
            return false;
        }

        private IEnumerator TriggerState(Flow flow, InteractablePlaceholder interactable)
        {
            Debug.Log("Visual SCripting Started");

            while (flow.stack != null)
            {
                yield return null;
            }

            Debug.Log("visual scripting finished event");

            interactable.CurrentEventCount--;

            interactable.VisualScriptingFinished = true;
        }

        public static void TriggerEventCallback(GameObject gameObject)
        {
            foreach (var graphReference in graphReferences)
            {
                if (graphReference.Key == gameObject)
                {
                    foreach (var graphReference2 in graphReference.Value)
                    {
                        var interactable = graphReference2.gameObject.GetComponent<InteractablePlaceholder>();

                        foreach (var instance in instances)
                        {
                            if (instance.Key != graphReference2)
                            {
                                continue;
                            }

                            foreach (var instance2 in instance.Value)
                            {
                                EventUnit<InteractablePlaceholder> eventUnit = instance2;
                                eventUnit.Trigger(graphReference2, interactable);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}
