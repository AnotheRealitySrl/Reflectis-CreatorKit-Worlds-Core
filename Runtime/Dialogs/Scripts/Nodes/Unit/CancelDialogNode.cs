using SPACS.Dialogs;
using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Reflectis Dialogs: Cancel Dialog")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("Cancel Dialog")]
    [UnitCategory("Reflectis\\Flow")]
    public class CancelDialogNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        public ValueInput DialogSystem { get; private set; }

        protected override void Definition()
        {
            DialogSystem = ValueInput<DialogSystem>(nameof(DialogSystem));

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                DialogSystem targetDialogSystem = f.GetValue<DialogSystem>(DialogSystem);
                targetDialogSystem.CancelDialog();

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
