using Virtuademy.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

using Virtuademy.Environments.ScriptingApi;

namespace Virtuademy.SDK.Environments.VisualScripting
{
    [UnitTitle("Reflectis Platform: Load Lobby")]
    [UnitSurtitle("Platform")]
    [UnitShortTitle("Load Lobby")]
    [UnitCategory("Reflectis\\Flow")]
    public class LoadDefaultEventNode : AwaitableUnit
    {
        protected override Task AwaitableAction(Flow flow)
        {
            // Nothing to wait for: by the time the lobby is up this world is gone, and so is the
            // graph that would have received the output trigger.
            IVirtuademyGameplay.Current.Scene.ReturnToLobby();

            return Task.CompletedTask;
        }
    }
}
