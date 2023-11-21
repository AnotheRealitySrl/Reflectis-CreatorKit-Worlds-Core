using Reflectis.SDK.CharacterController;
using Reflectis.SDK.Core;

using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.InteractionNew
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/CameraPanScriptableAction", fileName = "CameraPanScriptableAction")]
    public class CameraPanScriptableAction : AwaitableScriptableAction
    {
        [SerializeField] private string panTransformId;
        [SerializeField] private bool interact;

        public override async Task Action(IInteractable interactable = null)
        {
            if (interact)
            {
                Transform panTransform = interactable.GameObjectRef.GetComponentsInChildren<GenericHookComponent>().FirstOrDefault(x => x.Id == panTransformId).transform;
                await SM.GetSystem<ICharacterControllerSystem>().GoToInteractState(panTransform);
            }
            else
            {
                await SM.GetSystem<ICharacterControllerSystem>().GoToSetMovementState();
            }
        }
    }
}