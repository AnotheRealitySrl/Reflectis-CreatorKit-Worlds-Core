using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis ContextualMenuManageable: BlockInteractionBySelection")]
    [UnitSurtitle("ContextualMenuManageable")]
    [UnitShortTitle("BlockInteractionBySelection")]
    [UnitCategory("Reflectis\\Flow")]
    public abstract class SendDiagnosticUnitBase : Unit
    {

        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput ContextualMenuManageable { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput BlockValue { get; private set; }

        protected override void Definition()
        {
            ContextualMenuManageable = ValueInput<ContextualMenuManageable>(nameof(ContextualMenuManageable));

            BlockValue = ValueInput<bool>(nameof(BlockValue), false);

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {

                if (f.GetValue<bool>(BlockValue))
                {
                    f.GetValue<ContextualMenuManageable>(ContextualMenuManageable).CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                else
                {
                    f.GetValue<ContextualMenuManageable>(ContextualMenuManageable).CurrentBlockedState = f.GetValue<ContextualMenuManageable>(ContextualMenuManageable).CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);

        }
    }
}
