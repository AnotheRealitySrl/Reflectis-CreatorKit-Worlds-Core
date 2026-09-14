using Virtuademy.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

using UnityEngine;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Platform: Change Scene")]
    [UnitSurtitle("Platform")]
    [UnitShortTitle("Change Scene")]
    [UnitCategory("Reflectis\\Flow")]
    public class ChangeSceneNode : AwaitableUnit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput SceneAddressableName { get; private set; }

        protected override void Definition()
        {
            SceneAddressableName = ValueInput<string>(nameof(SceneAddressableName));

            base.Definition();
        }

        protected override async Task AwaitableAction(Flow flow)
        {

            var experience = await VirtuademyFramework.Current.FindExperienceByAddressableName(flow.GetValue<string>(SceneAddressableName));

            if (experience != null)
            {
                await VirtuademyFramework.Current.JoinExperience(experience, true);
            }
            else
            {
                Debug.LogError($"[Reflectis Creator Kit | Change Scene node] The key specified {flow.GetValue<string>(SceneAddressableName)} " +
                    $"for the environment is not correct or the experience is not flagged as " +
                    $"public");
            }
        }
    }
}
