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
        // Every target the platform supports. The set is not optional: the backend
        // rejects an environment DLL bundle that misses one, because a world that
        // silently loses its scripted behaviour on a single platform is worse than a
        // publish that fails loudly.
        static readonly BuildTarget[] TARGETS = {
            BuildTarget.StandaloneWindows64,
            BuildTarget.Android,
            BuildTarget.iOS,
            BuildTarget.WebGL
        };

        const string HOTUPDATE_FOLDER = "Assets/HotUpdate";
        const string ASMDEF_PREFIX = "HotUpdate_";

        // HybridCLR ships with its gitee mirrors as the default, which do not resolve outside
        // China. Every fresh Creator Kit project would fail its first install on a DNS error
        // that says nothing about the cause, so the known defaults are swapped for the GitHub
        // originals. A URL the team deliberately changed (a private mirror, say) is left alone.
        const string GITEE_HYBRIDCLR = "https://gitee.com/focus-creative-games/hybridclr";
        const string GITEE_IL2CPP_PLUS = "https://gitee.com/focus-creative-games/il2cpp_plus";
        const string GITHUB_HYBRIDCLR = "https://github.com/focus-creative-games/hybridclr";
        const string GITHUB_IL2CPP_PLUS = "https://github.com/focus-creative-games/il2cpp_plus";

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

        /// <summary>
        /// <see cref="TARGETS"/> as the folder/platform names shared with the backend: the
        /// compiler writes each DLL under HotUpdateDlls/&lt;BuildTarget&gt;/ and the bundle keeps
        /// the same segment, so both sides speak one vocabulary.
        /// </summary>
        public static string[] TargetNames => TARGETS.Select(t => t.ToString()).ToArray();

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

            SaveHybridCLRSettings(settings);
            Debug.Log("[Setup] asmdef registered in the Hot Update Assembly Definitions.");
        }

        static void SaveHybridCLRSettings(HybridCLRSettings settings)
        {
            InternalEditorUtility.SaveToSerializedFileAndForget(
                new Object[] { settings },
                "ProjectSettings/HybridCLRSettings.asset",
                true);
            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
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

            RedirectGiteeMirrorsToGitHub();

            try
            {
                EditorUtility.DisplayProgressBar("Interpreted scripting",
                    "Installing the HybridCLR interpreter (cloning hybridclr and il2cpp_plus)...", 0.5f);
                installer.InstallDefaultHybridCLR();
            }
            catch (System.Exception e)
            {
                // The message carries the repository URL, which is usually the whole story:
                // git missing from PATH, or the host unreachable from this network.
                Debug.LogError("[Setup] HybridCLR interpreter install failed. Check that git is on " +
                               "PATH and that the repository below is reachable from here: " + e.Message);
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
        //  PUBLISH FINGERPRINT — skip work that would change nothing
        // ============================================================
        // Keyed by project GUID because EditorPrefs is global to the machine: a bare key would
        // have two Creator Kit projects reading each other's marker.
        const string FINGERPRINT_KEY_PREFIX = "Reflectis_HotUpdate_PublishFingerprint_";

        static string FingerprintKey => FINGERPRINT_KEY_PREFIX + PlayerSettings.productGUID;

        /// <summary>
        /// True when the last <see cref="CompileVerifyAsync"/> found the published bundle already
        /// current, so the caller can skip building, uploading and importing it. Meaningful only
        /// within the run that set it — this is the verdict of that check, not stored state.
        /// </summary>
        public static bool BundleIsCurrent { get; private set; }

        /// <summary>Fingerprint of what was just compiled, persisted once the publish succeeds.</summary>
        static string pendingFingerprint;

        /// <summary>
        /// Records that the bundle now on the platform matches what is in this project. Called by
        /// the publisher after the import succeeds — never before, or a failed publish would be
        /// remembered as done and the next build would skip it.
        /// </summary>
        public static void MarkBundlePublished()
        {
            if (string.IsNullOrEmpty(pendingFingerprint))
                return;

            EditorPrefs.SetString(FingerprintKey, pendingFingerprint);
            pendingFingerprint = null;
        }

        /// <summary>
        /// Identity of what a publish would produce. Covers the scripts, the assembly definition —
        /// a reference added there changes the compiler's output with every .cs byte untouched —
        /// and the whitelist, so a policy that tightens forces a re-verify instead of leaving
        /// already-published code accepted under the old rules until someone edits a script.
        ///
        /// It cannot be derived from the compiled DLLs: HybridCLR stamps a fresh timestamp, two
        /// MVID GUIDs and a debug checksum on every compilation, so identical source produces
        /// different bytes every time (measured: 74 differing bytes on a 5 KB assembly).
        /// </summary>
        static string ComputePublishFingerprint(string policyJson)
        {
            System.Text.StringBuilder sb = new();

            string[] files = Directory.GetFiles(HOTUPDATE_FOLDER, "*.cs", SearchOption.AllDirectories);
            System.Array.Sort(files, System.StringComparer.Ordinal); // stable order, or the hash varies at random

            foreach (string file in files)
            {
                // The path too: renaming or moving a script changes what compiles.
                sb.Append(file.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar))
                  .Append('\n');
                sb.Append(File.ReadAllText(file)).Append('\n');
            }

            if (File.Exists(HotUpdateAsmdefPath))
            {
                sb.Append(File.ReadAllText(HotUpdateAsmdefPath)).Append('\n');
            }

            sb.Append(policyJson ?? string.Empty);

            return HotUpdateDllLocator.Sha256Hex(System.Text.Encoding.UTF8.GetBytes(sb.ToString()));
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

            // 1) The whitelist comes first: it is needed to verify, and it takes part in the
            //    fingerprint below, so a policy that changed has to be in hand before deciding
            //    whether there is anything to do.
            HotUpdatePolicyFetcher.FetchResult fetch = await HotUpdatePolicyFetcher.FetchAsync();
            if (!fetch.Ok)
            {
                Debug.LogError("[HotUpdateSecurity] Policy unavailable — build blocked (fail-closed). " + fetch.Error);
                return false;
            }

            // 2) Nothing to rebuild? Then nothing to verify or republish either. The scenes still
            //    get linked to the assembly afterwards — a new scene against unchanged scripts is
            //    exactly the case that must not be skipped.
            string fingerprint = ComputePublishFingerprint(fetch.Json);
            if (fingerprint == EditorPrefs.GetString(FingerprintKey, string.Empty))
            {
                BundleIsCurrent = true;
                Debug.Log($"[HotUpdate] '{HotUpdateAssemblyName}' is unchanged since the last publish " +
                          "(scripts, assembly definition and whitelist all match). Skipping compile, " +
                          "verification and upload; the scenes are still linked to it.");
                return true;
            }

            BundleIsCurrent = false;

            // 3) Build the DLL(s).
            CompileDll();

            // 4) Resolve the freshly compiled assembly (ScriptAssemblies → has a PDB for line info).
            string dllPath = HotUpdateDllLocator.ResolveDefaultDllPath(out _);
            if (string.IsNullOrEmpty(dllPath) || !File.Exists(dllPath))
            {
                Debug.LogError("[HotUpdateSecurity] Compiled HotUpdate assembly not found after build. Build blocked.");
                return false;
            }

            // 5) LOCAL check against the policy fetched above. Block on fail.
            VerificationResult local = HotUpdateDllLocator.VerifyAndLog(dllPath, fetch.Policy);
            if (!local.Passed)
            {
                Debug.LogError("[HotUpdateSecurity] LOCAL check FAILED — build blocked. See the violations above.");
                return false;
            }

            // 6) SERVER check (authoritative). Block on rejection OR if it can't complete.
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

            // Held, not stored: the marker is written only once the publish itself succeeds.
            pendingFingerprint = fingerprint;

            Debug.Log("[HotUpdateSecurity] Local + server checks PASSED. Proceeding with the addressables build.");
            return true;
        }

        /// <summary>
        /// Points the installer at the GitHub originals when it is still on HybridCLR's gitee
        /// defaults, which do not resolve outside China — the clone fails with a bare DNS error
        /// that gives no hint of the cause. Only the known defaults are rewritten: a URL the team
        /// pointed somewhere else on purpose stays as it is.
        /// </summary>
        static void RedirectGiteeMirrorsToGitHub()
        {
            HybridCLRSettings settings = null;
            try { settings = HybridCLRSettings.Instance; } catch { /* HybridCLR not ready */ }

            if (settings == null)
                return;

            bool changed = false;

            if (settings.hybridclrRepoURL == GITEE_HYBRIDCLR)
            {
                settings.hybridclrRepoURL = GITHUB_HYBRIDCLR;
                changed = true;
            }

            if (settings.il2cppPlusRepoURL == GITEE_IL2CPP_PLUS)
            {
                settings.il2cppPlusRepoURL = GITHUB_IL2CPP_PLUS;
                changed = true;
            }

            if (!changed)
                return;

            SaveHybridCLRSettings(settings);
            Debug.Log("[Setup] HybridCLR source repositories switched from the gitee mirrors to GitHub.");
        }

        private static void LogServerViolations(HotUpdateServerVerifier.ServerResponse resp)
        {
            if (resp?.Violations == null)
                return;
            foreach (HotUpdateServerVerifier.ServerViolation v in resp.Violations)
                Debug.LogError($"[HotUpdateSecurity][server] [{v.Kind}] {v.Detail}  ({v.Location})");
        }
    }
}
