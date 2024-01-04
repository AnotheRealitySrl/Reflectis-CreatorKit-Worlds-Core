using Reflectis.SDK.CreatorKit;

using UnityEngine;

public class BigScreenPlaceholder : SceneComponentPlaceholderBase
{
    [SerializeField] private Transform contentTransform;

    public Transform ContentTransform => contentTransform;
}
