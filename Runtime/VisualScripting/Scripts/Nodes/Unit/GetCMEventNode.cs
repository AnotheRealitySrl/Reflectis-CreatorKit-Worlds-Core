using Reflectis.CreatorKit.Core.ClientModels;
using Reflectis.SDK.Core;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis CMEvent: Get CMEvent")]
    [UnitSurtitle("CMEvent")]
    [UnitShortTitle("Get CMEvent")]
    [UnitCategory("Reflectis\\Get")]
    public class GetCMEventNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput CMEvent { get; private set; }

        protected override void Definition()
        {
            CMEvent = ValueOutput(nameof(CMEvent), (f) => SM.GetSystem<IClientModelSystem>().CurrentEvent);
        }
    }
}
