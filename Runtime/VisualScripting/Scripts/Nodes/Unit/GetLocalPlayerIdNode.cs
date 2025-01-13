using Reflectis.SDK.Core;
using Reflectis.SDK.Core.NetworkingSystem;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Networking: Get Local Player ID")]
    [UnitSurtitle("Networking")]
    [UnitShortTitle("Get Local Player ID")]
    [UnitCategory("Reflectis\\Get")]
    public class GetLocalPlayerIdNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabel("PlayerId")]
        public ValueOutput PlayerId { get; private set; }

        protected override void Definition()
        {
            PlayerId = ValueOutput(nameof(PlayerId), (f) => SM.GetSystem<INetworkingSystem>().GetLocalPlayerId());
        }
    }
}
