using UnityEngine;

public class ToastInteractablePlaceholder : SpawnAddressablePrefabPlaceholder
{
    [SerializeField] private Transform panTargetTransform;
    [SerializeField] private Collider proximityActivatorCollider;
    [SerializeField] private Collider mouseActivatorCollider;

    public Transform PanTargetTransform => PanTargetTransform;
    public Collider ProximityActivatorCollider => proximityActivatorCollider;
    public Collider MouseActivatorCollider => mouseActivatorCollider;
}
