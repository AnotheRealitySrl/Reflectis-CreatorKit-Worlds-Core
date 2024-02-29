using Reflectis.SDK.InteractionNew;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "Reflectis/Scriptable Actions/EnableChildrenBehaviourScriptableAction", fileName = "EnableChildrenBehaviourScriptableAction")]

    public class EnableBehaviourScriptableAction : AwaitableScriptableAction
    {
        [SerializeField]
        private bool enable;
        [SerializeField]
        private string referenceContainerId;

        public override Task Action(IInteractable interactable)
        {
            ReferenceContainer referenceContainer = interactable.GameObjectRef.GetComponentsInChildren<ReferenceContainer>(true).FirstOrDefault(x => x.Id == referenceContainerId);
            if (referenceContainer != null)
            {
                foreach (Reference reference in referenceContainer.CustomReferences)
                {
                    if (reference.component is Behaviour)
                    {
                        (reference.component as Behaviour).enabled = enable;
                    }

                    if (reference.component is Collider)
                    {
                        (reference.component as Collider).enabled = enable;
                    }
                }
            }
            else
            {
                Debug.LogWarning($"Cannot find reference container with id {referenceContainerId}!", interactable.GameObjectRef);
            }

            return Task.CompletedTask;
        }
    }
}
