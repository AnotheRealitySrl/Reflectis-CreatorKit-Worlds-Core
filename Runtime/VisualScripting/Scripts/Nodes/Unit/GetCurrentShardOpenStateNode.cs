using Unity.VisualScripting;

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
            IsOpen = ValueOutput(nameof(IsOpen), (f) => VirtuademyFramework.Current.IsCurrentShardOpen);
        }
    }
}
