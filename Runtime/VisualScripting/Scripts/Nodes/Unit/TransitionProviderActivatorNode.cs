using Reflectis.SDK.Core.Transitions;

using Unity.VisualScripting;

using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Transition Provider: Do Transition")]
    [UnitSurtitle("Transition Provider")]
    [UnitShortTitle("Do Transition")]
    [UnitCategory("Reflectis\\Flow")]
    public class TransitionProviderActivatorNode : Unit
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
        public ValueInput Enter { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput GameobjectVal { get; private set; }

        protected override void Definition()
        {
            Enter = ValueInput<bool>(nameof(Enter));

            GameobjectVal = ValueInput<GameObject>(nameof(GameobjectVal), null).NullMeansSelf();

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                f.GetValue<GameObject>(GameobjectVal).GetComponent<AbstractTransitionProvider>().DoTransition(f.GetValue<bool>(Enter));

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
