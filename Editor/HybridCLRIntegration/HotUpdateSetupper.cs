using System.IO;
using System.Linq;
using System.Threading.Tasks;

using HybridCLR.Editor.Commands;
using HybridCLR.Editor.Installer;
using HybridCLR.Editor.Settings;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

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
        const string ASMDEF_PREFIX = "HotUpdate_";

        /// <summary>
        /// Session flag raised by the Creator Kit setup window before it installs HybridCLR, so
        /// that <see cref="OnReloadAfterInstall"/> can finish the job on the domain reload that
        /// follows the package import. Must stay in sync with the window's own copy of the key.
        /// </summary>
        public const string PENDING_SETUP_KEY = "PENDING_HYBRIDCLR_SETUP";

        /// <summary>
        /// Assembly name of this project's hot-update code. It carries the project GUID so that
        /// two worlds authored in different projects never ship assemblies with the same name:
        /// the player resolves a scene's MonoScripts by assembly name, so a duplicate would make
        /// the world loaded second silently bind to the types of the first.
        /// </summary>
        public static string HotUpdateAssemblyName => ASMDEF_PREFIX + PlayerSettings.productGUID;

        /// <summary>Asset path of the asmdef that produces <see cref="HotUpdateAssemblyName"/>.</summary>
        public static string HotUpdateAsmdefPath => $"{HOTUPDATE_FOLDER}/{HotUpdateAssemblyName}.asmdef";

        [InitializeOnLoadMethod]
        static void OnReloadAfterInstall()
        {
            if (!SessionState.GetBool(PENDING_SETUP_KEY, false))
                return;

            SessionState.SetBool(PENDING_SETUP_KEY, false);
            Setup();
            Debug.Log("[Setup] HybridCLR configuration completed automatically after the install.");
        }

        // ============================================================
        //  SETUP — run once to prepare the project
        // ============================================================
        //[MenuItem("Reflectis Worlds/Creator Kit/Core/Setup Interpreted Scripting")]
        public static void Setup()
        {
            // Step 1: the interpreter itself, inside this Editor's IL2CPP. Nothing below matters
            // if the player is going to be built without it.
            EnsureInterpreterInstalled();

            // Step 2: the project-unique hot-update assembly.
            if (!Directory.Exists(HOTUPDATE_FOLDER))
            {
                Directory.CreateDirectory(HOTUPDATE_FOLDER);
                Debug.Log($"[Setup] Created folder {HOTUPDATE_FOLDER}");
            }

            if (!EnsureAsmdef())
                return;

            WarnAboutNestedAsmdefs();
            RegisterHotUpdateAssembly();

            // Report on what the build gate will actually check, not on the steps we just ran.
            string issue = GetSetupIssue();
            if (issue != null)
            {
                Debug.LogError($"[Setup] Setup incomplete: {issue}");
                return;
            }

            Debug.Log($"[Setup] Done. Write your scripts in {HOTUPDATE_FOLDER}; " +
                      $"they compile into '{HotUpdateAssemblyName}'.");
        }

        /// <summary>
        /// Brings the hot-update assembly definition to its expected state: one asmdef in
        /// <see cref="HOTUPDATE_FOLDER"/>, named <see cref="HotUpdateAssemblyName"/> both as a file
        /// and in its "name" field. An asmdef left from an earlier setup is renamed and rewritten
        /// in place rather than replaced, so its GUID — and therefore its HybridCLR registration
        /// and any reference the creator added — survives. Returns false when it cannot proceed.
        /// </summary>
        static bool EnsureAsmdef()
        {
            // Only the folder root: an asmdef in a subfolder is a separate assembly the creator
            // owns, not something this setup should rename.
            string[] existing = Directory.GetFiles(HOTUPDATE_FOLDER, "*.asmdef", SearchOption.TopDirectoryOnly)
                .Select(p => p.Replace('\\', '/'))
                .ToArray();

            if (existing.Length > 1)
            {
                Debug.LogError($"[Setup] {HOTUPDATE_FOLDER} contains {existing.Length} assembly definitions " +
                               $"({string.Join(", ", existing.Select(Path.GetFileName))}). Unity allows one per " +
                               "folder: keep a single one and run the setup again.");
                return false;
            }

            string asmdefPath = HotUpdateAsmdefPath;

            if (existing.Length == 0)
            {
                File.WriteAllText(asmdefPath, BuildAsmdefContent());
                AssetDatabase.Refresh();
                Debug.Log($"[Setup] Created asmdef {asmdefPath}");
                return true;
            }

            // Rename the file first, so the "name" field is aligned on the final path.
            if (existing[0] != asmdefPath)
            {
                string error = AssetDatabase.RenameAsset(existing[0], HotUpdateAssemblyName);
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"[Setup] Could not rename {existing[0]} to {HotUpdateAssemblyName}: {error}");
                    return false;
                }
                Debug.Log($"[Setup] Renamed {Path.GetFileName(existing[0])} → {Path.GetFileName(asmdefPath)}");
            }

            AlignDeclaredAssemblyName(asmdefPath);
            return true;
        }

        static string BuildAsmdefContent() =>
