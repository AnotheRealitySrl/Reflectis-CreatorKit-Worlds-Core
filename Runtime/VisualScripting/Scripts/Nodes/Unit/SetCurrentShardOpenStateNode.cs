using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.VisualScripting
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
                VirtuademyFramework.Current.SetCurrentShardOpen(f.GetValue<bool>(Open));
                //if (f.GetValue<bool>(Open))
                //{
                //    VirtuademyFramework.Current.SetCurrentShardOpen(true);
                //}
                //else
                //{
                //    VirtuademyFramework.Current.SetCurrentShardOpen(false);
                //}
                return OutputTrigger;
            });

            OutputTrigger = ControlOutput(nameof(OutputTrigger));

            Succession(InputTrigger, OutputTrigger);
        }
    }
}
