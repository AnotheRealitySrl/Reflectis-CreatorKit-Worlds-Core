using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SittablePlaceholder : SceneComponentPlaceholderNetwork
    {
        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        [Header("Sittable references")]
        [SerializeField] private Transform sitTransform;
        [SerializeField] private Transform stepUpTransform;
        [SerializeField] private bool isInteractable = true;



        public Transform SitTransform => sitTransform;
        public Transform StepUpTransform => stepUpTransform;
        public bool IsInteractable => isInteractable;
        public Role OwnershipMask => ownershipMask;
    }
}
