using GLTFast.Schema;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Virtuademy.DTO;

namespace Virtuademy.Placeholders
{
    public class AssetSpawnerPlaceholder : SceneComponentPlaceholderBase
    {
        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        [Header("Asset Spawner references")]
        [SerializeField] private List<GameObject> prefabsToInstantiate;
        [SerializeField] private List<Transform>  posToSpawn;
        [SerializeField] private float spawnRate;
        [SerializeField] private bool haveToWait = false;
        [SerializeField] private bool prefabsAlreadySpawned = false;
        [SerializeField] private float timerToSpawn;
        [SerializeField] private List<GameObject> connectables;

        public Role OwnershipMask  => ownershipMask;
        public List<GameObject> PrefabsToInstantiate  => prefabsToInstantiate; 
        public List<Transform> PosToSpawn => posToSpawn; 
        public float SpawnRate => spawnRate; 
        public bool HaveToWait  => haveToWait;
        public float TimerToSpawn => timerToSpawn;
        public List<GameObject> Connectables => connectables;
        public bool PrefabsAlreadySpawned => prefabsAlreadySpawned; 
    }
}
