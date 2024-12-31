using Reflectis.ClientModels;
using Reflectis.SDK.Core;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis CMUser: Get CMUser")]
    [UnitSurtitle("CMUser")]
    [UnitShortTitle("Get CMUser")]
    [UnitCategory("Reflectis\\Get")]
    public class GetCMUserNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput CMUser { get; private set; }

        protected override void Definition()
        {
            CMUser = ValueOutput(nameof(CMUser), (f) => SM.GetSystem<IClientModelSystem>().UserData);
        }
    }
}
