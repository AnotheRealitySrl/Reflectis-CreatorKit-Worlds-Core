using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Character: Set First Person Camera Mode")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Set First Person Mode")]
    [UnitCategory("Reflectis\\Flow")]
    public class SetFirstPersonCameraModeNode : Unit
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
                SM.GetSystem<ICharacterControllerSystem>().SetFirstPersonCameraMode();
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
