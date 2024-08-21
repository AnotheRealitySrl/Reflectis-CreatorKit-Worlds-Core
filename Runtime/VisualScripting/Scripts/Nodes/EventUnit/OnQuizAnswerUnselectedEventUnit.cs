using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Quiz Answer: On Quiz Answer Unselected")]
    [UnitSurtitle("Quiz Answer")]
    [UnitShortTitle("On Quiz Answer Unselected")]
    [UnitCategory("Events\\Reflectis")]
    public class OnQuizAnswerUnselectedEventUnit : AwaitableEventNode<(QuizPlaceholder, QuizAnswer)>
    {
        [DoNotSerialize]
        public ValueOutput Quiz { get; private set; }
        public ValueOutput Answer { get; private set; }

        protected override void Definition()
        {
            base.Definition();
            // Setting the value on our port.
            Quiz = ValueOutput<QuizPlaceholder>(nameof(Quiz));
            Answer = ValueOutput<QuizPlaceholder>(nameof(Answer));
        }

        // Setting the value on our port.
        protected override void AssignArguments(Flow flow, (QuizPlaceholder, QuizAnswer) data)
        {
            flow.SetValue(Quiz, data.Item1);
            flow.SetValue(Answer, data.Item2);
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook("QuizPlaceholder" + this.ToString().Split("Unit")[0]);
        }
    }
}
