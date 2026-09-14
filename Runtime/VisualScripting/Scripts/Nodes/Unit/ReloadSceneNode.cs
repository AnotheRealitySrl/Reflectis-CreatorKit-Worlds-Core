using Virtuademy.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Platform: Reload Scene")]
    [UnitSurtitle("Platform")]
    [UnitShortTitle("Reload Scene")]
    [UnitCategory("Reflectis\\Flow")]
    public class ReloadSceneNode : AwaitableUnit
    {
        //[NullMeansSelf]
        //[DoNotSerialize]
        //[PortLabelHidden]
        //public ValueInput IsTenantEnvironment { get; private set; }

        /*[NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput Multiplayer { get; private set; }*/

        protected override void Definition()
        {
            //IsTenantEnvironment = ValueInput<bool>(nameof(IsTenantEnvironment), false);

            base.Definition();
        }

        protected override async Task AwaitableAction(Flow flow)
        {

            var experience = await VirtuademyFramework.Current.FindExperienceByAddressableName(VirtuademyFramework.Current.CurrentEnvironmentName/*, flow.GetValue<bool>(IsTenantEnvironment)*/);
            var multiplayer = VirtuademyFramework.Current.IsCurrentEnvironmentMultiplayer;
            if (experience != null)
            {
                await VirtuademyFramework.Current.JoinExperience(experience, multiplayer);
            }
            /*else
            {
                Debug.LogError($"[Reflectis Creator Kit | Change Scene node] The key specified {SceneAddressableName.Name} " +
                    $"for the environment is not correct or the experience is not flagged as " +
                    $"public");
            }*/
        }
    }
}
