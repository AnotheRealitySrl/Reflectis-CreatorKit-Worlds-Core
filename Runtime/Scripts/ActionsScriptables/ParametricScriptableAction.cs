using Reflectis.SDK.InteractionNew;

using System.Linq;
using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "AnotheReality/Utilities/ParametricScriptableAction", fileName = "ParametricScriptableAction")]
    public class ParametricScriptableAction : AwaitableScriptableAction
    {
        [SerializeField] private bool isChildObject;
        [SerializeField] private string goName;
        [SerializeField] private string methodName;

        public override Task Action(IInteractable interactable = null)
        {
            if (isChildObject)
            {
                Transform[] allChildren = interactable.GameObjectRef.GetComponentsInChildren<Transform>(true);
                allChildren.FirstOrDefault(x => x.name == goName)?.gameObject.SendMessage(methodName);
            }
            else
            {
                GameObject.Find(goName).SendMessage(methodName);
            }

            return Task.CompletedTask;
        }
    }
}

