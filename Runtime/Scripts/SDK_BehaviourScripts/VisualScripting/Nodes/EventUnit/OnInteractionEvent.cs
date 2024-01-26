using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Interactable: On Interaction Trigger")]
    [UnitSurtitle("Interactable")]
    [UnitShortTitle("On Interaction Trigger")]
    [UnitCategory("Events\\Reflectis\\Interaction")]
    [TypeIcon(typeof(Input))]
    public class OnInteractionEvent : EventUnit<InteractablePlaceholder>
    {
        public static string eventName = "OnInteractionTrigger";

        public static Dictionary<GraphReference, List<OnInteractionEvent>> instances = new Dictionary<GraphReference, List<OnInteractionEvent>>();

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput interactionObjRef { get; private set; }
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
                List<OnInteractionEvent> variableList = new List<OnInteractionEvent>
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
            interactionObjRef = ValueInput<InteractablePlaceholder>(nameof(interactionObjRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, InteractablePlaceholder args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<InteractablePlaceholder>(interactionObjRef) == args)
            {
                return true;
            }
            return false;
        }
    }
}
