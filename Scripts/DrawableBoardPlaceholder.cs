using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class DrawableBoardPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private Collider drawableArea;
        [SerializeField] private Transform drawingProjectionTransform;
        [SerializeField] private Role ownershipMask;

        public Collider DrawableArea => drawableArea;
        public Transform DrawingProjectionTransform => drawingProjectionTransform;
        public Role OwnershipMask => ownershipMask;
    }
}

