using Unity.VisualScripting;
using Unity.VisualScripting;

using UnityEngine.Splines;

namespace Virtuademy.SDK.Environments.VisualScripting.Splines
{
    [UnitTitle("Virtuademy SplineAnimate: Set Max Speed")]
    [UnitSurtitle("SplineAnimate")]
    [UnitShortTitle("Set Max Speed")]
    [UnitCategory("Virtuademy\\Flow")]

    [RenamedFrom("SetSplineAnimateMaxSpeedUnit")]

    public class SetSplineAnimateMaxSpeedUnit : Unit
    {
        // Input
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput SplineAnimateInput { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabel("Max Speed")]
        public ValueInput MaxSpeedInput { get; private set; }

        // Output
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput SplineAnimateOutput { get; private set; }

        protected override void Definition()
        {
            SplineAnimateInput = ValueInput<SplineAnimate>(nameof(SplineAnimateInput), null).NullMeansSelf();
            MaxSpeedInput = ValueInput<float>(nameof(MaxSpeedInput), 1f);

            SplineAnimateOutput = ValueOutput(nameof(SplineAnimateOutput), (flow) => flow.GetValue<SplineAnimate>(SplineAnimateInput));

            InputTrigger = ControlInput(nameof(InputTrigger), (flow) =>
            {
                flow.GetValue<SplineAnimate>(SplineAnimateInput).MaxSpeed = flow.GetValue<float>(MaxSpeedInput);

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}