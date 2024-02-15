using Reflectis.SDK.Transitions;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Transition Provider Activator")]
    [UnitSurtitle("Transition Provider")]
    [UnitShortTitle("Transition Provider Activator")]
    [UnitCategory("Events\\Reflectis\\Utils")]
    [TypeIcon(typeof(Component))]
    public class TransitionProviderActivatorNode : Unit
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
        public ValueInput GameobjectVal { get; private set; }

        protected override void Definition()
        {
            boolVal = ValueInput<bool>(nameof(boolVal));

            GameobjectVal = ValueInput<GameObject>(nameof(GameobjectVal), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {
                f.GetValue<GameObject>(GameobjectVal).GetComponent<AbstractTransitionProvider>().DoTransition(f.GetValue<bool>(boolVal));

                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
