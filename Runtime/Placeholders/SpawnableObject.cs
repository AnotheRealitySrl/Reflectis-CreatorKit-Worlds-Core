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

        //------------ These 3 parameters will be hidden if spawnInHand is false.
        public bool symmetricHandPositioning = true;
        [HideInInspector] public Transform leftHandPivot;
        [HideInInspector] public Transform rightHandPivot;
        //------------------

        //If spawnInHand is false then show the parameter spawnPoint which is a Transform

        [HideInInspector] public GameObject LeftHandReference;
        [HideInInspector] public GameObject RightHandReference;


#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            if(rightHandPivot == null)
            {
                GameObject rightPivotGO = new GameObject("RightHandPivot");
                rightPivotGO.transform.parent = transform;
                rightPivotGO.transform.localPosition = Vector3.zero;
                rightHandPivot = rightPivotGO.transform;
            }

            if(leftHandPivot == null)
            {
                GameObject leftPivotGO = new GameObject("LeftHandPivot");
                leftPivotGO.transform.parent = transform;
                leftPivotGO.transform.localPosition = Vector3.zero;
                leftHandPivot = leftPivotGO.transform;
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
            Transform parentTransform;
            if (handType == 0){
                path = LeftHandReferencePrefabPath;
                hand = LeftHandReference;
                parentTransform = leftHandPivot;
            }
            else
            {
                path = RightHandReferencePrefabPath;
                hand = RightHandReference;
                parentTransform = rightHandPivot;
            }

            //Add the hand reference
            if (hand == null)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                hand = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parentTransform);
                hand.transform.localPosition = Vector3.zero;
                if (handType == 0)
                {
                    LeftHandReference = hand;
                }
                else
                {
                    RightHandReference = hand;
                }
                   
                EditorUtility.SetDirty(this);
            }
            else
            {
                DestroyImmediate(hand.gameObject);
                hand = null;
            }
        }

        public void SetPositionLeftHand()
        {
            if (leftHandPivot != null)
            {
                //leftSpawnPositionOffset = LeftHandReference.transform;
                if (symmetricHandPositioning)
                {
                    leftHandPivot.transform.localPosition = new Vector3(-rightHandPivot.transform.localPosition.x, rightHandPivot.transform.localPosition.y, rightHandPivot.transform.localPosition.z);
                    Vector3 rightRotation = rightHandPivot.transform.localEulerAngles;
                    leftHandPivot.transform.localEulerAngles = new Vector3(rightRotation.x, -rightRotation.y, -rightRotation.z);
                }
            }

        }

        /*public void SetPositionRightHand()
        {
            if (RightHandReference != null)
            {
                rightHandPivot = RightHandReference.transform;
            }
        }*/

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

            myScript.SetPositionLeftHand();
            //myScript.SetPositionRightHand();

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
