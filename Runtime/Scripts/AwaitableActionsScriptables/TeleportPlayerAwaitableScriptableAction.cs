using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;
using Reflectis.SDK.Fade;
using Reflectis.SDK.InteractionNew;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/TeleportPlayerScriptableAction", fileName = "TeleportPlayerScriptableAction")]
    public class TeleportPlayerAwaitableScriptableAction : AwaitableScriptableAction
    {
        [SerializeField]
        private string hookID = "TeleportTarget";

        private bool inTransition;
        public override async Task Action(IInteractable interactable)
        {
            Transform target = interactable.GameObjectRef.transform;

            var hook = interactable.GameObjectRef.GetComponentsInChildren<GenericHookComponent>(true).FirstOrDefault((x) => x.Id == hookID);

            if (hook != null)
            {
                target = hook.transform;
            }
            inTransition = true;
            SM.GetSystem<IFadeSystem>().FadeToBlack(() =>
            {
                SM.GetSystem<ICharacterControllerSystem>().MoveCharacter(new Pose(target.position, target.rotation));
                SM.GetSystem<IFadeSystem>().FadeFromBlack(() => inTransition = false);
            });
            while (inTransition)
            {
                await Task.Yield();
            }
        }
    }
}
