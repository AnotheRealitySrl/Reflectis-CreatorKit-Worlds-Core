using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [RequireComponent(typeof(Collider))]
    public class ActivatablePlaceholder : SceneComponentPlaceholderBase
    {
        private string activateActionName = "TriggerActivateEvent";

        public string ActivateActionName => activateActionName;
    }
}
