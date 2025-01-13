using Reflectis.SDK.Core;
using Reflectis.SDK.Core.CharacterController;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Character: Set Third Person Camera Mode")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Set Third Person Mode")]
    [UnitCategory("Reflectis\\Flow")]
    public class SetThirdPersonCameraModeNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        protected override void Definition()
        {
            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                SM.GetSystem<ICharacterControllerSystem>().SetThirdPersonCameraMode();
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
