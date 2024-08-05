using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Synced Variables: On Synced Variable Changed Init")]
    [UnitSurtitle("Synced Variables Init")]
    [UnitShortTitle("On Synced Variable Changed Init")]
    [UnitCategory("Events\\Reflectis")]
    public class OnSyncedVariableInitEventUnit : EventUnit<(SyncedVariables, string)>
    {
        public static string eventName = "SyncedVariablesOnVariableChangedInit";

        [NullMeansSelf]
        [PortLabelHidden]
        [DoNotSerialize]
        public ValueInput SyncedVariablesRef { get; private set; }

        [DoNotSerialize]
        public ValueInput VariableName { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput Value { get; private set; }

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
        }

        protected override bool ShouldTrigger(Flow flow, (SyncedVariables, string) args)
        {
            return args.Item1 != null && args.Item1.variableSettings.Exists(x => x.name == args.Item2)
                && flow.GetValue<string>(VariableName) == args.Item2;
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
