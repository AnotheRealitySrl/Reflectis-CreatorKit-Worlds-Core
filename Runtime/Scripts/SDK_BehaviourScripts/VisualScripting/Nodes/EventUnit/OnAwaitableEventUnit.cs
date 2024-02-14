using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Awaitable Event: Awaitable action")]
    [UnitSurtitle("Awaitable Event")]
    [UnitShortTitle("On Awaitable Action")]
    [UnitCategory("Events\\Reflectis")]
    [TypeIcon(typeof(Material))]
    public class OnAwaitableEventUnit : EventUnit<(InteractablePlaceholder, string)>
    {
        public static string eventName = "AwaitableAction";

        public static Dictionary<GraphReference, List<OnAwaitableEventUnit>> instances = new Dictionary<GraphReference, List<OnAwaitableEventUnit>>();

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput interactableRef { get; private set; }

        [DoNotSerialize]
        public ValueInput variableName { get; private set; }

        public static Dictionary<GameObject, List<GraphReference>> graphReferences = new Dictionary<GameObject, List<GraphReference>>();
        protected override bool register => true;

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
                List<OnAwaitableEventUnit> variableList = new List<OnAwaitableEventUnit>
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
            interactableRef = ValueInput<InteractablePlaceholder>(nameof(interactableRef), null).NullMeansSelf();
            variableName = ValueInput<string>(nameof(variableName), null);
        }

        protected override bool ShouldTrigger(Flow flow, (InteractablePlaceholder, string) args)
        {
            if (args.Item1 == null || args.Item2 == null) return false;

            if (flow.GetValue<string>(variableName) != args.Item2) { return false; }

            if (flow.GetValue<InteractablePlaceholder>(interactableRef) == args.Item1)
            {
                if (flow.stack != null)
                {
                    args.Item1.CurrentEventCount++;
                    args.Item1.VisualScriptingFinished = false;
                    flow.stack.component.StartCoroutine(TriggerState(flow, args.Item1));
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
    }
}
