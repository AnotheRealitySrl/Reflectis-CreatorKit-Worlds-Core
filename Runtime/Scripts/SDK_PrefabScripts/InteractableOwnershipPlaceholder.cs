using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class InteractableOwnershipPlaceholder : SceneComponentPlaceholderNetwork
    {
        public Transform grabbableItem; //The item to be grabbed, containing photonview

        private void Awake()
        {
            if (grabbableItem == null)
            {
                grabbableItem = gameObject.transform;
            }
        }

    }
}