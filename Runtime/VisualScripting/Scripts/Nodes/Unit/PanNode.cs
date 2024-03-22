using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Character: Pan")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Pan")]
    [UnitCategory("Reflectis\\Flow")]
    public class PanNode : Unit
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
        public ValueInput TargetTransform { get; private set; }

        private List<Flow> runningFlows = new List<Flow>();

        protected override void Definition()
        {
            TargetTransform = ValueInput<Transform>(nameof(TargetTransform));

            InputTrigger = ControlInputCoroutine(nameof(InputTrigger), PanCoroutine);

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }

        private IEnumerator PanCoroutine(Flow flow)
        {
            runningFlows.Add(flow);

            CallAwaitableMethod(flow);

            yield return new WaitUntil(() => runningFlows.Contains(flow));

            yield return OutputTrigger;
        }
        private async void CallAwaitableMethod(Flow flow)
        {
            await SM.GetSystem<ICharacterControllerSystem>().GoToInteractState(flow.GetValue<Transform>(TargetTransform));

            runningFlows.Remove(flow);
        }
    }
}
