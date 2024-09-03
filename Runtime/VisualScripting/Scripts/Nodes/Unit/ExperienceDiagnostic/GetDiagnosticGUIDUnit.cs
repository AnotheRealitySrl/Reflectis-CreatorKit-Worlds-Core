using Reflectis.SDK.Core;
using Reflectis.SDK.Diagnostics;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis Diagnostic: GetGUID")]
    [UnitSurtitle("Reflectis Diagnostic")]
    [UnitShortTitle("Get Guid")]
    [UnitCategory("Reflectis\\Get")]
    public class GetDiagnosticGUIDUnit : Unit
    {

        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput GUID { get; private set; }

        protected override void Definition()
        {

            GUID = ValueOutput(nameof(GUID),
                (flow) => SM.GetSystem<IDiagnosticsSystem>().GetUniqueSessionID());
        }


    }

}
