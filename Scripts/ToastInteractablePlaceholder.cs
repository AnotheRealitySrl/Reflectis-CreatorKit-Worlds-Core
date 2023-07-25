using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class ToastInteractablePlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private Transform panTargetTransform;
        [SerializeField] private Collider proximityActivatorCollider;
        [SerializeField] private Collider mouseActivatorCollider;

        public Transform PanTargetTransform => panTargetTransform;
        public Collider ProximityActivatorCollider => proximityActivatorCollider;
        public Collider MouseActivatorCollider => mouseActivatorCollider;
    }

}
