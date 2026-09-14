using SPACS.Dialogs;
using Unity.VisualScripting;
using UnityEngine.Events;


using SPACS.VisualScripting;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Reflectis Dialogs: On Dialog Node Option 2")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("On Dialog Node Option 2")]
    [UnitCategory("Events\\Reflectis")]
    public class OnDialogNodeOption2EventNode : UnityEventUnit<Null>
    {
        public static string eventName = "OnDialogNodeOption2";

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
            return Flow.New(reference).GetValue<DialogPart>(DialogPartReference).Node.onOption2;
        }

        protected override Null GetArguments(GraphReference reference)
        {
            return null;
        }
    }
}
