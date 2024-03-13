using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Synced Object: Release Ownership")]
    [UnitSurtitle("Synced Object")]
    [UnitShortTitle("Release Ownership")]
    [UnitCategory("Events\\Reflectis\\Ownership")]
    [TypeIcon(typeof(Material))]
    public class ReleaseOwnershipNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput inputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput outputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput syncedObject { get; private set; }

        protected override void Definition()
        {
            syncedObject = ValueInput<SyncedObject>(nameof(syncedObject), null).NullMeansSelf();

            inputTrigger = ControlInput(nameof(inputTrigger), (f) =>
            {
                f.GetValue<SyncedObject>(syncedObject).onReleaseOwnershipAction?.Invoke();
                return outputTrigger;
            });

            outputTrigger = ControlOutput(nameof(outputTrigger));

            Succession(inputTrigger, outputTrigger);
        }
    }
}
