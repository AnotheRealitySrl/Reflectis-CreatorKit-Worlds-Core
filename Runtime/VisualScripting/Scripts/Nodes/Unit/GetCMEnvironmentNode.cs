
using Reflectis.SDK.ApplicationManagement;
using Unity.VisualScripting;

namespace Reflectis.SDK.CreatorKit
{
    [UnitTitle("Reflectis CMEnvironment: Get CMEnvironment")]
    [UnitSurtitle("CMEnvironment")]
    [UnitShortTitle("Get CMEnvironment")]
    [UnitCategory("Reflectis\\Get")]
    public class GetCMEnvironmentNode : Unit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueOutput CMEnvironment { get; private set; }

        protected override void Definition()
        {
            CMEnvironment = ValueOutput(nameof(CMEnvironment), (f) => IReflectisApplicationManager.Instance.CurrentEvent.Environment);
        }
    }
}
