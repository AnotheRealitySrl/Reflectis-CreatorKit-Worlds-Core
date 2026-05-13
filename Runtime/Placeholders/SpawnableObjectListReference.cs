using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core
{
    public class SpawnableObjectListReference : MonoBehaviour
    {
        public static SpawnableObjectListReference Instance { get; private set; }

        public SpawnableObjectListData spawnObjectList;
        public List<SpawnableObjectPlaceholder> sceneSpawnableObjectList;

        private void Awake()
        {
            // 1. Check if an instance already exists
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // 2. Set the Instance
            Instance = this;

        }

        public int SubstituteSceneGO(int index, GameObject copy)
        {
            Debug.LogError("Index is " + index);
            sceneSpawnableObjectList[index] = copy.GetComponent<SpawnableObjectPlaceholder>();
            return index;
        }

        public int GetIndex(SpawnableObjectPlaceholder go)
        {
            return sceneSpawnableObjectList.IndexOf(go);
        }

        public void SetReference(SpawnableObjectListData data)
        {
            spawnObjectList = data;
        }

    }
}
