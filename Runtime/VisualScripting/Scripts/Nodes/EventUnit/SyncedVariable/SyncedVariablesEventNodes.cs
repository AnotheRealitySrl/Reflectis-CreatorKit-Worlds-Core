using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Synced Variables: On Synced Variable Changed")]
    [UnitSurtitle("Synced Variables")]
    [UnitShortTitle("On Synced Variable Changed")]
    [UnitCategory("Events\\Reflectis")]
    public class SyncedVariablesEventNodes : SyncedVariableBaseEventUnit<(string, object)>
    {
        public static string eventName = "SyncedVariablesOnVariableChanged";


        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput Value { get; private set; }

        protected override bool register => true;

        public void Init(GraphReference reference)
        {
            GetHook(reference);
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void Definition()
        {
            base.Definition();
            Value = ValueOutput<object>(nameof(Value));
        }

        protected override bool ShouldTrigger(Flow flow, (string, object) args)
        {
            return flow.GetValue<string>(VariableName) == args.Item1;
        }


        protected override void AssignArguments(Flow flow, (string, object) args)
        {
            flow.SetValue(Value, args.Item2);
        }
    }
}

