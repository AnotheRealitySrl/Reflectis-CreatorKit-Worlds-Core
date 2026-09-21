using SPACS.Dialogs;
using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Virtuademy Dialogs: Activate Dialog")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("Activate Dialog")]
    [UnitCategory("Virtuademy\\Flow")]
    public class ActivateDialogNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        //[DoNotSerialize]
        //[Serialize]
        public ValueInput Talkable { get; private set; }

        protected override void Definition()
        {
            Talkable = ValueInput<Talkable>(nameof(Talkable));

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                f.GetValue<Talkable>(Talkable).ActivateDialog();

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
