using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class InteractableOwnershipPlaceholder : SceneComponentPlaceholderNetwork
    {
        public Transform grabbableItem; //The item to be grabbed, containing photonview
        public Role ownershipMask; //The possible users that can grab the item

        private void Awake()
        {
            if (grabbableItem == null)
            {
                grabbableItem = gameObject.transform;
            }
        }

    }
}