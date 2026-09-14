using Virtuademy.SDK.Tasks;
using Unity.VisualScripting;
using UnityEngine.Events;


using SPACS.VisualScripting;

namespace Virtuademy.SDK.Environments.Tasks
{
    [UnitTitle("Reflectis Tasks: On Task Completed")]
    [UnitSurtitle("Tasks")]
    [UnitShortTitle("On Task Completed")]
    [UnitCategory("Events\\Reflectis")]
    public class OnTaskCompletedEventUnit : UnityEventUnit<Task, bool>
    {
        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput TaskReference { get; private set; }

        protected override void Definition()
        {
            base.Definition();
            TaskReference = ValueInput<Task>(nameof(TaskReference), null).NullMeansSelf();
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook("Task" + this.ToString().Split("EventUnit")[0]);
        }

        protected override UnityEvent<bool> GetEvent(GraphReference reference)
        {
            var taskReference = Flow.New(reference).GetValue<Task>(TaskReference);
            if (taskReference == null)
            {
                return new UnityEvent<bool>();
            }
            else
            {
                return taskReference.OnTaskCompleted;
            }
        }

        protected override Task GetArguments(GraphReference reference, bool value)
        {
            return Flow.New(reference).GetValue<Task>(TaskReference);
        }
    }
}
