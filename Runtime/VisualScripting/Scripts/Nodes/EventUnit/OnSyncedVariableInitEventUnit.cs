using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Synced Variables: On Synced Variable Changed Init")]
    [UnitSurtitle("Synced Variables Init")]
    [UnitShortTitle("On Synced Variable Changed Init")]
    [UnitCategory("Events\\Reflectis\\Synced Object")]
    [TypeIcon(typeof(Material))]
    public class OnSyncedVariableInitEventUnit : EventUnit<(SyncedVariables, string)>
    {
        public static string eventName = "SyncedVariablesOnVariableChangedInit";

        public static List<OnSyncedVariableInitEventUnit> instances = new List<OnSyncedVariableInitEventUnit>();

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput syncedVariablesRef { get; private set; }

        [DoNotSerialize]
        public ValueInput variableName { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput value { get; private set; }

        public static GraphReference graphReference;
        protected override bool register => true;
        public override EventHook GetHook(GraphReference reference)
        {
            graphReference = reference;
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            syncedVariablesRef = ValueInput<SyncedVariables>(nameof(syncedVariablesRef), null).NullMeansSelf();
            variableName = ValueInput<string>(nameof(variableName), null);
            value = ValueOutput<object>(nameof(value));

            if (!instances.Contains(this))
            {
                instances.Add(this);
            }
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
