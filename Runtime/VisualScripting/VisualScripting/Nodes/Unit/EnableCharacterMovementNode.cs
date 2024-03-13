using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Enable Character Movement")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Enable Character Movement")]
    [UnitCategory("Events\\Reflectis\\Character")]
    [TypeIcon(typeof(UnityEngine.CharacterController))]
    public class EnableCharacterMovementNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput boolVal { get; private set; }

        protected override void Definition()
        {
            boolVal = ValueInput<bool>(nameof(boolVal));

            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {
                SM.GetSystem<ICharacterControllerSystem>().EnableCharacterMovement(f.GetValue<bool>(boolVal));
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
