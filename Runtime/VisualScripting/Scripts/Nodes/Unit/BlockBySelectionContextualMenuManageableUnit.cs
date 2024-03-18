using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("ContextualMenuManageable: BlockInteraction")]
    [UnitSurtitle("ContextualMenuManageable")]
    [UnitShortTitle("BlockBySelection")]
    [UnitCategory("Reflectis\\ContextualMenuManageable")]
    public class BlockBySelectionContextualMenuManageableUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput contextualMenuManageable { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput blockValue { get; private set; }

        protected override void Definition()
        {
            contextualMenuManageable = ValueInput<ContextualMenuManageable>(nameof(contextualMenuManageable));

            blockValue = ValueInput<bool>(nameof(blockValue), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {

                if (f.GetValue<bool>(blockValue))
                {
                    f.GetValue<ContextualMenuManageable>(contextualMenuManageable).CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                else
                {
                    f.GetValue<ContextualMenuManageable>(contextualMenuManageable).CurrentBlockedState = f.GetValue<ContextualMenuManageable>(contextualMenuManageable).CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);

        }
    }
}
