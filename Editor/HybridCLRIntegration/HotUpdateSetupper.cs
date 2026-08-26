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
        /// <summary>
        /// Every target a bundle must carry. The backend rejects an import that misses one, so
        /// this list and its RequiredPlatforms are one contract in two places: iOS is here
        /// because a world that loses its scripted behaviour on iPad only is worse than a
        /// publish that fails while the creator is watching.
        /// </summary>
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
        /// A message the creator has to read, held across the domain reload that is about to
        /// destroy it. The build gate renames the hot-update assembly when the source has moved,
        /// and a rename is a script recompilation: with the Console's "Clear on Recompile" on —
        /// its default — the warning explaining what happened is wiped by the very recompilation
        /// it announces, leaving a build that stopped for no visible reason.
        /// </summary>
        const string PENDING_NOTICE_KEY = "PENDING_HOTUPDATE_NOTICE";

        /// <summary>
        /// Prefix every assembly this project publishes shares: the project GUID keeps two
        /// different projects from ever shipping assemblies with the same name. The player
        /// resolves a scene's MonoScripts by assembly name, so a duplicate would make the world
        /// loaded second silently bind to the types of the first.
        /// </summary>
        public static string ProjectAssemblyPrefix => ASMDEF_PREFIX + PlayerSettings.productGUID + "_";

        /// <summary>
        /// Assembly name of this project's hot-update code AS IT STANDS: prefix plus a digest of
        /// the source it would compile from. The name therefore changes when the code changes,
        /// which is the whole mechanism —
        ///
        ///   * the backend keys its record on the name, so republishing unchanged code is
        ///     recognised and stored once instead of overwriting anything;
        ///   * two worlds can carry two versions of this project and one player can load both,
        ///     which is impossible when two sets of bytes share a name and none can be unloaded;
        ///   * and the DLLs already on disk under this name are, by definition, current.
        ///
        /// Falls back to the prefix alone when the fingerprint cannot be computed (no hot-update
        /// folder yet): that shape is refused by <see cref="GetSetupIssue"/> rather than
        /// published.
        /// </summary>
        public static string HotUpdateAssemblyName
        {
            get
            {
                string fingerprint = HotUpdateFingerprint.Compute(HOTUPDATE_FOLDER, TARGETS);

                return string.IsNullOrEmpty(fingerprint)
                    ? ProjectAssemblyPrefix
                    : ProjectAssemblyPrefix + fingerprint;
            }
        }

        /// <summary>
        /// Whether a declared assembly name is one this project produces — prefix plus an 8-digit
        /// hex fingerprint. Used where the question is "is this ours" rather than "is this
        /// current": the fingerprint of a name written before the last edit is legitimately
        /// stale, and staleness is not a setup problem, it is a rebuild.
        /// </summary>
        public static bool LooksLikeProjectAssembly(string declared)
        {
            if (string.IsNullOrEmpty(declared) || !declared.StartsWith(ProjectAssemblyPrefix, System.StringComparison.Ordinal))
                return false;

            string suffix = declared.Substring(ProjectAssemblyPrefix.Length);

            return suffix.Length == 8 && suffix.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f'));
        }

        /// <summary>
        /// Asset path the asmdef SHOULD have: the one that produces
        /// <see cref="HotUpdateAssemblyName"/>. Since that name tracks the source, this path moves
        /// with every edit — use it to create or rename towards, never to check what exists.
        /// </summary>
        public static string HotUpdateAsmdefPath => $"{HOTUPDATE_FOLDER}/{HotUpdateAssemblyName}.asmdef";

        /// <summary>
        /// <see cref="TARGETS"/> as the folder/platform names shared with the backend: the
        /// compiler writes each DLL under HotUpdateDlls/&lt;BuildTarget&gt;/ and the bundle keeps
        /// the same segment, so both sides speak one vocabulary.
        /// </summary>
        public static string[] TargetNames => TARGETS.Select(t => t.ToString()).ToArray();

        /// <summary>
        /// Asset path of the asmdef that is actually on disk at the root of the hot-update folder,
        /// or null when there is none. This is what "is the project set up" has to be asked about:
        /// after an edit the file is still there, correctly, under the name of the previous
        /// fingerprint — looking for <see cref="HotUpdateAsmdefPath"/> would report it missing.
        /// </summary>
        public static string ExistingAsmdefPath
        {
            get
            {
                if (!Directory.Exists(HOTUPDATE_FOLDER))
                    return null;

                return Directory.GetFiles(HOTUPDATE_FOLDER, "*.asmdef", SearchOption.TopDirectoryOnly)
                    .Select(p => p.Replace('\\', '/'))
                    .OrderBy(p => p, System.StringComparer.Ordinal)
                    .FirstOrDefault();
            }
        }

        /// <summary>
        /// Brings the asmdef's name to the digest of the current source, if it is not there
        /// already. Returns true when the project is in a state where the question even applies —
        /// so a caller can tell "nothing to do" from "not set up".
        ///
        /// Called from the build gate and nowhere else. Doing it on every script reload instead
        /// would keep the name always current, at the price of a second recompilation after every
        /// save the creator makes — too much to pay all day for something only a publish needs.
        /// </summary>
        /// <param name="renamed">
        /// True when a rename actually happened, which means Unity is now recompiling and the
        /// assembly on disk is not the one the name promises until it finishes.
        /// </param>
        static bool AlignAssemblyNameToSource(out bool renamed)
        {
            renamed = false;

            string asmdefPath = ExistingAsmdefPath;
            if (asmdefPath == null)
                return false;

            string expected = HotUpdateAssemblyName;

            // The fallback shape, prefix with no digest, means the fingerprint could not be
            // computed. Renaming to that would strip the project's identity down to something two
            // publishes could share, so leave the asmdef alone and let GetSetupIssue explain.
            if (expected == ProjectAssemblyPrefix)
                return false;

            string declared = ReadDeclaredAssemblyName(asmdefPath);
            if (declared == expected)
                return true;

            if (!EnsureAsmdef(expected))
            {
                Debug.LogError($"[HotUpdate] Could not rename the hot-update assembly to '{expected}'.");
                return false;
            }

            renamed = true;

            string notice = $"[HotUpdate] The scripts changed: the assembly is now '{expected}' " +
                            $"(was '{declared}'). Unity is recompiling it.";

            // Logged now, and held for the reload the rename is about to trigger: with the
            // Console's Clear on Recompile enabled, this line would otherwise be wiped by the
            // very recompilation it announces. The caller that is mid-publish parks its own work
            // and picks it up on the other side, so this is information, not an instruction.
            Debug.Log(notice);
            SessionState.SetString(PENDING_NOTICE_KEY, notice);

            return true;
        }

        /// <summary>
        /// Re-logs the message the aligner or the gate left behind before triggering a reload. Separate from
        /// <see cref="OnReloadAfterInstall"/> because it has nothing to do with the install: the
        /// two just happen to need the same moment, the first frame after the domain is back.
        /// </summary>
        [InitializeOnLoadMethod]
        static void ReplayPendingNotice()
        {
            string notice = SessionState.GetString(PENDING_NOTICE_KEY, string.Empty);
            if (string.IsNullOrEmpty(notice))
                return;

            SessionState.EraseString(PENDING_NOTICE_KEY);
            Debug.Log(notice);
        }

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

            // Read once and carried: the name digests the source, so re-reading it between the
            // steps below would let a file saved mid-setup split them across two identities.
            string assemblyName = HotUpdateAssemblyName;

            if (!EnsureAsmdef(assemblyName))
                return;

            WarnAboutNestedAsmdefs();
            RegisterHotUpdateAssembly(assemblyName);

            // Report on what the build gate will actually check, not on the steps we just ran.
            string issue = GetSetupIssue();
            if (issue != null)
            {
                Debug.LogError($"[Setup] Setup incomplete: {issue}");
                return;
            }

            Debug.Log($"[Setup] Done. Write your scripts in {HOTUPDATE_FOLDER}; " +
                      $"they compile into '{assemblyName}'.");
        }

        /// <summary>
        /// Brings the hot-update assembly definition to its expected state: one asmdef in
        /// <see cref="HOTUPDATE_FOLDER"/>, named <see cref="HotUpdateAssemblyName"/> both as a file
        /// and in its "name" field. An asmdef left from an earlier setup is renamed and rewritten
        /// in place rather than replaced, so its GUID — and therefore its HybridCLR registration
        /// and any reference the creator added — survives. Returns false when it cannot proceed.
        /// </summary>
        /// <param name="assemblyName">
        /// The name to converge on, read once by the caller. Passed rather than re-read: the
        /// property rescans the source on every access, and this method uses the name four times
        /// — as a file name, in the rename, in the comparison and in the "name" field — so a file
        /// saved halfway through could leave the asmdef called one thing and declaring another.
        /// </param>
        static bool EnsureAsmdef(string assemblyName)
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

            string asmdefPath = $"{HOTUPDATE_FOLDER}/{assemblyName}.asmdef";

            if (existing.Length == 0)
            {
                File.WriteAllText(asmdefPath, BuildAsmdefContent(assemblyName));
                AssetDatabase.Refresh();
                Debug.Log($"[Setup] Created asmdef {asmdefPath}");
                return true;
            }

            // Rename the file first, so the "name" field is aligned on the final path.
            if (existing[0] != asmdefPath)
            {
                string error = AssetDatabase.RenameAsset(existing[0], assemblyName);
                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"[Setup] Could not rename {existing[0]} to {assemblyName}: {error}");
                    return false;
                }
                Debug.Log($"[Setup] Renamed {Path.GetFileName(existing[0])} → {Path.GetFileName(asmdefPath)}");
            }

            AlignDeclaredAssemblyName(asmdefPath, assemblyName);
            return true;
        }

        static string BuildAsmdefContent(string assemblyName) =>
