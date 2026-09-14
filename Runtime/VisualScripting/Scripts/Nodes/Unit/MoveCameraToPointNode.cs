using Virtuademy.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

using UnityEngine;

using Virtuademy.Environments.ScriptingApi;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Character: Move camera to point")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Move camera")]
    [UnitCategory("Reflectis\\Flow")]
    public class MoveCameraToPointNode : AwaitableUnit
    {
        [NullMeansSelf]
        [DoNotSerialize]
        [PortLabelHidden]
        public ValueInput TargetTransform { get; private set; }

        protected override void Definition()
        {
            TargetTransform = ValueInput<Transform>(nameof(TargetTransform));

            base.Definition();
        }

        protected override Task AwaitableAction(Flow flow)
        {
            TaskCompletionSource<bool> arrived = new();
            IVirtuademyGameplay.Current.Player.MoveCameraTo(flow.GetValue<Transform>(TargetTransform),
                                                            () => arrived.TrySetResult(true));

            return arrived.Task;
        }
    }
}
