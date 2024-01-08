using Reflectis.SDK.CreatorKit;

using UnityEngine;

public class BigScreenPlaceholder : SceneComponentPlaceholderNetwork
{
    [SerializeField] private Transform contentTransform;

    public Transform ContentTransform => contentTransform;
}