$@"{{
    ""name"": ""{assemblyName}"",
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
        static void AlignDeclaredAssemblyName(string asmdefPath, string assemblyName)
        {
            try
            {
                JObject asmdef = JObject.Parse(File.ReadAllText(asmdefPath));
                string declared = (string)asmdef["name"];

                if (declared == assemblyName)
                {
                    Debug.Log($"[Setup] asmdef already declares '{assemblyName}', skipping.");
                    return;
                }

                asmdef["name"] = assemblyName;
                File.WriteAllText(asmdefPath, asmdef.ToString(Formatting.Indented));
                AssetDatabase.ImportAsset(asmdefPath, ImportAssetOptions.ForceUpdate);

                Debug.Log($"[Setup] {Path.GetFileName(asmdefPath)}: assembly name " +
                          $"'{declared}' → '{assemblyName}'.");
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
                if (normalized == ExistingAsmdefPath)
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

        static void RegisterHotUpdateAssembly(string assemblyName)
        {
            HybridCLRSettings settings = HybridCLRSettings.Instance;

            string asmdefPath = $"{HOTUPDATE_FOLDER}/{assemblyName}.asmdef";

            AssemblyDefinitionAsset asmdefAsset =
                AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(asmdefPath);
            if (asmdefAsset == null)
            {
                Debug.LogError($"[Setup] Cannot load the asmdef to register at {asmdefPath}.");
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

            string asmdefPath = ExistingAsmdefPath;

            if (asmdefPath == null || !File.Exists(asmdefPath))
            {
                return $"the hot-update assembly definition is missing ({HOTUPDATE_FOLDER}). " +
                       "Open the Creator Kit setup window and configure the interpreter.";
            }

            // Shape, not currency: the name carries a digest of the source, so the one on disk
            // is stale the moment the creator edits a script. That is a rebuild (handled by the
            // build gate, which renames and recompiles), not a broken setup. What would be broken
            // is a name that is not this project's at all — a generic one collides with the
            // hot-update assembly of any other project once both are loaded in the player.
            string declared = ReadDeclaredAssemblyName(asmdefPath);
            if (!LooksLikeProjectAssembly(declared))
            {
                return $"{asmdefPath} declares assembly name '{declared}', which is not of the " +
                       $"form '{ProjectAssemblyPrefix}<fingerprint>'. Re-run the interpreter " +
                       "configuration to align it.";
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
        /// <summary>
        /// Path of the per-target hot-update DLL HybridCLR produces for a given assembly name.
        /// </summary>
        static string TargetDllPath(BuildTarget target, string assemblyName)
            => Path.Combine($"HybridCLRData/HotUpdateDlls/{target}", $"{assemblyName}.dll");

        /// <summary>
        /// Whether every target already has a DLL compiled for that assembly name. Since the name
        /// is a digest of the source, their presence means they were compiled from exactly this
        /// code — so there is nothing to recompile.
        /// </summary>
        public static bool CompiledDllsArePresent(string assemblyName)
            => !string.IsNullOrEmpty(assemblyName)
               && TARGETS.All(target => File.Exists(TargetDllPath(target, assemblyName)));

        /// <summary>
        /// Targets whose DLL is missing for that assembly name. Empty means the set is complete —
        /// which the build gate insists on, because the backend rejects a partial bundle.
        /// </summary>
        public static BuildTarget[] MissingTargets(string assemblyName)
            => TARGETS.Where(target => !File.Exists(TargetDllPath(target, assemblyName))).ToArray();

        /// <summary>
        /// Compiles the hot-update assembly for every target, or does nothing when the DLLs for
        /// that name are already there — the name digests the source, so their presence means
        /// they came from exactly this code.
        /// </summary>
        /// <param name="assemblyName">
        /// Taken as a parameter rather than read from <see cref="HotUpdateAssemblyName"/>, which
        /// rescans the source on every access: the caller has already decided which name this run
        /// is about, and a file saved halfway through would otherwise have us compile under one
        /// name while the caller verifies another.
        /// </param>
        public static void CompileDll(string assemblyName)
        {
            if (CompiledDllsArePresent(assemblyName))
            {
                Debug.Log($"[Compile] {assemblyName} is already compiled for every target — " +
                          "the source has not changed, nothing to do.");
                return;
            }

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

                string dllPath = TargetDllPath(target, assemblyName);

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

        /// <summary>
        /// True when the last <see cref="CompileVerifyAsync"/> stopped because it renamed the
        /// hot-update assembly and Unity is recompiling it — not because anything was rejected.
        /// The caller has to tell the two apart: one asks the creator to press build again, the
        /// other says their code was refused.
        /// </summary>
        public static bool AwaitingRecompile { get; private set; }

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
        /// What a publish would produce, plus the rules it must satisfy. Two inputs only:
        ///
        ///   * the assembly name, which already digests the source — the scripts, the assembly
        ///     definition minus its own name field, the Unity version, the per-target defines and
        ///     the resolved package set (see <see cref="HotUpdateFingerprint"/>). Re-scanning the
        ///     project here would be a second implementation of the same thing, free to drift from
        ///     the one the name is built from;
        ///   * the whitelist, so a policy that tightens forces a re-verify instead of leaving
        ///     already-published code accepted under the old rules until someone edits a script.
        ///
        /// The policy stays OUT of the assembly name for the same reason it belongs here: it
        /// decides whether the code is acceptable, not what the code compiles to. Folding it into
        /// the name would mint a new identity for bytes that did not change, and the backend would
        /// store a second copy of an assembly it already has.
        /// </summary>
        static string ComputePublishFingerprint(string assemblyName, string policyJson)
        {
            return HotUpdateDllLocator.Sha256Hex(System.Text.Encoding.UTF8.GetBytes(
                (assemblyName ?? string.Empty) + "\n" + (policyJson ?? string.Empty)));
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
            //    publish marker below, so a policy that changed has to be in hand before deciding
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
            //
            //    Read once and carried through the steps below: the property rescans the source on
            //    every access, and this run must not straddle two identities.
            string expected = HotUpdateAssemblyName;
            string fingerprint = ComputePublishFingerprint(expected, fetch.Json);

            if (fingerprint == EditorPrefs.GetString(FingerprintKey, string.Empty))
            {
                BundleIsCurrent = true;
                Debug.Log($"[HotUpdate] '{expected}' is unchanged since the last publish " +
                          "(scripts, assembly definition, build inputs and whitelist all match). Skipping " +
                          "compile, verification and upload; the scenes are still linked to it.");
                return true;
            }

            BundleIsCurrent = false;
            AwaitingRecompile = false;

            // 3) The name has to carry the fingerprint of the source before anything is compiled
            //    against it, and an edit since the last publish means it does not yet. Renaming the
            //    asmdef is a script recompilation and a domain reload, which would tear this task
            //    down mid-await, so the run ends here — but it ends quietly: the caller parks what
            //    it was doing and the reload picks it back up, so the creator sees a pause in the
            //    deploy and one line in the console, nothing to answer and nothing to click again.
            if (!AlignAssemblyNameToSource(out bool renamed))
            {
                // Already logged: the asmdef could not be renamed.
                return false;
            }

            if (renamed)
            {
                AwaitingRecompile = true;
                return false;
            }

            // 4) Build the DLL(s) — a no-op when they already exist under this name, which is
            //    exactly the case where the source has not moved.
            CompileDll(expected);

            BuildTarget[] missing = MissingTargets(expected);
            if (missing.Length > 0)
            {
                // The backend rejects a bundle that misses a target, so failing here — where the
                // reason is still on screen — beats failing at import time.
                Debug.LogError($"[HotUpdateSecurity] No DLL was produced for: {string.Join(", ", missing)}. " +
                               "The build support module for those targets may not be installed in this " +
                               "Editor. Build blocked.");
                return false;
            }

            // 5) Resolve the freshly compiled assembly (ScriptAssemblies → has a PDB for line info).
            string dllPath = HotUpdateDllLocator.ResolveDefaultDllPath(out _);
            if (string.IsNullOrEmpty(dllPath) || !File.Exists(dllPath))
            {
                Debug.LogError("[HotUpdateSecurity] Compiled HotUpdate assembly not found after build. Build blocked.");
                return false;
            }

            // 6) LOCAL check against the policy fetched above. Block on fail.
            VerificationResult local = HotUpdateDllLocator.VerifyAndLog(dllPath, fetch.Policy);
            if (!local.Passed)
            {
                Debug.LogError("[HotUpdateSecurity] LOCAL check FAILED — build blocked. See the violations above.");
                return false;
            }

            // 7) SERVER check (authoritative). Block on rejection OR if it can't complete.
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
