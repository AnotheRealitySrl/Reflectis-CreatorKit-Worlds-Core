using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis GenericInteractable: BlockInteractionBySelection")]
    [UnitSurtitle("GenericInteractable")]
    [UnitShortTitle("BlockInteractionBySelection")]
    [UnitCategory("Reflectis\\Flow")]
    public class BlockBySelectionGenericInteractableUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput GenericInteractable { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput BlockValue { get; private set; }

        protected override void Definition()
        {
            GenericInteractable = ValueInput<GenericInteractable>(nameof(GenericInteractable));

            BlockValue = ValueInput<bool>(nameof(BlockValue), false);

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {

                if (f.GetValue<bool>(BlockValue))
                {
                    f.GetValue<GenericInteractable>(GenericInteractable).CurrentBlockedState |= InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                else
                {
                    f.GetValue<GenericInteractable>(GenericInteractable).CurrentBlockedState = f.GetValue<GenericInteractable>(GenericInteractable).CurrentBlockedState & ~InteractableBehaviourBase.EBlockedState.BlockedBySelection;
                }
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);

        }
    }
}
