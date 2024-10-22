using Reflectis.SDK.Core;
using Reflectis.SDK.NetworkingSystem;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Networking: Set Current Shard Open State")]
    [UnitSurtitle("Networking")]
    [UnitShortTitle("Set Current Shard Open State")]
    [UnitCategory("Reflectis\\Flow")]
    public class SetCurrentShardOpenStateNode : Unit
    {
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlInput InputTrigger { get; private set; }
        [DoNotSerialize]
        [PortLabelHidden]
        public ControlOutput OutputTrigger { get; private set; }

        [NullMeansSelf]
        [DoNotSerialize]
        public ValueInput Open { get; private set; }

        protected override void Definition()
        {
            Open = ValueInput<bool>(nameof(Open), false);

            InputTrigger = ControlInput(nameof(InputTrigger), (f) =>
            {
                if (f.GetValue<bool>(Open))
                {
                    SM.GetSystem<INetworkingSystem>().OpenCurrentShard();
                }
                else
                {
                    SM.GetSystem<INetworkingSystem>().CloseCurrentShard();
                }
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
