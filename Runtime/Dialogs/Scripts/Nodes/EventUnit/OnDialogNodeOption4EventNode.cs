using SPACS.Dialogs;
using Unity.VisualScripting;
using UnityEngine.Events;


using SPACS.VisualScripting;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Virtuademy Dialogs: On Dialog Node Option 4")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("On Dialog Node Option 4")]
    [UnitCategory("Events\\Virtuademy")]
    public class OnDialogNodeOption4EventNode : UnityEventUnit<Null>
    {
        public static string eventName = "OnDialogNodeOption4";

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
            return Flow.New(reference).GetValue<DialogPart>(DialogPartReference).Node.onOption4;
        }

        protected override Null GetArguments(GraphReference reference)
        {
            return null;
        }
    }
}
