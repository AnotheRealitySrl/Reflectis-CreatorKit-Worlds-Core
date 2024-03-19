using Reflectis.SDK.Core;
using Reflectis.SDK.Help;
using System.Collections;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Tutorial: Enable")]
    [UnitSurtitle("Tutorial")]
    [UnitShortTitle("Enable")]
    [UnitCategory("Reflectis\\Flow")]
    public class ManageTutorialActionNode : Unit
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

        bool awaitableMethodRuning = false;

        protected override void Definition()
        {
            Enable = ValueInput<bool>(nameof(Enable));

            InputTrigger = ControlInputCoroutine(nameof(InputTrigger), ManageTutorialCoroutine);

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }

        private IEnumerator ManageTutorialCoroutine(Flow flow)
        {
            awaitableMethodRuning = false;

            CallAwaitableMethod(flow);

            while (!awaitableMethodRuning)
            {
                yield return null;
            }

            yield return OutputTrigger;
        }

        private async void CallAwaitableMethod(Flow flow)
        {
            var helpSystem = SM.GetSystem<IHelpSystem>();

            if (flow.GetValue<bool>(Enable))
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
