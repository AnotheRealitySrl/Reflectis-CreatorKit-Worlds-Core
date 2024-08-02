using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Synced Variables: On Synced Variable Changed")]
    [UnitSurtitle("Synced Variables")]
    [UnitShortTitle("On Synced Variable Changed")]
    [UnitCategory("Events\\Reflectis")]
    public class SyncedVariablesEventNodes : EventUnit<(SyncedVariables, string)>
    {
        public static string eventName = "SyncedVariablesOnVariableChanged";

        public static Dictionary<GraphReference, List<SyncedVariablesEventNodes>> instances = new Dictionary<GraphReference, List<SyncedVariablesEventNodes>>();

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput SyncedVariablesRef { get; private set; }

        [DoNotSerialize]
        public ValueInput VariableName { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput Value { get; private set; }

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
            SyncedVariablesRef = ValueInput<SyncedVariables>(nameof(SyncedVariablesRef), null).NullMeansSelf();
            VariableName = ValueInput<string>(nameof(VariableName), null);
            Value = ValueOutput<object>(nameof(Value));
        }

        protected override bool ShouldTrigger(Flow flow, (SyncedVariables, string) args)
        {
            if (flow.GetValue<string>(VariableName) != args.Item2) { Debug.LogError("STOP BEACAUSE OF NAME " + args.Item2 + " | " + flow.GetValue<string>(VariableName)); return false; }


            if (flow.GetValue<SyncedVariables>(SyncedVariablesRef) == args.Item1 && flow.GetValue<string>(VariableName) == args.Item2 && args.Item1.variableSettings.Count != 0)
            {
                foreach (SyncedVariables.Data data in args.Item1.variableSettings)
                {
                    if (data.declaration == null)
                    {
                        Debug.LogError("STOPED BECAUSe NO DECLARATION");
                        return false;
                    }
                    else
                    {
                        Debug.LogError("LAUNCH " + args.Item2);
                        return true;
                    }

                }
            }
            Debug.LogError("NOT LAUNCH for no reason");
            return false;
        }

        protected override void AssignArguments(Flow flow, (SyncedVariables, string) args)
        {
            foreach (SyncedVariables.Data data in args.Item1.variableSettings)
            {
                if (data.name == args.Item2)
                {
                    flow.SetValue(Value, data.DeclarationValue);
                    break;
                }
            }
        }
    }
}

