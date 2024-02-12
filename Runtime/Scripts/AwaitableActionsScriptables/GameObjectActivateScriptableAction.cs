using Reflectis.SDK.CharacterController;
using Reflectis.SDK.InteractionNew;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/GameObjectActivateScriptableAction", fileName = "GameObjectActivateScriptableAction")]
    public class GameObjectActivateScriptableAction : AwaitableScriptableAction
    {
        [SerializeField] private List<string> objectKeys = new();
        [SerializeField] private bool activate;

        public override Task Action(IInteractable interactable = null)
        {
            if (interactable != null)
            {
                foreach (var item in interactable.GameObjectRef.GetComponentsInChildren<GenericHookComponent>(true).Where(x => objectKeys.Contains(x.Id)))
                {
                    item.gameObject.SetActive(activate);
                };
            }

            return Task.CompletedTask;
        }
    }
}
