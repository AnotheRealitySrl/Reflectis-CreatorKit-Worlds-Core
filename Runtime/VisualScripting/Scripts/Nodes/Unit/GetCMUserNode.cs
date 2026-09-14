
using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.VisualScripting
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
            CMUser = ValueOutput(nameof(CMUser), (f) => VirtuademyFramework.Current.LocalUser);
        }
    }
}
