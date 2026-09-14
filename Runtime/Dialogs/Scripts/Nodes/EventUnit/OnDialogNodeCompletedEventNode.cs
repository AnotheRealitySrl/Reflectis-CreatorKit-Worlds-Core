using Virtuademy.SDK.Core.VisualScripting;
using SPACS.Dialogs;
using Unity.VisualScripting;
using UnityEngine.Events;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Reflectis Dialogs: On Dialog Node Complete")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("On Dialog Node Complete")]
    [UnitCategory("Events\\Reflectis")]
    public class OnDialogNodeCompletedEventNode : UnityEventUnit<Null>
    {
        public static string eventName = "OnDialogNodeCompleted";

        protected override bool register => true;

        [DoNotSerialize]
        [PortLabel("Dialog Part")]
        public ValueInput DialogPartReference { get; private set; }

        protected override void Definition()
        {
            base.Definition();
            DialogPartReference = ValueInput<DialogSystem>(nameof(DialogPartReference));
        }


        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override UnityEvent GetEvent(GraphReference reference)
        {
            return Flow.New(reference).GetValue<DialogPart>(DialogPartReference).OnDialogCompleted;
        }

        protected override Null GetArguments(GraphReference reference)
        {
            return null;
        }
    }
}
