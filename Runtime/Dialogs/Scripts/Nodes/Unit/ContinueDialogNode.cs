using SPACS.Dialogs;
using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.Dialogs
{
    [UnitTitle("Reflectis Dialogs: Continue Dialog")]
    [UnitSurtitle("Dialogs")]
    [UnitShortTitle("Continue Dialog")]
    [UnitCategory("Reflectis\\Flow")]
    public class ContinueDialogNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        public ValueInput DialogSystem { get; private set; }
        [PortLabel("Dialog Option")]
        public ValueInput DialogOption { get; private set; }

        protected override void Definition()
        {
            DialogSystem = ValueInput<DialogSystem>(nameof(DialogSystem));
            DialogOption = ValueInput<int>(nameof(DialogOption), 0);

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                DialogSystem targetDialogSystem = f.GetValue<DialogSystem>(DialogSystem);
                int dialogOption = f.GetValue<int>(DialogOption);
                targetDialogSystem.ContinueDialog(dialogOption);

                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
