using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Synced Variables: On Synced Variable Init")]
    [UnitSurtitle("Synced Variables Init")]
    [UnitShortTitle("On Synced Variable Init")]
    [UnitCategory("Events\\Reflectis")]
    public class OnSyncedVariableInit : EventUnit<(SyncedVariables, string)>
    {
        public static string eventName = "SyncedVariablesOnVariableInit";


        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput SyncedVariablesRef { get; private set; }

        [DoNotSerialize]
        public ValueInput VariableName { get; private set; }

        [DoNotSerialize]
        public ValueOutput Value { get; private set; }

        [DoNotSerialize]
        [Tooltip("This variable returns true if the variable in the variable name field has been changed at least once from its default state. Otherwise it returns false")]
        public ValueOutput IsChanged { get; private set; }


        protected override bool register => true;
        public override EventHook GetHook(GraphReference reference)
        {
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
            return args.Item1 != null && args.Item1.variableSettings.Exists(x => x.name == args.Item2)
                && flow.GetValue<string>(VariableName) == args.Item2;
        }

        protected override void AssignArguments(Flow flow, (SyncedVariables, string) args)
        {
            int i = 0;
            foreach (SyncedVariables.Data data in args.Item1.variableSettings)
            {
                if (data.name == args.Item2)
                {
                    flow.SetValue(Value, data.DeclarationValue);
                    if (args.Item1.variableSettings[i].hasChanged)
                    {
                        flow.SetValue(IsChanged, true);
                    }
                    else
                    {
                        flow.SetValue(IsChanged, false);
                    }
                    break;
                }
                i++;
            }
        }
    }
}
