using Virtuademy.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

using Virtuademy.Environments.ScriptingApi;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Character: Exit Pan")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Exit Pan")]
    [UnitCategory("Reflectis\\Flow")]
    public class ExitPanNode : AwaitableUnit
    {
        protected override Task AwaitableAction(Flow flow)
        {
            TaskCompletionSource<bool> left = new();
            IVirtuademyGameplay.Current.Player.ExitCameraPan(() => left.TrySetResult(true));

            return left.Task;
        }
    }
}
