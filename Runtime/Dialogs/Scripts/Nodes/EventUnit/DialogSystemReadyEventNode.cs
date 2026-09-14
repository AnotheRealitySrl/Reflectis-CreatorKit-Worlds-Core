using Virtuademy.SDK.Core.VisualScripting;
using SPACS.Dialogs;
using Unity.VisualScripting;
using UnityEngine.Events;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Reflectis Dialogs: Dialog System Ready")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("Dialog System Ready")]
    [UnitCategory("Events\\Reflectis")]
    public class DialogSystemReadyEventNode : UnityEventUnit<DialogSystem>
    {
        public static string eventName = "DialogSystemReady";

        protected override bool register => true;

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

            return new EventHook(eventName);
        }

        protected override UnityEvent GetEvent(GraphReference reference)
        {
            return Flow.New(reference).GetValue<DialogSystem>(DialogSystemReference).dialogSystemReady;
        }

        protected override DialogSystem GetArguments(GraphReference reference)
        {
            return Flow.New(reference).GetValue<DialogSystem>(DialogSystemReference);
        }
    }
}
