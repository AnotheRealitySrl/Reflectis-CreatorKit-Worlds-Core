using SPACS.Dialogs;
using Unity.VisualScripting;
using UnityEngine.Events;
using static SPACS.Dialogs.DialogNode;


using SPACS.VisualScripting;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Virtuademy Dialogs: Dialog Node Status Changed")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("Dialog Node Status Changed")]
    [UnitCategory("Events\\Virtuademy")]
    public class OnDialogNodeStatusChangedEventNode : UnityEventUnit<(DialogStatus, DialogStatus), DialogStatus>
    {
        public static string eventName = "OnDialogNodeStatusChanged";

        protected override bool register => true;

        [DoNotSerialize]
        [PortLabel("Dialog Part")]
        public ValueInput DialogPartReference { get; private set; }

        [DoNotSerialize]
        [PortLabel("Old Dialog Status")]
        public ValueOutput OldDialogStatus { get; private set; }

        [DoNotSerialize]
        [PortLabel("New Dialog Status")]
        public ValueOutput NewDialogStatus { get; private set; }

        protected override void Definition()
        {
            base.Definition();
            DialogPartReference = ValueInput<DialogSystem>(nameof(DialogPartReference));
            OldDialogStatus = ValueOutput<DialogNode.DialogStatus>(nameof(OldDialogStatus));
            NewDialogStatus = ValueOutput<DialogNode.DialogStatus>(nameof(NewDialogStatus));
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void AssignArguments(Flow flow, (DialogNode.DialogStatus, DialogNode.DialogStatus) args)
        {
            flow.SetValue(OldDialogStatus, args.Item1);
            flow.SetValue(NewDialogStatus, args.Item2);
        }


        protected override UnityEvent<DialogStatus> GetEvent(GraphReference reference)
        {
            return Flow.New(reference).GetValue<DialogPart>(DialogPartReference).Node.onStatusChanged;
        }

        protected override (DialogStatus, DialogStatus) GetArguments(GraphReference reference, DialogStatus eventData)
        {
            return (eventData, Flow.New(reference).GetValue<DialogPart>(DialogPartReference).Node.Status);
        }
    }
}
