using Reflectis.SDK.ApplicationManagement;
using Reflectis.SDK.Avatars;
using Reflectis.ClientModels;
using Reflectis.SDK.Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
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
            var clientModelSystem = SM.GetSystem<IClientModelSystem>();

            var staticEvents = await clientModelSystem.GetStaticEvents();

            if (staticEvents.Count == 0 || staticEvents == null)
            {
                Debug.LogError($"[Reflectis Creator Kit | Change Scene node] No static event available.");
                return;
            }

            CMEvent staticEvent = staticEvents.Find(
                x => x.Environment.AddressableKey == flow.GetValue<string>(SceneAddressableName));

            if (staticEvent != null && staticEvent.CanJoin)
            {
                await IReflectisApplicationManager.Instance.LoadEvent(staticEvent);
            }
            else
            {
                Debug.LogError($"[Reflectis Creator Kit | Change Scene node] The key specified " +
                    $"for the environment is not correct or the event is not flagged as " +
                    $"static, or the event cannot be joined");
            }
        }
    }
}
