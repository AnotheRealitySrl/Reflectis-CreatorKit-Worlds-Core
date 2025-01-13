using Reflectis.CreatorKit.Core.ClientModels;
using Reflectis.SDK.Core;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Synced Object: Is Owned Locally")]
    [UnitSurtitle("Synced Object")]
    [UnitShortTitle("Is Owned Locally")]
    [UnitCategory("Reflectis\\Flow")]
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
        public ValueInput SyncedObject { get; private set; }

        protected override void Definition()
        {
            SyncedObject = ValueInput<SyncedObject>(nameof(SyncedObject), null).NullMeansSelf();
            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                if (!SM.GetSystem<IClientModelSystem>().CurrentEvent.Multiplayer
                || !f.GetValue<SyncedObject>(SyncedObject).IsNetworked
                || f.GetValue<SyncedObject>(SyncedObject).OnCheckOwnershipFunction())
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
    }
}
