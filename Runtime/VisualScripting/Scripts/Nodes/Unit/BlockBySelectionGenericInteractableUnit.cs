using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("GenericInteractable: BlockInteraction")]
    [UnitSurtitle("GenericInteractable")]
    [UnitShortTitle("BlockBySelection")]
    [UnitCategory("Reflectis\\GenericInteractable")]
    public class BlockBySelectionGenericInteractableUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput genericInteractable { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput blockValue { get; private set; }

        protected override void Definition()
        {
            genericInteractable = ValueInput<GenericInteractable>(nameof(genericInteractable));

            blockValue = ValueInput<bool>(nameof(blockValue), false);

            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {

                if (f.GetValue<bool>(blockValue))
                {
                    f.GetValue<GenericInteractable>(genericInteractable).CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                else
                {
                    f.GetValue<GenericInteractable>(genericInteractable).CurrentBlockedState = f.GetValue<GenericInteractable>(genericInteractable).CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);

        }
    }
}
