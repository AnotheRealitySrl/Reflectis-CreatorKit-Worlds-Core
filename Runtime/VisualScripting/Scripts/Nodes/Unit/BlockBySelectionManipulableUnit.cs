using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Manipulable: BlockInteraction")]
    [UnitSurtitle("Manipulable")]
    [UnitShortTitle("BlockBySelection")]
    [UnitCategory("Reflectis\\Manipulable")]
    public class BlockBySelectionManipulableUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput manipulable { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput blockValue { get; private set; }

        protected override void Definition()
        {
            manipulable = ValueInput<Manipulable>(nameof(manipulable));

            blockValue = ValueInput<bool>(nameof(blockValue));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {

                if (f.GetValue<bool>(blockValue))
                {
                    f.GetValue<Manipulable>(manipulable).CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                else
                {
                    f.GetValue<Manipulable>(manipulable).CurrentBlockedState = f.GetValue<Manipulable>(manipulable).CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);

        }
    }
}
