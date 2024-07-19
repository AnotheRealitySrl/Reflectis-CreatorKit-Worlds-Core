using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class HelpSpawnObjectPlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField] private GameObject prefab;

        public GameObject Prefab { get => prefab; }
    }
}
