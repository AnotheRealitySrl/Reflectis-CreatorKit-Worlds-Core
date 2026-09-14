using Virtuademy.SDK.Core.VisualScripting;
using System.Threading.Tasks;

using Unity.VisualScripting;

using UnityEngine;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Platform: Check Scene Availability")]
    [UnitSurtitle("Platform")]
    [UnitShortTitle("Check Scene Availability")]
    [UnitCategory("Reflectis\\Flow")]
    public class CheckSceneAvailabilityNode : AwaitableUnit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput SceneAddressableName { get; private set; }
        public ValueOutput IsAvailable { get; private set; }
        private bool _isAvailable;

        protected override void Definition()
        {
            SceneAddressableName = ValueInput<string>(nameof(SceneAddressableName));
            IsAvailable = ValueOutput<bool>(nameof(IsAvailable), f => _isAvailable);

            base.Definition();
        }

        protected override async Task AwaitableAction(Flow flow)
        {

            var experience = await VirtuademyFramework.Current.FindExperienceByAddressableName(flow.GetValue<string>(SceneAddressableName));

            if (experience != null)
            {
                _isAvailable = true;
            }
            else
            {
                _isAvailable = false;
            }
        }
    }
}
