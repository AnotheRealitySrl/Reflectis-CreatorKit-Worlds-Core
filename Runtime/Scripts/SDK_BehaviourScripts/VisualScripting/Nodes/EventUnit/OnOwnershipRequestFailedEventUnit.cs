using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Synced Object: On Owner Request Failed")]
    [UnitSurtitle("Synced Object")]
    [UnitShortTitle("On Owner Request Failed")]
    [UnitCategory("Events\\Reflectis\\Ownership")]
    [TypeIcon(typeof(Material))]
    public class OnOwnershipRequestFailedEventUnit : EventUnit<SyncedObject>
    {
        public static string eventName = "SyncedObjectOnOwnerRequestFailed";
        public static Dictionary<GraphReference, List<OnOwnershipRequestFailedEventUnit>> instances = new Dictionary<GraphReference, List<OnOwnershipRequestFailedEventUnit>>();

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput syncedObjectRef { get; private set; }
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
                List<OnOwnershipRequestFailedEventUnit> variableList = new List<OnOwnershipRequestFailedEventUnit>
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
            syncedObjectRef = ValueInput<SyncedObject>(nameof(syncedObjectRef), null).NullMeansSelf();
        }

        protected override bool ShouldTrigger(Flow flow, SyncedObject args)
        {
            if (args == null)
            {
                return false;
            }
            if (flow.GetValue<SyncedObject>(syncedObjectRef) == args)
            {
                return true;
            }
            return false;
        }
    }
}