$@"{{
    ""name"": ""{HotUpdateAssemblyName}"",
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
}}";

        /// <summary>
        /// Rewrites the asmdef's "name" field to <see cref="HotUpdateAssemblyName"/>, leaving every
        /// other field untouched. Renaming the file is not enough on its own: the field is what
        /// Unity compiles the assembly as, so a stale "HotUpdate" there keeps shipping an assembly
        /// that collides with the one of every other project.
        /// </summary>
        static void AlignDeclaredAssemblyName(string asmdefPath)
        {
            try
            {
                JObject asmdef = JObject.Parse(File.ReadAllText(asmdefPath));
                string declared = (string)asmdef["name"];

                if (declared == HotUpdateAssemblyName)
                {
                    Debug.Log($"[Setup] asmdef already declares '{HotUpdateAssemblyName}', skipping.");
                    return;
                }

                asmdef["name"] = HotUpdateAssemblyName;
                File.WriteAllText(asmdefPath, asmdef.ToString(Formatting.Indented));
                AssetDatabase.ImportAsset(asmdefPath, ImportAssetOptions.ForceUpdate);

                Debug.Log($"[Setup] {Path.GetFileName(asmdefPath)}: assembly name " +
                          $"'{declared}' → '{HotUpdateAssemblyName}'.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Setup] Could not rewrite the assembly name in {asmdefPath}: {e.Message}");
            }
        }

        /// <summary>
        /// Flags asmdefs nested under the hot-update folder. They are separate assemblies this
        /// setup does not manage, and each one ships under whatever name it declares — a generic
        /// one collides with the assemblies of other projects in the player.
        /// </summary>
        static void WarnAboutNestedAsmdefs()
        {
            foreach (string nested in Directory.GetFiles(HOTUPDATE_FOLDER, "*.asmdef", SearchOption.AllDirectories))
            {
                string normalized = nested.Replace('\\', '/');
                if (normalized == HotUpdateAsmdefPath)
                    continue;

                Debug.LogWarning($"[Setup] Assembly definition nested under {HOTUPDATE_FOLDER}: {normalized} " +
                                 $"(declares '{ReadDeclaredAssemblyName(nested)}'). The setup does not manage " +
                                 "it — make sure its name cannot collide with another project's assembly.");
            }
        }

        static string ReadDeclaredAssemblyName(string asmdefPath)
        {
            try { return (string)JObject.Parse(File.ReadAllText(asmdefPath))["name"]; }
            catch { return null; }
        }

        static void RegisterHotUpdateAssembly()
        {
            HybridCLRSettings settings = HybridCLRSettings.Instance;

            AssemblyDefinitionAsset asmdefAsset =
                AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(HotUpdateAsmdefPath);
            if (asmdefAsset == null)
            {
                Debug.LogError($"[Setup] Cannot load the asmdef to register at {HotUpdateAsmdefPath}.");
                return;
            }

            AssemblyDefinitionAsset[] current = settings.hotUpdateAssemblyDefinitions;

            if (current != null && current.Any(a => a == asmdefAsset))
            {
                Debug.Log("[Setup] asmdef already registered in HybridCLR, skipping.");
                return;
            }

            int len = current?.Length ?? 0;
            AssemblyDefinitionAsset[] updated = new AssemblyDefinitionAsset[len + 1];
            current?.CopyTo(updated, 0);
            updated[len] = asmdefAsset;
            settings.hotUpdateAssemblyDefinitions = updated;

            InternalEditorUtility.SaveToSerializedFileAndForget(
                new Object[] { settings },
                "ProjectSettings/HybridCLRSettings.asset",
                true);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
            Debug.Log("[Setup] asmdef registered in the Hot Update Assembly Definitions.");
        }

        // ============================================================
        //  SETUP VALIDATION
        // ============================================================
        /// <summary>
        /// Checks that the project can compile hot-update code safely: the uniquely named asmdef
        /// exists, declares that same name, and is registered with HybridCLR. Returns <c>null</c>
        /// when everything is in place, otherwise the reason to show the user.
        /// </summary>
        public static string GetSetupIssue()
        {
            InstallerController installer = null;
            try { installer = new InstallerController(); } catch { /* manifest unreadable */ }

            if (installer == null || !installer.HasInstalledHybridCLR())
            {
                return "the HybridCLR interpreter is not installed into this Editor's IL2CPP — the " +
                       "UPM package alone does not install it, and a player built like this ships " +
                       "without an interpreter, so hot-update assemblies would load but never run. " +
                       "Re-run the interpreter configuration.";
            }

            string asmdefPath = HotUpdateAsmdefPath;

            if (!File.Exists(asmdefPath))
            {
                return $"the hot-update assembly definition is missing ({asmdefPath}). " +
                       "Open the Creator Kit setup window and configure the interpreter.";
            }

            string declared = ReadDeclaredAssemblyName(asmdefPath);
            if (declared != HotUpdateAssemblyName)
            {
                return $"{asmdefPath} declares assembly name '{declared}' instead of " +
                       $"'{HotUpdateAssemblyName}'. A generic name collides with the hot-update " +
                       "assembly of any other project once both are loaded in the player. " +
                       "Re-run the interpreter configuration to align it.";
            }

            AssemblyDefinitionAsset asmdefAsset =
                AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath);
            if (asmdefAsset == null)
            {
                return $"{asmdefPath} exists on disk but Unity has not imported it yet. " +
                       "Let the editor refresh and retry.";
            }

            HybridCLRSettings settings = null;
            try { settings = HybridCLRSettings.Instance; } catch { /* HybridCLR not ready */ }

            AssemblyDefinitionAsset[] defs = settings?.hotUpdateAssemblyDefinitions;
            if (defs == null || !defs.Any(a => a == asmdefAsset))
            {
                return $"'{HotUpdateAssemblyName}' is not registered in HybridCLR's Hot Update " +
                       "Assembly Definitions, so it would be compiled into the player instead of " +
                       "being interpreted. Re-run the interpreter configuration.";
            }

            return null;
        }

        /// <summary>True when <see cref="GetSetupIssue"/> finds nothing to complain about.</summary>
        public static bool IsHotUpdateReady() => GetSetupIssue() == null;

        // ============================================================
        //  INTERPRETER INSTALL
        // ============================================================
        /// <summary>
        /// Installs HybridCLR's patched libil2cpp into this Editor's IL2CPP when it is not there
        /// yet. Adding the UPM package is NOT enough: without this step the player is built with
        /// the stock IL2CPP and ships no interpreter, so hot-update assemblies load but never run.
        /// Clones two git repositories, so git has to be on PATH and the call takes a while.
        /// </summary>
        public static bool EnsureInterpreterInstalled()
        {
            InstallerController installer;
            try
            {
                installer = new InstallerController();
            }
            catch (System.Exception e)
            {
                Debug.LogError("[Setup] Cannot read the HybridCLR version manifest: " + e.Message);
                return false;
            }

            if (installer.HasInstalledHybridCLR())
            {
                Debug.Log("[Setup] HybridCLR interpreter already installed (libil2cpp " +
                          $"v{installer.InstalledLibil2cppVersion ?? "unknown"}).");
                return true;
            }

            if (installer.GetCompatibleType() == InstallerController.CompatibleType.Incompatible)
            {
                Debug.LogError("[Setup] HybridCLR is incompatible with this Unity version. Minimum: " +
                               installer.GetCurrentUnityVersionMinCompatibleVersionStr());
                return false;
            }

            try
            {
                EditorUtility.DisplayProgressBar("Interpreted scripting",
                    "Installing the HybridCLR interpreter (cloning hybridclr and il2cpp_plus)...", 0.5f);
                installer.InstallDefaultHybridCLR();
            }
            catch (System.Exception e)
            {
                Debug.LogError("[Setup] HybridCLR interpreter install failed (is git on PATH?): " + e.Message);
                return false;
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            // The installer logs and returns instead of throwing on some failures, so the state on
            // disk is the only trustworthy outcome.
            if (!installer.HasInstalledHybridCLR())
            {
                Debug.LogError("[Setup] HybridCLR interpreter install did not produce a patched " +
                               "libil2cpp. See the errors above.");
                return false;
            }

            Debug.Log("[Setup] HybridCLR interpreter installed into the local IL2CPP.");
            return true;
        }

        // ============================================================
        //  COMPILE DLL — recurring operation
        // ============================================================
        public static void CompileDll()
        {
            foreach (BuildTarget target in TARGETS)
            {
                Debug.Log($"[Compile] Compiling the DLL for target: {target} ...");

                try
                {
                    CompileDllCommand.CompileDll(target);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[Compile] Error compiling for {target}: {e.Message}");
                    continue;
                }

                string dllPath = Path.Combine(
                    $"HybridCLRData/HotUpdateDlls/{target}", $"{HotUpdateAssemblyName}.dll");

                if (File.Exists(dllPath))
                    Debug.Log($"[Compile] DLL produced for {target}: {Path.GetFullPath(dllPath)}");
                else
                    Debug.LogWarning($"[Compile] {target} compiled, but no DLL was found at: {dllPath}");
            }

            Debug.Log("[Compile] Compilation cycle completed.");
        }

        // ============================================================
        //  BUILD + VERIFY — used by the Addressables build gate
        // ============================================================
        /// <summary>
        /// Builds the hot-update DLL, then runs the LOCAL whitelist check and the AUTHORITATIVE
        /// SERVER check. Returns true only if BOTH pass; otherwise logs the reason and returns
        /// false so the caller can block the addressables build (fail-closed).
        /// </summary>
        public static async Task<bool> CompileVerifyAsync()
        {
            // 0) The project has to be set up, and set up with a project-unique assembly name: a
            //    generically named assembly produces a world that shadows — or is shadowed by —
            //    any other world loaded in the same player session.
            string setupIssue = GetSetupIssue();
            if (setupIssue != null)
            {
                Debug.LogError($"[HotUpdateSecurity] Interpreted scripting is not set up: {setupIssue} Build blocked.");
                return false;
            }

            // 1) Build the DLL(s).
            CompileDll();

            // 2) Resolve the freshly compiled assembly (ScriptAssemblies → has a PDB for line info).
            string dllPath = HotUpdateDllLocator.ResolveDefaultDllPath(out _);
            if (string.IsNullOrEmpty(dllPath) || !File.Exists(dllPath))
            {
                Debug.LogError("[HotUpdateSecurity] Compiled HotUpdate assembly not found after build. Build blocked.");
                return false;
            }

            // 3) LOCAL check (download the shared policy, then verify). Block on fail.
            HotUpdatePolicyFetcher.FetchResult fetch = await HotUpdatePolicyFetcher.FetchAsync();
            if (!fetch.Ok)
            {
                Debug.LogError("[HotUpdateSecurity] Policy unavailable — build blocked (fail-closed). " + fetch.Error);
                return false;
            }

            VerificationResult local = HotUpdateDllLocator.VerifyAndLog(dllPath, fetch.Policy);
            if (!local.Passed)
            {
                Debug.LogError("[HotUpdateSecurity] LOCAL check FAILED — build blocked. See the violations above.");
                return false;
            }

            // 4) SERVER check (authoritative). Block on rejection OR if it can't complete.
            byte[] bytes = File.ReadAllBytes(dllPath);
            HotUpdateServerVerifier.Result server = await HotUpdateServerVerifier.VerifyAsync(bytes, Path.GetFileName(dllPath));

            if (!server.Reachable)
            {
                Debug.LogError("[HotUpdateSecurity] SERVER check could not complete — build blocked. " + server.Error);
                return false;
            }
            if (!server.Passed)
            {
                LogServerViolations(server.Response);
                Debug.LogError("[HotUpdateSecurity] SERVER check REJECTED the DLL — build blocked.");
                return false;
            }

            Debug.Log("[HotUpdateSecurity] Local + server checks PASSED. Proceeding with the addressables build.");
            return true;
        }

        private static void LogServerViolations(HotUpdateServerVerifier.ServerResponse resp)
        {
            if (resp?.Violations == null)
                return;
            foreach (HotUpdateServerVerifier.ServerViolation v in resp.Violations)
                Debug.LogError($"[HotUpdateSecurity][server] [{v.Kind}] {v.Detail}  ({v.Location})");
        }

        // ============================================================
        //  LEGACY publish stub — superseded by the Addressables build gate above.
        //  Kept until the team confirms it can go; the upload was never wired.
        // ============================================================
        //[MenuItem("Reflectis Worlds/Creator Kit/Core/Compile Interpreted Scripting")]
        public static async void CompileAndPublish()
        {
            string setupIssue = GetSetupIssue();
            if (setupIssue != null)
            {
                Debug.LogWarning($"[Publish] Interpreted scripting is not set up: {setupIssue} Aborted.");
                return;
            }

            if (!ScriptsChanged())
            {
                Debug.Log("[Publish] No change to the HotUpdate scripts since the last compilation. Skipping.");
                return;
            }

            CompileDll();
            SaveScriptsHash();

            BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
            string dllPath = Path.Combine(
                $"HybridCLRData/HotUpdateDlls/{target}", $"{HotUpdateAssemblyName}.dll");

            if (!File.Exists(dllPath))
            {
                Debug.LogError("[Publish] DLL not found, aborting the upload.");
                return;
            }

            byte[] dllBytes = File.ReadAllBytes(dllPath);

            Debug.LogWarning($"[Publish] Upload not wired yet. DLL ready ({dllBytes.Length} bytes). " +
                             "Connect the backend endpoint to enable publishing.");
            await Task.CompletedTask;
        }

        const string HASH_PREF_KEY = "HotUpdate_LastScriptsHash";

        /// <summary>Hash of every script in the hot-update folder, name included so that adding or
        /// removing a file changes the result.</summary>
        static string ComputeScriptsHash()
        {
            string[] files = Directory.GetFiles(HOTUPDATE_FOLDER, "*.cs", SearchOption.AllDirectories);
            System.Array.Sort(files); // stable order, otherwise the hash varies at random

            using System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            System.Text.StringBuilder sb = new();

            foreach (string file in files)
            {
                sb.Append(file);
                sb.Append(File.ReadAllText(file));
            }

            byte[] hashBytes = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sb.ToString()));
            return System.Convert.ToBase64String(hashBytes);
        }

        static bool ScriptsChanged() => ComputeScriptsHash() != EditorPrefs.GetString(HASH_PREF_KEY, "");

        static void SaveScriptsHash() => EditorPrefs.SetString(HASH_PREF_KEY, ComputeScriptsHash());
    }
}
