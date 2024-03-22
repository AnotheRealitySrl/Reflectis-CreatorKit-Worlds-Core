using Reflectis.SDK.Avatars;
using Reflectis.SDK.Core;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Character: Enable Mesh")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Enable Mesh")]
    [UnitCategory("Reflectis\\Flow")]
    public class EnableCharacterMeshNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput Enable { get; private set; }

        protected override void Definition()
        {
            Enable = ValueInput<bool>(nameof(Enable));

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(f.GetValue<bool>(Enable));

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
