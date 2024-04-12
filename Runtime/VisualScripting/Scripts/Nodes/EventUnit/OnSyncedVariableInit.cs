using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Synced Variables: On Synced Variable  Init")]
    [UnitSurtitle("Synced Variables Init")]
    [UnitShortTitle("On Synced Variable Init")]
    [UnitCategory("Events\\Reflectis")]
    public class OnSyncedVariableInit : EventUnit<(SyncedVariables, string)>
    {
        public static string eventName = "SyncedVariablesOnVariableInit";

        public static Dictionary<GraphReference, List<OnSyncedVariableInit>> instances = new Dictionary<GraphReference, List<OnSyncedVariableInit>>();
        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput SyncedVariablesRef { get; private set; }

        [DoNotSerialize]
        public ValueInput VariableName { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput Value { get; private set; }

        [DoNotSerialize]
        [Tooltip("This variable returns true if the variable in the variable name field has been changed at least once from its default state. Otherwise it returns false")]
        public ValueOutput IsChanged { get; private set; }

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
                List<OnSyncedVariableInit> variableList = new List<OnSyncedVariableInit>
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
            IsChanged = ValueOutput<bool>(nameof(IsChanged));
        }

        protected override bool ShouldTrigger(Flow flow, (SyncedVariables, string) args)
        {
            if (flow.GetValue<string>(VariableName) != args.Item2) { return false; }
            if (flow.GetValue<SyncedVariables>(SyncedVariablesRef) == args.Item1) { }
            else if (args.Item1.variableSettings.Count != 0) { }

            if (args.Item1.variableSettings.Count != 0)
            {
                if (flow.GetValue<SyncedVariables>(SyncedVariablesRef) == args.Item1 && flow.GetValue<string>(VariableName) == args.Item2 && args.Item1.variableSettings.Count != 0)
                {
                    int i = 0;
                    foreach (SyncedVariables.Data data in args.Item1.variableSettings)
                    {
                        if (data.declaration == null)
                        {
                            return false;
                        }
                        else
                        {
                            if (args.Item1.variableSettings[i].hasChanged)
                            {
                                flow.SetValue(IsChanged, true);
                                i++;
                            }
                            else
                            {
                                flow.SetValue(IsChanged, false);
                            }
                            return true;
                        }
                    }
                    //change boolean to true
                    //Debug.LogError("argsItem1: " + args.Item1 + "syncedVariableRef " + flow.GetValue<SyncedVariables>(SyncedVariablesRef) + "args 2" + args.Item2);
                    //flow.SetValue(IsChanged, true);
                    //IsChanged = ValueOutput(nameof(IsChanged), (f) => true);
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
                    //flow.SetValue(Value, data.Value);
                    break;
                }
            }
        }
    }
}
