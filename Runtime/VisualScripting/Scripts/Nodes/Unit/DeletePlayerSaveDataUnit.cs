using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Player Save Data: Delete Data")]
    [UnitSurtitle("Player Save Data")]
    [UnitShortTitle("Delete Player Save Data")]
    [UnitCategory("Reflectis\\Flow")]
    public class DeletePlayerSaveDataUnit : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput Key { get; private set; }


        protected override void Definition()
        {
            Key = ValueInput<string>(nameof(Key), string.Empty);

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                VirtuademyFramework.Current.DeleteMySaveData(
                    f.GetValue<string>(Key));
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
