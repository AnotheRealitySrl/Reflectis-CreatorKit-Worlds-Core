using System.Collections.Generic;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core
{
    [CreateAssetMenu(menuName = "Reflectis/SDK-ObjectSpawner/SpawnableObjectListData", fileName = "SpawnableObjectListData")]
    public class SpawnableObjectListData : ScriptableObject
    {
        public List<GameObject> spawnableObjectList;

        public int AddToList(GameObject spawnObject)
        {
            spawnableObjectList.Add(spawnObject);
            return spawnableObjectList.IndexOf(spawnObject);
        }

        public List<GameObject> GetList()
        {
            return spawnableObjectList;
        }

        public int GetObjectIndex(GameObject value)
        {
            if (spawnableObjectList.Contains(value))
            {
                return spawnableObjectList.IndexOf(value);
            }
            else
            {
                return -1;
            }
        }

        public GameObject GetObjectInPosition(int index)
        {
            return spawnableObjectList[index];
        }
    }
}
