using Reflectis.SDK.Core;
using Reflectis.SDK.Core.Avatars;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
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
