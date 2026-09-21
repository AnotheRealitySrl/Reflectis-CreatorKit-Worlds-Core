using SPACS.Dialogs;
using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Virtuademy Dialogs: Set Dialog")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("Set Dialog")]
    [UnitCategory("Virtuademy\\Flow")]
    public class SetDialogNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        public ValueInput Talkable { get; private set; }
        [PortLabel("Dialog Part")]
        public ValueInput NewDialogPart { get; private set; }

        protected override void Definition()
        {
            Talkable = ValueInput<Talkable>(nameof(Talkable));
            NewDialogPart = ValueInput<DialogPart>(nameof(NewDialogPart));

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                Talkable targetTalkable = f.GetValue<Talkable>(Talkable);
                DialogPart newDialogPart = f.GetValue<DialogPart>(NewDialogPart);
                targetTalkable.SetDialog(newDialogPart);

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
