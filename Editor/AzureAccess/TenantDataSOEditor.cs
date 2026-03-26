using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.CoreEditor
{
    [CustomEditor(typeof(TenantDataSO))]
    public class TenantDataSOEditor : Editor
    {
        [Serializable]
        private class TenantDataJson
        {
            public string clientId;
            public string tenant;
            public string policy;
            public string profileApi;
            public string profileApiId;
            public string apiLabel;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("JSON", EditorStyles.boldLabel);

            if (GUILayout.Button("Import from JSON"))
            {
                string path = EditorUtility.OpenFilePanel("Seleziona Tenant Data JSON", "", "json");
                if (!string.IsNullOrEmpty(path))
                    ImportFromJson(path);
            }

            if (GUILayout.Button("Export to JSON"))
            {
                string path = EditorUtility.SaveFilePanel("Salva Tenant Data JSON", "", target.name, "json");
                if (!string.IsNullOrEmpty(path))
                    ExportToJson(path);
            }
        }

        private void ImportFromJson(string path)
        {
            try
            {
                string json = File.ReadAllText(path);
                TenantDataJson data = JsonConvert.DeserializeObject<TenantDataJson>(json);

                if (data == null)
                {
                    Debug.LogError("Import fallito: JSON non valido o vuoto.");
                    return;
                }

                SerializedObject so = new SerializedObject(target);

                SetString(so, "clientId", data.clientId);
                SetString(so, "tenant", data.tenant);
                SetString(so, "profileApi", data.profileApi);
                SetString(so, "profileApiId", data.profileApiId);
                SetString(so, "apiLabel", data.apiLabel);

                if (!string.IsNullOrEmpty(data.policy))
                {
                    SerializedProperty policyProp = so.FindProperty("policy");
                    int enumIndex = Array.IndexOf(policyProp.enumNames, data.policy);
                    if (enumIndex >= 0)
                        policyProp.enumValueIndex = enumIndex;
                    else
                        Debug.LogWarning($"Policy '{data.policy}' non riconosciuta. Valori validi: {string.Join(", ", policyProp.enumNames)}");
                }

                so.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
                AssetDatabase.SaveAssets();

                Debug.Log($"TenantDataSO importato da: {path}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Errore durante l'import da JSON: {ex.Message}");
            }
        }

        private void ExportToJson(string path)
        {
            try
            {
                TenantDataSO tenantData = (TenantDataSO)target;
                TenantDataJson data = new TenantDataJson
                {
                    clientId = tenantData.ClientId,
                    tenant = tenantData.Tenant,
                    policy = tenantData.Policy.ToString(),
                    profileApi = tenantData.ProfileApi,
                    profileApiId = tenantData.ProfileApiId,
                    apiLabel = tenantData.ApiLabel
                };

                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                File.WriteAllText(path, json);

                Debug.Log($"TenantDataSO esportato in: {path}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Errore durante l'export in JSON: {ex.Message}");
            }
        }

        private static void SetString(SerializedObject so, string propertyName, string value)
        {
            if (value == null) return;
            SerializedProperty prop = so.FindProperty(propertyName);
            if (prop != null)
                prop.stringValue = value;
        }
    }
}
