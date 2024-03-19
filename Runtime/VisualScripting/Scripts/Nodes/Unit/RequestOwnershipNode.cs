using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Synced Object: Try Get Ownership")]
    [UnitSurtitle("Synced Object")]
    [UnitShortTitle("Try Get Ownership")]
    [UnitCategory("Reflectis\\Flow")]
    public class RequestOwnershipNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput SyncedObject { get; private set; }

        protected override void Definition()
        {
            SyncedObject = ValueInput<SyncedObject>(nameof(SyncedObject), null).NullMeansSelf();

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                f.GetValue<SyncedObject>(SyncedObject).onRequestOwnershipAction?.Invoke();
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
