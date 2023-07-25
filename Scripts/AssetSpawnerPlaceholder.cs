using System.Collections.Generic;

using UnityEngine;

using static BaseAssetSpawnerController;

namespace Virtuademy.Placeholders
{
    public class AssetSpawnerPlaceholder : SceneComponentPlaceholderNetwork
    {
        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        [Header("Asset Spawner references")]
        [SerializeField] private List<GameObject> prefabsToInstantiate;
        [SerializeField] private List<Transform> posToSpawn;
        [SerializeField] private float spawnRate;
        [SerializeField] private bool haveToWait = false;
        [SerializeField] private float timerToSpawn;
        [SerializeField] private SpawnerState spawnerState = SpawnerState.NotSpawned;
        [SerializeField] private List<GameObject> connectables;
        [SerializeField] private GameObject fatherConnecter;

        public Role OwnershipMask => ownershipMask;
        public List<GameObject> PrefabsToInstantiate => prefabsToInstantiate;
        public List<Transform> PosToSpawn => posToSpawn;
        public float SpawnRate => spawnRate;
        public bool HaveToWait => haveToWait;
        public float TimerToSpawn => timerToSpawn;
        public List<GameObject> Connectables => connectables;
        public SpawnerState SpawnerState => spawnerState;
        public GameObject FatherConnecter => fatherConnecter;
    }
}
