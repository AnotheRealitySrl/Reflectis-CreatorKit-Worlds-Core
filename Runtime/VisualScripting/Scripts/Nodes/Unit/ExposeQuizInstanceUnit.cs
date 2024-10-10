using Reflectis.SDK.InteractionNew;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis: Expose Quiz Instance")]
    [UnitSurtitle("Expose")]
    [UnitShortTitle("Quiz Instance")]
    [UnitCategory("Reflectis\\Expose")]
    public class ExposeQuizInstanceUnit : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput Quiz { get; private set; }

        [DoNotSerialize]
        public ValueOutput GameObjectReference { get; private set; }

        [DoNotSerialize]
        public ValueOutput TitleLabel { get; private set; }

        [DoNotSerialize]
        public ValueOutput TitleValue { get; private set; }

        [DoNotSerialize]
        public ValueOutput DescriptionLabel { get; private set; }

        [DoNotSerialize]
        public ValueOutput DescriptionValue { get; private set; }

        [DoNotSerialize]
        public ValueOutput Answers { get; private set; }

        [DoNotSerialize]
        public ValueOutput AnswersCount { get; private set; }

        [DoNotSerialize]
        public ValueOutput ScoreIfAllGood { get; private set; }

        [DoNotSerialize]
        public ValueOutput ScoreIfAllBad { get; private set; }

        [DoNotSerialize]
        public ValueOutput CorrectAnswers { get; private set; }

        [DoNotSerialize]
        public ValueOutput Score { get; private set; }

        protected override void Definition()
        {
            Quiz = ValueInput<QuizPlaceholder>(nameof(Quiz), null).NullMeansSelf();

            GameObjectReference = ValueOutput(nameof(GameObjectReference), (flow) => flow.GetValue<QuizPlaceholder>(Quiz).gameObject);

            // Anagraphics

            TitleLabel = ValueOutput(nameof(TitleLabel), (flow) => flow.GetValue<QuizPlaceholder>(Quiz).TitleLabel);

            // ToDo: Fill with localized text
            TitleValue = ValueOutput(nameof(TitleValue), (flow) =>
            {
                QuizPlaceholder ans = flow.GetValue<QuizPlaceholder>(Quiz);
                string locLbl = ans.TitleLabel;
                string locVal = ans.QuizInstanceTitleValue;

                return !string.IsNullOrEmpty(locVal) ? locVal : locLbl;
            });

            DescriptionLabel = ValueOutput(nameof(DescriptionLabel), (flow) => flow.GetValue<QuizPlaceholder>(Quiz).DescriptionLabel);

            // ToDo: Fill with localized text
            DescriptionValue = ValueOutput(nameof(DescriptionValue), (flow) =>
            {
                QuizPlaceholder ans = flow.GetValue<QuizPlaceholder>(Quiz);
                string locLbl = ans.DescriptionLabel;
                string locVal = ans.QuizInstanceDescriptionValue;

                return !string.IsNullOrEmpty(locVal) ? locVal : locLbl;
            });

            // Instance-related info

            Answers = ValueOutput(nameof(Answers), (flow) => flow.GetValue<QuizPlaceholder>(Quiz).QuizInstanceAnswers);

            AnswersCount = ValueOutput(nameof(AnswersCount), (flow) => flow.GetValue<QuizPlaceholder>(Quiz).QuizInstanceAnswersCount);

            ScoreIfAllGood = ValueOutput(nameof(ScoreIfAllGood), (flow) => flow.GetValue<QuizPlaceholder>(Quiz).QuizInstanceAllGoodScore);

            ScoreIfAllBad = ValueOutput(nameof(ScoreIfAllBad), (flow) => flow.GetValue<QuizPlaceholder>(Quiz).QuizInstanceAllBadScore);

            CorrectAnswers = ValueOutput(nameof(CorrectAnswers), (flow) => flow.GetValue<QuizPlaceholder>(Quiz).QuizInstanceCorrectAnswersCount);

            Score = ValueOutput(nameof(Score), (flow) => flow.GetValue<QuizPlaceholder>(Quiz).QuizInstanceScore);
        }
    }
}
