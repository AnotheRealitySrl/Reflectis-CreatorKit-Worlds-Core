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

        [InitializeOnLoadMethod]
        static void OnReloadAfterInstall()
        {
            if (SessionState.GetBool("PENDING_HYBRIDCLR_SETUP", false))
            {
                SessionState.SetBool("PENDING_HYBRIDCLR_SETUP", false);
                HotUpdateSetupper.Setup();   // crea assembly + registra nei settings
                Debug.Log("[Setup] Configurazione HybridCLR completata automaticamente.");
            }
        }

        // ============================================================
        //  SETUP — da lanciare UNA VOLTA per predisporre il progetto
        // ============================================================
        [MenuItem("Reflectis Worlds/Creator Kit/Core/Setup Interpreted Scripting")]
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

            UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget(
                new UnityEngine.Object[] { settings },
                "ProjectSettings/HybridCLRSettings.asset",
            true);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            Debug.Log("[Setup] asmdef registrato nelle Hot Update Assembly Definitions.");
        }

        // ============================================================
        //  COMPILA DLL — operazione ricorrente
        // ============================================================     
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

        [MenuItem("Reflectis Worlds/Creator Kit/Core/Compile Interpreted Scripting")]
        public static async void CompilaEPubblica()
        {
            // 1) Verifica che il setup sia stato fatto (cartella + asmdef + registrazione)
            if (!IsHotUpdateReady())
            {
                Debug.LogWarning("[Pubblica] HotUpdate non configurato (asmdef mancante o non registrato). " +
                                 "Esegui prima il Setup. Operazione annullata.");
                return;
            }

            string productGuid = PlayerSettings.productGUID.ToString();
            string asmdefGuid = AssetDatabase.AssetPathToGUID(ASMDEF_PATH);
            string GUIDCode = productGuid + asmdefGuid;

            Debug.LogError("Codice identificativo = " + GUIDCode);

            CompilaDll();  // prima compila

            var target = EditorUserBuildSettings.activeBuildTarget;
            string dllPath = Path.Combine($"HybridCLRData/HotUpdateDlls/{target}", $"{ASMDEF_NAME}.dll");

            if (!File.Exists(dllPath))
            {
                Debug.LogError("[Pubblica] DLL non trovata, annullo l'upload.");
                return;
            }

            byte[] dllBytes = File.ReadAllBytes(dllPath);

            // === DA COMPLETARE COL TEAM WEB ===
            // Qui andra la POST verso il vostro endpoint di validazione.
            // Esempio concettuale (NON attivo):
            //
            // using var client = new System.Net.Http.HttpClient();
            // var content = new System.Net.Http.ByteArrayContent(dllBytes);
            // content.Headers.Add("Authorization", "Bearer <token>");
            // var resp = await client.PostAsync("https://vostro-backend/upload-dll", content);
            // if (resp.IsSuccessStatusCode) Debug.Log("[Pubblica] DLL caricata e validata.");
            // else Debug.LogError($"[Pubblica] Rifiutata dal backend: {resp.StatusCode}");
            // ===================================

            Debug.LogWarning($"[Pubblica] Upload non ancora attivo. DLL pronta ({dllBytes.Length} bytes). " +
                             "Collega l'endpoint del backend per attivare la pubblicazione.");
            await System.Threading.Tasks.Task.CompletedTask;
        }

        // Check di configurazione: asmdef esiste E registrato in HybridCLR
        static bool IsHotUpdateReady()
        {
            // a) l'asmdef esiste su disco?
            if (!File.Exists(ASMDEF_PATH))
                return false;

            // b) e' registrato nelle Settings di HybridCLR?
            var settings = HybridCLRSettings.Instance;
            var current = settings.hotUpdateAssemblyDefinitions;
            if (current == null) return false;

            var asmdefAsset = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionAsset>(ASMDEF_PATH);
            foreach (var a in current)
                if (a == asmdefAsset)
                    return true;

            return false;
        }
    }
}
