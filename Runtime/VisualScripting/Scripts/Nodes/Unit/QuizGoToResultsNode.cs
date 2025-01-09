using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Quiz: Go To Results")]
    [UnitSurtitle("Quiz")]
    [UnitShortTitle("Go To Results")]
    [UnitCategory("Reflectis\\Flow")]
    public class QuizGoToResultsNode : Unit
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

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput ShowEditButton { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput ShowResetButton { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput ShowContinueButton { get; private set; }

        protected override void Definition()
        {
            Quiz = ValueInput<QuizPlaceholder>(nameof(QuizPlaceholder), null).NullMeansSelf();
            ShowEditButton = ValueInput<bool>(nameof(ShowEditButton), false);
            ShowResetButton = ValueInput<bool>(nameof(ShowResetButton), false);
            ShowContinueButton = ValueInput<bool>(nameof(ShowContinueButton), false);

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                f.GetValue<QuizPlaceholder>(Quiz).VSNode_GoToResults(
                    f.GetValue<bool>(ShowEditButton),
                    f.GetValue<bool>(ShowResetButton),
                    f.GetValue<bool>(ShowContinueButton));
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
