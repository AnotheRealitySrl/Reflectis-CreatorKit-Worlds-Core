using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Pan Character Camera")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Pan Character Camera")]
    [UnitCategory("Events\\Reflectis\\Character")]
    [TypeIcon(typeof(UnityEngine.CharacterController))]
    public class PanCharacterCameraNode : Unit
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

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput transformVal { get; private set; }

        bool awaitableMethodRuning = false;

        protected override void Definition()
        {
            boolVal = ValueInput<bool>(nameof(boolVal));

            transformVal = ValueInput<Transform>(nameof(transformVal));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), PanCoroutine);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator PanCoroutine(Flow flow)
        {
            awaitableMethodRuning = false;

            CallAwaitableMethod(flow);

            while (!awaitableMethodRuning)
            {
                yield return null;
            }

            yield return outputTrigger;
        }

        private async void CallAwaitableMethod(Flow flow)
        {
            if (flow.GetValue<bool>(boolVal))
            {
                await SM.GetSystem<ICharacterControllerSystem>().GoToInteractState(flow.GetValue<Transform>(transformVal));

                awaitableMethodRuning = true;

            }
            else
            {
                await SM.GetSystem<ICharacterControllerSystem>().GoToSetMovementState();

                awaitableMethodRuning = true;
            }
        }
    }
}
