using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class DrawableBoardPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private int instantiationId;
        [SerializeField] private Collider drawableArea;
        [SerializeField] private Transform drawingProjectionTransform;
        [SerializeField] private Role ownershipMask;

        public int InitializationId => instantiationId;
        public Collider DrawableArea => drawableArea;
        public Transform DrawingProjectionTransform => drawingProjectionTransform;
        public Role OwnershipMask => ownershipMask;
    }
}

