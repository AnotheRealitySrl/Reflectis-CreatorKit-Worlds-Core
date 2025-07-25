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
        private const string LeftHandReferencePrefabPath = "Packages/com.anotherealitysrl.reflectis-creatorkit-worlds-core/Prefab/LeftHandReference.prefab";
        private const string RightHandReferencePrefabPath = "Packages/com.anotherealitysrl.reflectis-creatorkit-worlds-core/Prefab/RightHandReference.prefab";

        private bool _isSceneObject;
        public bool IsSceneObject => _isSceneObject;

        private bool spawnInHand = true; //in the future add spawnOnHand bool and if false spawnPoint transform.
        //------------ These 2 parameters will be hidden if spawnInHand is false.
        public Vector3 leftSpawnPositionOffset = Vector3.zero; 
        public Vector3 rightSpawnPositionOffset = Vector3.zero;
        //------------------

        //If spawnInHand is false then show the parameter spawnPoint which is a Transform

        /*[HideInInspector]*/ public GameObject LeftHandReference;
        /*[HideInInspector]*/ public GameObject RightHandReference;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            //Check whether or not I am a prefab and I am in prefab mode 
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
                    spawnList.AddToList(this.gameObject);

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

        public void ShowHandReference(GameObject HandReference, int handType)
        {
            GameObject hand = null;
            string path = "";
            if (handType == 0){
                path = LeftHandReferencePrefabPath;
                hand = LeftHandReference;
            }
            else
            {
                path = RightHandReferencePrefabPath;
                hand = RightHandReference;
            }

            //Add the hand reference
            if (hand == null)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                hand = (GameObject)PrefabUtility.InstantiatePrefab(prefab, this.transform);
                hand.transform.localPosition = Vector3.zero;
                if (handType == 0)
                    LeftHandReference = hand;
                else
                    RightHandReference = hand;
                EditorUtility.SetDirty(this);
            }
            else
            {
                DestroyImmediate(hand.gameObject);
                hand = null;
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
#endif
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(SpawnableObject))]
    public class SpawnableObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            SpawnableObject myScript = (SpawnableObject)target;
            string leftButtonLabel = myScript.LeftHandReference == null ? "Show Left Reference" : "Hide Left Reference";
            string rigthButtonLabel = myScript.RightHandReference == null ? "Show Right Reference" : "Hide Right Reference";

            if (myScript.LeftHandReference != null)
            {
                //place the hand based on the position set by the user
                myScript.LeftHandReference.transform.position = myScript.leftSpawnPositionOffset;
            }
            if (myScript.RightHandReference != null)
            {
                //place the hand based on the position set by the user
                myScript.RightHandReference.transform.position = myScript.rightSpawnPositionOffset;
            }

            if (GUILayout.Button(leftButtonLabel))
            {
                myScript.ShowHandReference(myScript.LeftHandReference, 0);
            }
            if (GUILayout.Button(rigthButtonLabel))
            {
                myScript.ShowHandReference(myScript.RightHandReference, 1);
            }
        }
    }
#endif
}
