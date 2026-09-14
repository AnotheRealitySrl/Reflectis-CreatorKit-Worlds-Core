using SPACS.Dialogs;
using Unity.VisualScripting;
using UnityEngine.Events;


using SPACS.VisualScripting;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Reflectis Dialogs: Dialog Changed")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("Dialog Changed")]
    [UnitCategory("Events\\Reflectis")]
    public class DialogChangedEventNode : UnityEventUnit<DialogNode, DialogNode>
    {
        public static string eventName = "DialogChanged";

        protected override bool register => true;

        [DoNotSerialize]
        [PortLabel("Dialog System")]
        public ValueInput DialogSystemReference { get; private set; }

        [DoNotSerialize]
        [PortLabel("Dialog Node")]
        public ValueOutput ChangedDialogNode { get; private set; }

        protected override void Definition()
        {
            base.Definition();
            DialogSystemReference = ValueInput<DialogSystem>(nameof(DialogSystemReference));
            ChangedDialogNode = ValueOutput<DialogNode>(nameof(ChangedDialogNode));
        }

        public override EventHook GetHook(GraphReference reference)
        {
            return new EventHook(eventName);
        }

        protected override void AssignArguments(Flow flow, DialogNode args)
        {
            flow.SetValue(ChangedDialogNode, args);
        }


        protected override UnityEvent<DialogNode> GetEvent(GraphReference reference)
        {
            return Flow.New(reference).GetValue<DialogSystem>(DialogSystemReference).dialogChanged;
        }

        protected override DialogNode GetArguments(GraphReference reference, DialogNode dialogNode)
        {
            return dialogNode;
        }
    }
}
