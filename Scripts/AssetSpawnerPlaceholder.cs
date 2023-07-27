using System.Collections.Generic;

using UnityEngine;

namespace Virtuademy.Placeholders
{
    public class AssetSpawnerPlaceholder : SceneComponentPlaceholderBase
    {
        public enum ESpawnerState
        {
            NotSpawned = 1,
            Spawned = 2
        }

        [Header("Network settings")]
        [SerializeField] private Role ownershipMask;

        [Header("Asset Spawner references")]
        [SerializeField] private List<GameObject> prefabsToInstantiate;
        [SerializeField] private List<Transform> posToSpawn;
        [SerializeField] private float spawnChance;
        [SerializeField] private bool haveToWait = false;
        //[SerializeField] private bool canMultipleSpawn = false;
        [SerializeField] private float timerToSpawn;
        [SerializeField] private ESpawnerState spawnerState = ESpawnerState.NotSpawned;
        [SerializeField] private List<GameObject> connectables;
        [SerializeField] private GameObject fatherConnecter;

        public Role OwnershipMask => ownershipMask;
        public List<GameObject> PrefabsToInstantiate => prefabsToInstantiate;
        public List<Transform> PosToSpawn => posToSpawn;
        public float SpawnChance => spawnChance;
        public bool HaveToWait => haveToWait;
        //public bool CanMultipleSpawn=> canMultipleSpawn;
        public float TimerToSpawn => timerToSpawn;
        public List<GameObject> Connectables => connectables;
        public ESpawnerState SpawnerState => spawnerState;
        public GameObject FatherConnecter => fatherConnecter;
    }
}
