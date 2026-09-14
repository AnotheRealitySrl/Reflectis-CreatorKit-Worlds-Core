using Virtuademy.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

using UnityEngine;

using Virtuademy.ScriptingApi;

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

        protected override Task AwaitableAction(Flow flow)
        {
            string key = flow.GetValue<string>(SceneAddressableName);
            TaskCompletionSource<bool> done = new();

            IVirtuademyFramework.Current.Session.FindExperience(key, experience =>
            {
                if (experience == null)
                {
                    Debug.LogError($"[Reflectis Creator Kit | Change Scene node] The key specified {key} " +
                        $"for the environment is not correct or the experience is not flagged as " +
                        $"public");
                    done.TrySetResult(false);

                    return;
                }

                IVirtuademyFramework.Current.Session.JoinExperience(experience, true,
                                                                   joined => done.TrySetResult(joined));
            });

            return done.Task;
        }
    }
}
