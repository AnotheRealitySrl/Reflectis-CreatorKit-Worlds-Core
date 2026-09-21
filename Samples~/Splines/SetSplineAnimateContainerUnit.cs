using Unity.Mathematics;
using Unity.VisualScripting;

using UnityEngine.Splines;

namespace Virtuademy.SDK.Environments.VisualScripting.Splines
{
    [UnitTitle("Virtuademy SplineAnimate: Set Container")]
    [UnitSurtitle("SplineAnimate")]
    [UnitShortTitle("Set Container")]
    [UnitCategory("Virtuademy\\Flow")]

    [RenamedFrom("SetSplineAnimateContainerUnit")]

    public class SetSplineAnimateContainerUnit : Unit
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
        [PortLabel("Container")]
        public ValueInput SplineContainerInput { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabel("Move Backwards")]
        public ValueInput MoveBackwardsInput { get; private set; }

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
            SplineContainerInput = ValueInput<SplineContainer>(nameof(SplineContainerInput), null).NullMeansSelf();
            MoveBackwardsInput = ValueInput<bool>(nameof(MoveBackwardsInput));

            SplineAnimateOutput = ValueOutput(nameof(SplineAnimateOutput), (flow) => flow.GetValue<SplineAnimate>(SplineAnimateInput));

            InputTrigger = ControlInput(nameof(InputTrigger), (flow) =>
            {
                SplineAnimate sa = flow.GetValue<SplineAnimate>(SplineAnimateInput);
                bool mb = flow.GetValue<bool>(MoveBackwardsInput);

                sa.Container = flow.GetValue<SplineContainer>(SplineContainerInput);

                if (sa.Container != null)
                {
                    float3 pos, tan, up;
                    if (mb)
                    {
                        sa.Container.Evaluate(1f, out pos, out tan, out up);
                    }
                    else
                    {
                        sa.Container.Evaluate(0f, out pos, out tan, out up);
                    }

                    sa.transform.position = pos;
                    sa.transform.LookAt(pos + tan, up);
                }

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}