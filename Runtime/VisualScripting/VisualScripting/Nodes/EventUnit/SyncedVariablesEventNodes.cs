using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Synced Variables: On Variable Changed")]
    [UnitSurtitle("Synced Variables")]
    [UnitShortTitle("On Synced Variable Changed")]
    [UnitCategory("Events\\Reflectis\\Synced Object")]
    [TypeIcon(typeof(Material))]
    public class SyncedVariablesEventNodes : EventUnit<(SyncedVariables, string)>
    {
        public static string eventName = "SyncedVariablesOnVariableChanged";

        public static Dictionary<GraphReference, List<SyncedVariablesEventNodes>> instances = new Dictionary<GraphReference, List<SyncedVariablesEventNodes>>();

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput syncedVariablesRef { get; private set; }

        [DoNotSerialize]
        public ValueInput variableName { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

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
                List<SyncedVariablesEventNodes> variableList = new List<SyncedVariablesEventNodes>
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
            syncedVariablesRef = ValueInput<SyncedVariables>(nameof(syncedVariablesRef), null).NullMeansSelf();
            variableName = ValueInput<string>(nameof(variableName), null);
            value = ValueOutput<object>(nameof(value));
        }

        protected override bool ShouldTrigger(Flow flow, (SyncedVariables, string) args)
        {
            if (flow.GetValue<string>(variableName) != args.Item2) { return false; }
            if (flow.GetValue<SyncedVariables>(syncedVariablesRef) == args.Item1) { }
            else if (args.Item1.variableSettings.Count != 0) { }

            if (flow.GetValue<SyncedVariables>(syncedVariablesRef) == args.Item1 && flow.GetValue<string>(variableName) == args.Item2 && args.Item1.variableSettings.Count != 0)
            {
                foreach (SyncedVariables.Data data in args.Item1.variableSettings)
                {
                    if (data.declaration == null)
                        return false;
                    else
                        return true;
                }
            }
            return false;
        }

        protected override void AssignArguments(Flow flow, (SyncedVariables, string) args)
        {
            foreach (SyncedVariables.Data data in args.Item1.variableSettings)
            {
                if (data.name == args.Item2)
                {
                    flow.SetValue(value, data.Value);
                    break;
                }
            }
        }
    }
}

