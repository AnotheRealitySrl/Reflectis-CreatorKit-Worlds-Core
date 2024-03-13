using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Synced Object: If Owned Locally")]
    [UnitSurtitle("Synced Object")]
    [UnitShortTitle("Check if Owned Locally")]
    [UnitCategory("ReflectisUnit\\Ownership")]
    public class CheckOwnershipNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabel("True")]
        public ControlOutput OutputTriggerTrue { get; private set; }
        [DoNotSerialize]
        [PortLabel("False")]
        public ControlOutput OutputTriggerFalse { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedObject { get; private set; }

        protected override void Definition()
        {
            syncedObject = ValueInput<SyncedObject>(nameof(syncedObject), null).NullMeansSelf();

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                if (f.GetValue<SyncedObject>(syncedObject).OnCheckOwnershipFunction())
                {
                    return OutputTriggerTrue;
                }
                else
                {
                    return OutputTriggerFalse;
                }
            });

            OutputTriggerTrue = ControlOutput(nameof(OutputTriggerTrue));
            OutputTriggerFalse = ControlOutput(nameof(OutputTriggerFalse));

            Succession(InputTrigger, OutputTriggerTrue);
            Succession(InputTrigger, OutputTriggerFalse);
        }

        private bool CheckOwnershipNode_onCheckOwnershipObject()
        {
            throw new System.NotImplementedException();
        }
    }
}
