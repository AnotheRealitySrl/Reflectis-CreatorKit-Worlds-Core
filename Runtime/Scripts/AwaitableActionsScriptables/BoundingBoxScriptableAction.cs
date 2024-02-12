using Reflectis.SDK.CharacterController;

using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.InteractionNew
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/BoundingBoxScriptableAction", fileName = "BoundingBoxScriptableAction")]
    public class BoundingBoxScriptableAction : AwaitableScriptableAction
    {
        [SerializeField] private string boundingBoxHookId = "BoundingBox";
        [SerializeField] private bool activate;

        public override Task Action(IInteractable interactable)
        {
            GenericHookComponent hook = interactable.GameObjectRef.GetComponentsInChildren<GenericHookComponent>().FirstOrDefault(x => x.Id == boundingBoxHookId);
            if (hook)
            {
                hook.GetComponentInChildren<MeshRenderer>(true).enabled = activate;
            }

            return Task.CompletedTask;
        }

    }
}
