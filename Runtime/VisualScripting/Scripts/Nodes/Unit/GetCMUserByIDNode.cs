using Reflectis.CreatorKit.Core.ClientModels;
using Reflectis.SDK.Core;
using Reflectis.SDK.Core.VisualScripting;

using System.Collections.Generic;
using System.Threading.Tasks;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis CMUser: Get CMUserByID")]
    [UnitSurtitle("CMUserByID")]
    [UnitShortTitle("Get CMUserByID")]
    [UnitCategory("Reflectis\\Get")]
    public class GetCMUserByIDNode : AwaitableUnit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput CMUser { get; private set; }

        public ValueInput UserID { get; private set; }

        private List<Flow> runningFlows = new List<Flow>();

        private CMUser cmUserData;

        protected override void Definition()
        {
            UserID = ValueInput<int>(nameof(UserID));
            CMUser = ValueOutput<CMUser>(nameof(CMUser), f => cmUserData);

            base.Definition();
        }

        protected async override Task AwaitableAction(Flow flow)
        {
            cmUserData = await SM.GetSystem<IClientModelSystem>().GetUserData(flow.GetValue<int>(UserID));
        }

    }
}
