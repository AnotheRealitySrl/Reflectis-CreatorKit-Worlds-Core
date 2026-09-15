using SPACS.Tasks;
using Unity.VisualScripting;
using UnityEngine.Events;


using SPACS.VisualScripting;

namespace Virtuademy.SDK.Environments.Tasks
{
    [UnitTitle("Reflectis Tasks: On Task System Ready")]
    [UnitSurtitle("Tasks")]
    [UnitShortTitle("On Task System Ready")]
    [UnitCategory("Events\\Reflectis")]
    public class OnTaskSystemReadyEventUnit : UnityEventUnit<TaskSystemReflectis>
    {
        protected override bool register => true;

        [DoNotSerialize]
        public ValueInput TaskSystemReference { get; private set; }

        protected override void Definition()
        {
            base.Definition();
            TaskSystemReference = ValueInput<Task>(nameof(TaskSystemReference), null).NullMeansSelf();
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook("Task" + this.ToString().Split("EventUnit")[0]);
        }

        protected override UnityEvent GetEvent(GraphReference reference)
        {
            var taskSystemReference = Flow.New(reference).GetValue<TaskSystemReflectis>(TaskSystemReference);
            if (taskSystemReference == null)
            {
                return new UnityEvent();
            }
            else
            {
                return taskSystemReference.OnTaskSystemReady;
            }
        }

        protected override TaskSystemReflectis GetArguments(GraphReference reference)
        {
            return Flow.New(reference).GetValue<TaskSystemReflectis>(TaskSystemReference);
        }
    }
}
