using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class SittablePlaceholder : SceneComponentPlaceholderNetwork
    {
        [Header("Sittable references")]
        [SerializeField] private Transform sitTransform;
        [SerializeField] private Transform stepUpTransform;
        [SerializeField] private bool isInteractable = true;

        public Transform SitTransform => sitTransform;
        public Transform StepUpTransform => stepUpTransform;
        public bool IsInteractable => isInteractable;
    }
}
