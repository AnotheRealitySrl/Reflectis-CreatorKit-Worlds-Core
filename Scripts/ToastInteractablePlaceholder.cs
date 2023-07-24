using UnityEngine;

using Virtuademy.Placeholders;

public class ToastInteractablePlaceholder : SceneComponentPlaceholderBase
{
    [SerializeField] private Transform panTargetTransform;
    [SerializeField] private Collider proximityActivatorCollider;
    [SerializeField] private Collider mouseActivatorCollider;

    public Transform PanTargetTransform => PanTargetTransform;
    public Collider ProximityActivatorCollider => proximityActivatorCollider;
    public Collider MouseActivatorCollider => mouseActivatorCollider;
}
