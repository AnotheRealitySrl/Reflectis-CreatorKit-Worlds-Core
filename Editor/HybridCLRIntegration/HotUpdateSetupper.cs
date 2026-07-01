// File: Assets/Editor/HotUpdateAutomation.cs
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Settings;

namespace Reflectis.CreatorKit.Worlds.Core.HybridCLR.Editor
{
    public static class HotUpdateSetupper
    {
        const string HOTUPDATE_FOLDER = "Assets/HotUpdate";
        const string ASMDEF_NAME = "HotUpdate";
        const string ASMDEF_PATH = "Assets/HotUpdate/HotUpdate.asmdef";

        // ============================================================
        //  SETUP — da lanciare UNA VOLTA per predisporre il progetto
        // ============================================================
        [MenuItem("Tools/HotUpdate/1. Setup (una tantum)")]
        public static void Setup()
        {
            // 1) Crea la cartella HotUpdate se non esiste
            if (!Directory.Exists(HOTUPDATE_FOLDER))
            {
                Directory.CreateDirectory(HOTUPDATE_FOLDER);
                Debug.Log($"[Setup] Creata cartella {HOTUPDATE_FOLDER}");
            }
            else
            {
                Debug.Log($"[Setup] Cartella {HOTUPDATE_FOLDER} gia esistente, salto.");
            }

            // 2) Crea l'asmdef se non esiste
            if (!File.Exists(ASMDEF_PATH))
            {
                string asmdefContent =
    @"{
    ""name"": ""HotUpdate"",
    ""rootNamespace"": """",
    ""references"": [],
    ""includePlatforms"": [],
    ""excludePlatforms"": [],
    ""allowUnsafeCode"": false,
    ""overrideReferences"": false,
    ""precompiledReferences"": [],
    ""autoReferenced"": true,
    ""defineConstraints"": [],
    ""versionDefines"": [],
    ""noEngineReferences"": false
}";
                File.WriteAllText(ASMDEF_PATH, asmdefContent);
                AssetDatabase.Refresh();
                Debug.Log($"[Setup] Creato asmdef {ASMDEF_PATH}");
            }
            else
            {
                Debug.Log($"[Setup] asmdef gia esistente, salto.");
            }

            // 3) Registra l'asmdef nelle Hot Update Assembly Definitions di HybridCLR
            RegisterHotUpdateAssembly();

            Debug.Log("[Setup] Completato. Ora puoi scrivere script in Assets/HotUpdate e usare 'Compila DLL'.");
        }

        static void RegisterHotUpdateAssembly()
        {
            var settings = HybridCLRSettings.Instance;

            // Carica l'AssemblyDefinitionAsset dell'asmdef
            var asmdefAsset = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionAsset>(ASMDEF_PATH);
            if (asmdefAsset == null)
            {
                Debug.LogError("[Setup] Non trovo l'asmdef da registrare.");
                return;
            }

            // Check: e' gia registrato?
            var current = settings.hotUpdateAssemblyDefinitions;
            bool giaPresente = false;
            if (current != null)
            {
                foreach (var a in current)
                    if (a == asmdefAsset) { giaPresente = true; break; }
            }

            if (giaPresente)
            {
                Debug.Log("[Setup] asmdef gia registrato in HybridCLR, salto.");
                return;
            }

            // Aggiungi in coda
            int len = current?.Length ?? 0;
            var nuovo = new UnityEditorInternal.AssemblyDefinitionAsset[len + 1];
            if (current != null) current.CopyTo(nuovo, 0);
            nuovo[len] = asmdefAsset;
            settings.hotUpdateAssemblyDefinitions = nuovo;

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            Debug.Log("[Setup] asmdef registrato nelle Hot Update Assembly Definitions.");
        }

        // ============================================================
        //  COMPILA DLL — operazione ricorrente
        // ============================================================
        [MenuItem("Tools/HotUpdate/2. Compila DLL")]
        public static void CompilaDll()
        {
            var target = EditorUserBuildSettings.activeBuildTarget;
            Debug.Log($"[Compila] Compilo la DLL per target: {target} ...");

            CompileDllCommand.CompileDll(target);

            // Il path di output ufficiale
            string outputDir = $"HybridCLRData/HotUpdateDlls/{target}";
            string dllPath = Path.Combine(outputDir, $"{ASMDEF_NAME}.dll");

            if (File.Exists(dllPath))
                Debug.Log($"[Compila] DLL generata: {Path.GetFullPath(dllPath)}");
            else
                Debug.LogWarning($"[Compila] Compilazione fatta, ma non trovo la DLL nel path atteso: {dllPath}. Controlla la cartella HybridCLRData/HotUpdateDlls/.");
        }
    }
}
