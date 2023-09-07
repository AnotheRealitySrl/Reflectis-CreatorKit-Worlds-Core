using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.SDK.CreatorKit
{
    [System.Serializable]
    public class AssetReferences 
    {
        public GameObject prefabToInstantiate;
        public Transform posToSpawn;
        public float spawnChance;
    }
}
