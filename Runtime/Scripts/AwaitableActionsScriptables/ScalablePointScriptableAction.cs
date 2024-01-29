using Reflectis.SDK.CharacterController;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.InteractionNew
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/ScalablePointScriptableAction", fileName = "ScalablePointScriptableAction")]
    public class ScalablePointScriptableAction : AwaitableScriptableAction
    {
        [SerializeField] private string scalableCornerHookId = "ScalableCorner";
        [SerializeField] private string scalableFaceHookId = "ScalableFace";
        [SerializeField] private bool activate;

        public override Task Action(IInteractable interactable)
        {
            List<GenericHookComponent> hooks = interactable.GameObjectRef.GetComponentsInChildren<GenericHookComponent>()
                .Where(x => x.Id == scalableCornerHookId || x.Id == scalableFaceHookId).ToList();
            if (hooks.Count > 0)
            {
                hooks.ForEach(x => x.GetComponent<MeshRenderer>().enabled = activate);
            }

            return Task.CompletedTask;
        }
    }
}