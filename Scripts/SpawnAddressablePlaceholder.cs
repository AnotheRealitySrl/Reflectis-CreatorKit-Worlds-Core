using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class SpawnAddressablePlaceholder : SceneComponentPlaceholderBase
    {
        [SerializeField]
        private string prefabAddressableName;
        [SerializeField]
        private Transform parent;

        public string PrefabAddressableName => prefabAddressableName;
        public Transform Parent => parent;
    }

}
