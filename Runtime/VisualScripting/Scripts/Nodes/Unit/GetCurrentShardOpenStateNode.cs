using Unity.VisualScripting;

using Virtuademy.ScriptingApi;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Networking: Get Current Shard Open State")]
    [UnitSurtitle("Networking")]
    [UnitShortTitle("Get Current Shard Open State")]
    [UnitCategory("Reflectis\\Get")]
    public class GetCurrentShardOpenStateNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabel("Is Open")]
        public ValueOutput IsOpen { get; private set; }

        protected override void Definition()
        {
            // Was bool?, where null meant "no shard at all". The grouped surface splits that
            // into HasShard and IsShardOpen rather than carrying a nullable, and this port keeps
            // the one the node was really asked for: with no shard it now reads false instead of
            // null, which is what every graph downstream already treated null as.
            IsOpen = ValueOutput<bool>(nameof(IsOpen),
                                       (f) => IVirtuademyFramework.Current.Session.IsShardOpen);
        }
    }
}
