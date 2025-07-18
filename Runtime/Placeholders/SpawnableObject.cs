using Reflectis.CreatorKit.Worlds.Core.Placeholders;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core
{
    public class SpawnableObject : SceneComponentPlaceholderBase
    {

        private const string SpawnableObjectListDataPath = "Assets/SpawnableObject/SpawnableObjectList.asset";

        private bool _isSceneObject;
        public bool IsSceneObject => _isSceneObject;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            //Check whether or not I am a prefab and I am in prefab mode (?)
            if ((EditorUtility.IsPersistent(this) || PrefabUtility.GetPrefabAssetType(gameObject) != PrefabAssetType.NotAPrefab) && PrefabUtility.IsPartOfPrefabAsset(gameObject))
            {
                //Add object to scriptable object containing the list of all the items
                SpawnableObjectListData spawnList = LoadOrCreateData();
                if (spawnList == null)
                {
                    Debug.LogError("Spawn List data not found.");
                    return;
                }

                if (spawnList.GetList().Count ==0 || !spawnList.GetList().Any(obj => obj != null && AssetDatabase.GetAssetPath(obj) == AssetDatabase.GetAssetPath(this)))
                {
                    spawnList.AddToList(this.gameObject);// spawnableObjectList.Add(this.gameObject);

                    // Clean duplicates and nulls
                    spawnList.spawnableObjectList = spawnList.spawnableObjectList
                        .Where(obj => obj != null)
                        .Distinct()
                        .ToList();

                    EditorUtility.SetDirty(spawnList);
                    EditorApplication.delayCall += () =>
                    {
                        EditorUtility.SetDirty(spawnList);
                        AssetDatabase.SaveAssets();
                    };
                }
            }
        }

        /*private void OnDestroy()
        {
            // Only handle editor-time logic (not during play mode)
            if (Application.isPlaying)
                return;

            Debug.LogError("DESTROY");
            // We're destroying the component (or the whole GameObject)
            SpawnableObjectListData spawnList = AssetDatabase.LoadAssetAtPath<SpawnableObjectListData>(SpawnableObjectListDataPath);
            if (spawnList == null) return;

            if (spawnList.spawnableObjectList.Contains(gameObject))
            {
                spawnList.spawnableObjectList.Remove(gameObject);
                EditorUtility.SetDirty(spawnList);
                AssetDatabase.SaveAssets();
                Debug.Log($"Removed {name} from spawnable list (component removed or destroyed)");
            }
        }*/

        private static SpawnableObjectListData LoadOrCreateData()
        {
            SpawnableObjectListData spawnableList = AssetDatabase.LoadAssetAtPath<SpawnableObjectListData>(SpawnableObjectListDataPath);
            if (spawnableList == null)
            {
                // Ensure folder exists
                string dir = Path.GetDirectoryName(SpawnableObjectListDataPath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                // Create instance and save
                spawnableList = ScriptableObject.CreateInstance<SpawnableObjectListData>();
                AssetDatabase.CreateAsset(spawnableList, SpawnableObjectListDataPath);
            }

            return spawnableList;
        }
#endif


    }
}
