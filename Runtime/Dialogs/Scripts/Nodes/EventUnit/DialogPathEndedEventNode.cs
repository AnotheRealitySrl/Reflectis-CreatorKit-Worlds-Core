using SPACS.Dialogs;
using Unity.VisualScripting;
using UnityEngine.Events;


using SPACS.VisualScripting;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Virtuademy Dialogs: Dialog Path Ended")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("Dialog Path Ended")]
    [UnitCategory("Events\\Virtuademy")]
    public class DialogPathEndedEventNode : UnityEventUnit<Null>
    {
        public static string eventName = "DialogPathEnded";

        protected override bool register => true;

        protected DialogSystem dialogSystemReference;

        [DoNotSerialize]
        [PortLabel("Dialog System")]
        public ValueInput DialogSystemReference { get; private set; }

        protected override void Definition()
        {
            base.Definition();
            DialogSystemReference = ValueInput<DialogSystem>(nameof(DialogSystemReference));
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return eventName;
        }
        protected override UnityEvent GetEvent(GraphReference reference)
        {
            return Flow.New(reference).GetValue<DialogSystem>(DialogSystemReference).dialogPathEnded;
        }

        protected override Null GetArguments(GraphReference reference)
        {
            return null;
        }
    }
}
