using Reflectis.SDK.Avatars;
using Reflectis.SDK.Core;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Character: Enable Tag")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Enable Tag")]
    [UnitCategory("Reflectis\\Flow")]
    public class EnableCharacterTagNode : Unit
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
                Debug.Log("ENABLE TAG " + f.GetValue<bool>(Enable));
                SM.GetSystem<AvatarSystem>().EnableAvatarInstanceLabel(f.GetValue<bool>(Enable));

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
