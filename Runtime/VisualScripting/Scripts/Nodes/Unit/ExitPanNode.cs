using Reflectis.SDK.Core;
using Reflectis.SDK.Core.CharacterController;
using Reflectis.SDK.Core.VisualScripting;

using System.Threading.Tasks;

using Unity.VisualScripting;

namespace Reflectis.CreatorKit.Core
{
    [UnitTitle("Reflectis Character: Exit Pan")]
    [UnitSurtitle("Character")]
    [UnitShortTitle("Exit Pan")]
    [UnitCategory("Reflectis\\Flow")]
    public class ExitPanNode : AwaitableUnit
    {
        protected async override Task AwaitableAction(Flow flow)
        {
            await SM.GetSystem<ICharacterControllerSystem>().GoToSetMovementState();
        }
    }
}
