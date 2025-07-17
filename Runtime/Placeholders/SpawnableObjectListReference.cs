using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core
{
    public class SpawnableObjectListReference : MonoBehaviour
    {
        public SpawnableObjectListData spawnObjectList;

        public void SetReference(SpawnableObjectListData data)
        {
            spawnObjectList = data;
        }
    }
}
