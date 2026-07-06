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

        static readonly BuildTarget[] TARGETS = {
            BuildTarget.StandaloneWindows64,
            BuildTarget.Android,
            BuildTarget.WebGL
        };

        const string HOTUPDATE_FOLDER = "Assets/HotUpdate";
        const string ASMDEF_NAME = "HotUpdate";
        const string ASMDEF_PATH = "Assets/HotUpdate/HotUpdate_";

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

            string productGuid = PlayerSettings.productGUID.ToString();
            string asmdefGuid = AssetDatabase.AssetPathToGUID(ASMDEF_PATH);
            string GUIDCode = productGuid + asmdefGuid;
            string correctAsmdefPath = ASMDEF_PATH + GUIDCode + ".asmdef";

            // 2) Crea l'asmdef se non esiste
            if (!File.Exists(correctAsmdefPath))
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
                File.WriteAllText(correctAsmdefPath, asmdefContent);
                AssetDatabase.Refresh();
                Debug.Log($"[Setup] Creato asmdef {correctAsmdefPath}");
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

            string productGuid = PlayerSettings.productGUID.ToString();
            string asmdefGuid = AssetDatabase.AssetPathToGUID(ASMDEF_PATH);
            string GUIDCode = productGuid + asmdefGuid;
            string correctAsmdefPath = ASMDEF_PATH + GUIDCode + ".asmdef";

            // Carica l'AssemblyDefinitionAsset dell'asmdef
            var asmdefAsset = AssetDatabase.LoadAssetAtPath<UnityEditorInternal.AssemblyDefinitionAsset>(correctAsmdefPath);
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
            foreach (var target in TARGETS)
            {
                Debug.Log($"[Compila] Compilo la DLL per target: {target} ...");

                try
                {
                    CompileDllCommand.CompileDll(target);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[Compila] Errore compilando per {target}: {e.Message}");
                    continue;   // passa al target successivo
                }

                // Verifica che la DLL sia stata prodotta nel path del target
                string outputDir = $"HybridCLRData/HotUpdateDlls/{target}";

                string productGuid = PlayerSettings.productGUID.ToString();
                string asmdefGuid = AssetDatabase.AssetPathToGUID(ASMDEF_PATH);
                string GUIDCode = productGuid + asmdefGuid;
                string correctAsmdefPath = ASMDEF_PATH + GUIDCode + ".asmdef";

                string dllPath = Path.Combine(outputDir, $"{correctAsmdefPath}.dll");

                if (File.Exists(dllPath))
                    Debug.Log($"[Compila] DLL generata per {target}: {Path.GetFullPath(dllPath)}");
                else
                    Debug.LogWarning($"[Compila] Compilazione di {target} fatta, ma DLL non trovata in: {dllPath}");
            }

            Debug.Log("[Compila] Ciclo di compilazione completato."); 
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

            if (!ScriptsCambiati())
            {
                Debug.Log("[Pubblica] Nessuna modifica agli script HotUpdate dall'ultima compilazione. Salto.");
                return;   // oppure prosegui solo con l'upload della DLL esistente, vedi sotto
            }

            string productGuid = PlayerSettings.productGUID.ToString();
            string asmdefGuid = AssetDatabase.AssetPathToGUID(ASMDEF_PATH);
            string GUIDCode = productGuid + asmdefGuid;

            Debug.LogError("Codice identificativo = " + GUIDCode);

            CompilaDll();  // prima compila
            SalvaHashScripts();

            var target = EditorUserBuildSettings.activeBuildTarget;
            string dllPath = Path.Combine($"HybridCLRData/HotUpdateDlls/{target}", $"{ASMDEF_NAME}.dll");

            if (!File.Exists(dllPath))
            {
                Debug.LogError("[Pubblica] DLL non trovata, annullo l'upload.");
                return;
            }

            byte[] dllBytes = File.ReadAllBytes(dllPath);


            //REQUIRE ID
            //GET ID
            //RESEND DLL WITH ID

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

        // Calcola un hash del contenuto di tutti gli script dell'assembly HotUpdate
        static string CalcolaHashScripts()
        {
            // Trova tutti i .cs nella cartella HotUpdate
            string[] files = Directory.GetFiles(HOTUPDATE_FOLDER, "*.cs", SearchOption.AllDirectories);
            System.Array.Sort(files); // ordine stabile, altrimenti l'hash varia a caso

            using var md5 = System.Security.Cryptography.MD5.Create();
            var sb = new System.Text.StringBuilder();

            foreach (string file in files)
            {
                // includi il nome (cosi aggiungere/rimuovere file cambia l'hash)
                sb.Append(file);
                sb.Append(File.ReadAllText(file));
            }

            byte[] hashBytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sb.ToString()));
            return System.Convert.ToBase64String(hashBytes);
        }

        const string HASH_PREF_KEY = "HotUpdate_LastScriptsHash";

        // Ritorna true se qualcosa e' cambiato dall'ultima compilazione
        static bool ScriptsCambiati()
        {
            string hashAttuale = CalcolaHashScripts();
            string hashPrecedente = EditorPrefs.GetString(HASH_PREF_KEY, "");

            if (hashAttuale == hashPrecedente)
                return false;   // nulla cambiato

            return true;
        }

        static void SalvaHashScripts()
        {
            EditorPrefs.SetString(HASH_PREF_KEY, CalcolaHashScripts());
        }
    }
}
