using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Quiz: Finish")]
    [UnitSurtitle("Quiz")]
    [UnitShortTitle("Finish")]
    [UnitCategory("Reflectis\\Flow")]
    public class QuizFinishNode : Unit
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
        public ValueInput Quiz { get; private set; }

        protected override void Definition()
        {
            Quiz = ValueInput<QuizPlaceholder>(nameof(QuizPlaceholder), null).NullMeansSelf();

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                f.GetValue<QuizPlaceholder>(Quiz).VSNode_Finish();
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
