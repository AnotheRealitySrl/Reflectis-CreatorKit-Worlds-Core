using System.Collections.Generic;

using UnityEngine;
using static Reflectis.SDK.CreatorKit.UsablePlaceholder;

namespace Reflectis.SDK.CreatorKit
{
    public class AssetSpawnerPlaceholder : SceneComponentPlaceholderNetwork
    {
        public enum ESpawnerState
        {
            NotSpawned = 1,
            Spawned = 2
        }

        [Header("Asset Spawner references")]
        [SerializeField] private List<AssetReferences> assetReferences;
        [SerializeField] private bool haveToWait = false;
        [SerializeField] private float timerToSpawn = 0;
        [SerializeField] private bool canMultipleSpawn = false;
        [SerializeField] private ESpawnerState spawnerState = ESpawnerState.NotSpawned;
        [SerializeField] private bool isInteractable = true;


        public List<AssetReferences> AssetReferences => assetReferences;
        public bool HaveToWait => haveToWait;
        public bool CanMultipleSpawn => canMultipleSpawn;
        public float TimerToSpawn => timerToSpawn;
        public ESpawnerState SpawnerState => spawnerState;
        public bool IsInteractable => isInteractable;

    }
}
