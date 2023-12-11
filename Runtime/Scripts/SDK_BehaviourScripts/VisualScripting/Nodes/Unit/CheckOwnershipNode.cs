using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Synced Object: If Owned Locally")]
    [UnitSurtitle("Synced Object")]
    [UnitShortTitle("Check if Owned Locally")]
    [UnitCategory("Events\\Reflectis\\Ownership")]
    [TypeIcon(typeof(Material))]
    public class CheckOwnershipNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabel("True")]
        public ControlOutput outputTriggerTrue { get; private set; }
        [DoNotSerialize]
        [PortLabel("False")]
        public ControlOutput outputTriggerFalse { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedObject { get; private set; }

        protected override void Definition()
        {
            syncedObject = ValueInput<SyncedObject>(nameof(syncedObject), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {
                if (f.GetValue<SyncedObject>(syncedObject).OnCheckOwnershipFunction())
                {
                    return outputTriggerTrue;
                }
                else
                {
                    return outputTriggerFalse;
                }
            });

            outputTriggerTrue = ControlOutput(nameof(outputTriggerTrue));
            outputTriggerFalse = ControlOutput(nameof(outputTriggerFalse));

            Succession(inputTrigger, outputTriggerTrue);
            Succession(inputTrigger, outputTriggerFalse);
        }

        private bool CheckOwnershipNode_onCheckOwnershipObject()
        {
            throw new System.NotImplementedException();
        }
    }
}
