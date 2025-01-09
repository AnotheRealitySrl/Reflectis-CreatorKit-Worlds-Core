using Reflectis.CreatorKit.Core;
using Reflectis.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Platform: Load Default Event")]
    [UnitSurtitle("Platform")]
    [UnitShortTitle("Load Default Event")]
    [UnitCategory("Reflectis\\Flow")]
    public class LoadDefaultEventNode : AwaitableUnit
    {
        protected override async Task AwaitableAction(Flow flow)
        {
            await IReflectisApplicationManager.Instance.LoadDefaultEvent();
        }
    }
}
