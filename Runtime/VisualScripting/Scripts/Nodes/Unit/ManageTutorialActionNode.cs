using Reflectis.SDK.Core;
using Reflectis.SDK.Help;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Manage Tutorial Action")]
    [UnitSurtitle("Tutorial")]
    [UnitShortTitle("Open or Close Tutorial")]
    [UnitCategory("Events\\Reflectis\\Tutorial")]
    [TypeIcon(typeof(Material))]
    public class ManageTutorialActionNode : Unit
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

        bool awaitableMethodRuning = false;

        protected override void Definition()
        {
            boolVal = ValueInput<bool>(nameof(boolVal));

            inputTrigger = ControlInputCoroutine(nameof(inputTrigger), ManageTutorialCoroutine);

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }

        private IEnumerator ManageTutorialCoroutine(Flow flow)
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
            var helpSystem = SM.GetSystem<IHelpSystem>();

            if (flow.GetValue<bool>(boolVal))
            {
                await helpSystem.CallGetHelp();

                awaitableMethodRuning = true;
            }
            else
            {
                await helpSystem.CallCloseGetHelp();

                awaitableMethodRuning = true;
            }
        }
    }
}
