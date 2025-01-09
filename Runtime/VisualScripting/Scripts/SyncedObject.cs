using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Reflectis.CreatorKit.Core
{
    public class SyncedObject : SceneComponentPlaceholderNetwork
    {

        public enum OwnershipRequestEnum
        {
            Request,
            Takeover,
            Fixed
        }

        [SerializeField] private OwnershipRequestEnum ownershipRequestType;

        [HideInInspector] public bool syncTransform = true;

        [HideInInspector] public string assetID; // unity prefab asset ID
        [HideInInspector] public string instanceID;

        public Action onRequestOwnershipAction;
        public Action onReleaseOwnershipAction;

        public delegate bool OnCheckOwnershipObject();
        public event OnCheckOwnershipObject onCheckOwnershipObject;

        public OwnershipRequestEnum OwnershipRequestType { get => ownershipRequestType; }

        public bool OnCheckOwnershipFunction()
        {
            if (onCheckOwnershipObject == null)
            {
                Debug.LogError("Null ownership");
            }

            return (bool)onCheckOwnershipObject?.Invoke();
        }

#if UNITY_EDITOR
        [ContextMenu("Remove Synced Variables")]
        private void RemoveSyncedVariables()
        {
            if (TryGetComponent(out SyncedVariables variables))
            {
                DestroyImmediate(variables);
            }
        }

        protected void OnValidate()
        {
            if (Application.isPlaying)
                return;

            PrefabInstanceStatus prefabInstanceStatus = PrefabUtility.GetPrefabInstanceStatus(this);
            PrefabAssetType prefabAssetType = PrefabUtility.GetPrefabAssetType(this);

            bool isPrefab = prefabAssetType != PrefabAssetType.NotAPrefab;
            bool isPrefabInstance = isPrefab && prefabInstanceStatus == PrefabInstanceStatus.Connected;
            bool isPrefabAsset = isPrefab && prefabInstanceStatus == PrefabInstanceStatus.NotAPrefab;

            // set assetID if it's a prefab or instance of prefab
            if (isPrefabAsset || isPrefabInstance)
            {
                string assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
                string assetGUID = AssetDatabase.AssetPathToGUID(assetPath);
                if (assetID != assetGUID)
                {
                    assetID = assetGUID;
                }
            }
            else
            {
                assetID = null;
            }

            // set instance ID if it's an instance in the scene
            // or embedded within a prefab object
            if (isPrefabAsset)
            {
                instanceID = null;
            }
            else
            {
                HashSet<string> allInstanceIDs = new HashSet<string>();
                SyncedObject[] allInstanceSyncedObjects = GameObject.FindObjectsOfType<SyncedObject>();
                foreach (SyncedObject syncedObject in allInstanceSyncedObjects)
                {
                    if (syncedObject == this)
                        continue;

                    allInstanceIDs.Add(syncedObject.instanceID);
                }

                while (string.IsNullOrEmpty(instanceID) || allInstanceIDs.Contains(instanceID))
                {
                    instanceID = System.Guid.NewGuid().ToString();
                }
            }
        }
#endif
    }
}
