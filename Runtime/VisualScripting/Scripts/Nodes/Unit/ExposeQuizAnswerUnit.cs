using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis: Expose Quiz Answer")]
    [UnitSurtitle("Expose")]
    [UnitShortTitle("Quiz Answer")]
    [UnitCategory("Reflectis\\Expose")]
    public class ExposeQuizAnswerUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput Answer { get; private set; }

        [DoNotSerialize]
        public ValueOutput TitleLabel { get; private set; }

        [DoNotSerialize]
        public ValueOutput TitleValue { get; private set; }

        [DoNotSerialize]
        public ValueOutput Image { get; private set; }

        [DoNotSerialize]
        public ValueOutput CorrectAnswer { get; private set; }

        [DoNotSerialize]
        public ValueOutput ScoreIfGood { get; private set; }

        [DoNotSerialize]
        public ValueOutput ScoreIfBad { get; private set; }

        [DoNotSerialize]
        public ValueOutput FeedbackLabel { get; private set; }

        [DoNotSerialize]
        public ValueOutput FeedbackValue { get; private set; }

        [DoNotSerialize]
        public ValueOutput IsSelected { get; private set; }

        [DoNotSerialize]
        public ValueOutput IsCorrectSelection { get; private set; }

        [DoNotSerialize]
        public ValueOutput Score { get; private set; }

        protected override void Definition()
        {
            Answer = ValueInput<QuizAnswer>(nameof(Answer), null).NullMeansSelf();

            // Anagraphics

            TitleLabel = ValueOutput(nameof(TitleLabel), (flow) => flow.GetValue<QuizAnswer>(Answer).TitleLabel);

            // ToDo: Fill with localized text
            TitleValue = ValueOutput(nameof(TitleValue), (flow) => flow.GetValue<QuizAnswer>(Answer).TitleLabel);

            Image = ValueOutput(nameof(Image), (flow) => flow.GetValue<QuizAnswer>(Answer).Image);

            CorrectAnswer = ValueOutput(nameof(CorrectAnswer), (flow) => flow.GetValue<QuizAnswer>(Answer).CorrectAnswer);

            ScoreIfGood = ValueOutput(nameof(ScoreIfGood), (flow) => flow.GetValue<QuizAnswer>(Answer).ScoreIfGood);

            ScoreIfBad = ValueOutput(nameof(ScoreIfBad), (flow) => flow.GetValue<QuizAnswer>(Answer).ScoreIfBad);

            FeedbackLabel = ValueOutput(nameof(FeedbackLabel), (flow) => flow.GetValue<QuizAnswer>(Answer).FeedbackLabel);

            // ToDo: Fill with localized text
            FeedbackValue = ValueOutput(nameof(FeedbackValue), (flow) => flow.GetValue<QuizAnswer>(Answer).FeedbackLabel);

            // Instance-related info

            IsSelected = ValueOutput(nameof(IsSelected), (flow) => flow.GetValue<QuizAnswer>(Answer).IsSelected);

            IsCorrectSelection = ValueOutput(nameof(IsCorrectSelection), (flow) => flow.GetValue<QuizAnswer>(Answer).IsCorrectSelection);

            Score = ValueOutput(nameof(Score), (flow) => flow.GetValue<QuizAnswer>(Answer).CurrentScore);
        }
    }
}
