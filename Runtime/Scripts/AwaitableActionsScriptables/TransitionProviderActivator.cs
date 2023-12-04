using Reflectis.SDK.InteractionNew;
using Reflectis.SDK.Transitions;

using System.Threading.Tasks;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [CreateAssetMenu(menuName = "Reflectis/SDK-CreatorKit/TransitionProviderActivator", fileName = "TransitionProviderActivator")]
    public class TransitionProviderActivator : AwaitableScriptableAction
    {
        public string transitionProviderGO;
        public bool enable;

        public override Task Action(IInteractable interactable = null)
        {
            interactable.GameObjectRef.transform.Find(transitionProviderGO).GetComponent<AbstractTransitionProvider>().DoTransition(enable);
            return Task.CompletedTask;
        }
    }
}
