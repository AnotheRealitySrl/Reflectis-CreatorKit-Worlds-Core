using Reflectis.SDK.Core;
using Reflectis.SDK.Diagnostics;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Diagnostic: GetNetworkGUID")]
    [UnitSurtitle("Reflectis Diagnostic")]
    [UnitShortTitle("Get Network Guid")]
    [UnitCategory("Reflectis\\Get")]
    public class GetDiagnosticNetworkGUIDUnit : Unit
    {

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput NetworkGUID { get; private set; }

        protected override void Definition()
        {

            NetworkGUID = ValueOutput(nameof(NetworkGUID),
                (flow) => SM.GetSystem<IDiagnosticsSystem>().GetUniqueNetworkID());
        }


    }

}
