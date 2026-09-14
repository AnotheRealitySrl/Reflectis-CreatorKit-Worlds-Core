
using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.VisualScripting
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
            CMEvent = ValueOutput(nameof(CMEvent), (f) => VirtuademyFramework.Current.CurrentSession);
        }
    }
}
