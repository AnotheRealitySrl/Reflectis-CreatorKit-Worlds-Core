using Reflectis.SDK.Avatars;
using Reflectis.SDK.Core;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Enable Character Mesh")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Enable Character Mesh")]
    [UnitCategory("Events\\Reflectis\\Character")]
    [TypeIcon(typeof(UnityEngine.CharacterController))]
    public class EnableCharacterMeshNode : Unit
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
                SM.GetSystem<AvatarSystem>().EnableAvatarInstanceMeshes(f.GetValue<bool>(boolVal));

                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
