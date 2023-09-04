using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class DrawableBoardPlaceholder : SceneComponentPlaceholderNetwork
    {
        [SerializeField] private Collider drawableArea;
        [SerializeField] private Transform drawingProjectionTransform;

        public Collider DrawableArea => drawableArea;
        public Transform DrawingProjectionTransform => drawingProjectionTransform;
    }
}

