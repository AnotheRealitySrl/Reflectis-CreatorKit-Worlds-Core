using Reflectis.SDK.CreatorKit;

using UnityEngine;

public class BigScreenPlaceholder : SceneComponentPlaceholderNetwork
{
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Transform screenTransform;

    public Transform ScreenTransform => screenTransform;
    public Transform ContentTransform => contentTransform;
}