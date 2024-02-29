using Reflectis.SDK.CharacterController;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.InteractionNew
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/EnableChildrenObjectsScriptableAction", fileName = "EnableChildrenObjectsScriptableAction")]
    public class EnableChildrenObjectsScriptableAction : AwaitableScriptableAction
    {
        [SerializeField] private string enableObjectsId;
        [SerializeField] private bool enable;

        public override Task Action(IInteractable interactable)
        {
            if (interactable == null || interactable.GameObjectRef == null)
            {
                return Task.CompletedTask;
            }
            foreach (GenericHookComponent component in interactable.GameObjectRef.GetComponentsInChildren<GenericHookComponent>(true))
            {
                if (enableObjectsId == component.Id)
                {
                    component.gameObject.SetActive(enable);
                }

            }

            return Task.CompletedTask;
        }
    }
}