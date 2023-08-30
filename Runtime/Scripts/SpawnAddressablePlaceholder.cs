using System.Collections.Generic;

using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    public class SpawnAddressablePlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Addressable settings")]
        [SerializeField] private string addressableKey;
        [SerializeField] private Transform parent;
        [SerializeField] private List<GameObject> objsToDeactivate = new();
        [SerializeField] private bool isUI;

        public string AddressableKey => addressableKey;
        public Transform Parent => parent;
        public List<GameObject> ObjsToDeactivate => objsToDeactivate;
        public bool IsUI => isUI;
    }
}

