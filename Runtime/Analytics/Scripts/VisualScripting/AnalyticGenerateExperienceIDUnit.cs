using Virtuademy.SDK.Environments.Analytics;
using Virtuademy.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

using UnityEngine;

using Virtuademy.ScriptingApi;

namespace Virtuademy.SDK.Environments.Analytics
{
    [UnitTitle(UNIT_TITLE)]
    [UnitSurtitle("Reflectis Analytic")]
    [UnitShortTitle("Generate ExperienceID")]
    [UnitCategory("Reflectis\\Flow")]
    public class AnalyticGenerateExperienceIDUnit : AwaitableUnit
    {
        public const string UNIT_TITLE = "Reflectis Analytic: Generate ExperienceID";

        private const int MAX_KEY_LENGTH = 15;

        [DoNotSerialize]
        public ValueInput Key { get; private set; }

        private GameObject gameObject;

        public override void Instantiate(GraphReference instance)
        {
            base.Instantiate(instance);

            gameObject = instance.gameObject;
        }


        protected override async Task AwaitableAction(Flow flow)
        {
            string desiredKey = flow.GetConvertedValue(Key) as string;

            if (string.IsNullOrEmpty(desiredKey) || desiredKey.Length > MAX_KEY_LENGTH)
            {
                Debug.LogError($"Error during execution of \"{UNIT_TITLE}\" on gameObject {gameObject}" +
                    $"The key \"{desiredKey}\" is not valid! " +
                    $"The key string must not be null or empty and it has to contain less than " + MAX_KEY_LENGTH + " characters! ", gameObject);
            }
            else
            {
                TaskCompletionSource<bool> minted = new();
                IVirtuademyFramework.Current.Analytics.GenerateExperienceGuid(
                    desiredKey, () => minted.TrySetResult(true));

                await minted.Task;
            }
        }

        protected override void Definition()
        {
            base.Definition();

            Key = ValueInput(typeof(string), nameof(Key));
            Requirement(Key, InputTrigger);

        }


    }
}
