using Reflectis.SDK.CreatorKit;

using UnityEngine;

public class BigScreenPlaceholder : SceneComponentPlaceholderBase
{
    [SerializeField] private Transform headerTransform;
    [SerializeField] private Transform controllerTransform;

    public Transform HeaderTransform => headerTransform;
    public Transform ControllerTransform => controllerTransform;
}
