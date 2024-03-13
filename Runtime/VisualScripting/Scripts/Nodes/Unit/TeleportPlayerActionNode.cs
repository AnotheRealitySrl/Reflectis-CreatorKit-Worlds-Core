using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;
using Reflectis.SDK.Fade;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Teleport Player Action")]
    [UnitSurtitle("Teleport Player")]
    [UnitShortTitle("Teleport Player Action")]
    [UnitCategory("Events\\Reflectis\\Character")]
    [TypeIcon(typeof(Material))]
    public class TeleportPlayerActionNode : Unit
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
        public ValueInput transformVal { get; private set; }

        bool awaitableMethodRuning = false;

        protected override void Definition()
        {
            transformVal = ValueInput<Transform>(nameof(transformVal));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), TeleportPlayerCoroutine);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator TeleportPlayerCoroutine(Flow flow)
        {
            awaitableMethodRuning = false;

            SM.GetSystem<IFadeSystem>().FadeToBlack(() =>
            {
                SM.GetSystem<ICharacterControllerSystem>().MoveCharacter(new Pose(flow.GetValue<Transform>(transformVal).position, flow.GetValue<Transform>(transformVal).rotation));
                SM.GetSystem<IFadeSystem>().FadeFromBlack(() => awaitableMethodRuning = true);
            });

            while (!awaitableMethodRuning)
            {
                yield return null;
            }

            yield return outputTrigger;
        }
    }
}
