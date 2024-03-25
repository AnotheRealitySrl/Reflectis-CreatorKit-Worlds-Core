using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Character: Exit Pan")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Exit Pan")]
    [UnitCategory("Reflectis\\Flow")]
    public class ExitPanNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        private List<Flow> runningFlows = new List<Flow>();

        protected override void Definition()
        {
            InputTrigger = ControlInputCoroutine(nameof(InputTrigger), PanCoroutine);

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }

        private IEnumerator PanCoroutine(Flow flow)
        {
            runningFlows.Add(flow);

            CallAwaitableMethod(flow);

            yield return new WaitUntil(() => !runningFlows.Contains(flow));

            yield return OutputTrigger;
        }
        private async void CallAwaitableMethod(Flow flow)
        {
            await SM.GetSystem<ICharacterControllerSystem>().GoToSetMovementState();

            runningFlows.Remove(flow);
        }
    }
}
