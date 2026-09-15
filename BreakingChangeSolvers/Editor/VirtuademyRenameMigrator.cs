using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace Virtuademy.SDK.Environments.Installer.Editor
{
    /// <summary>
    /// Project-wide migration for the two renames the packages have been through:
    /// the Reflectis -> Virtuademy brand rename, and the authoring package becoming
    /// Virtuademy-SDK-Environments (namespaces, assembly names, package ids).
    ///
    /// One pass covers both, in that order, so a project can arrive from either side.
    ///
    /// MonoBehaviour references survive the rename on their own (they resolve by GUID), but
    /// every reference stored BY NAME does not: [SerializeReference] payloads in scenes,
    /// prefabs and ScriptableObjects; Visual Scripting graphs (node types are serialized as
    /// fully-qualified type + assembly strings); UXML custom-control tags; the creator's own
    /// C# scripts (using directives) and asmdef references; the UPM manifest package ids.
    /// This tool rewrites all of those as raw text, BEFORE Unity tries to resolve the old
    /// names, then refreshes the AssetDatabase and rebuilds the Visual Scripting node library.
    ///
    /// Recommended flow for a creator project:
    ///   1. Commit / back up the project (the rewrite touches many files).
    ///   2. Update the SDK / Creator Kit packages to the renamed (Virtuademy) versions.
    ///   3. Run this routine, review the file list, Apply.
    ///   4. Let Unity recompile and reimport, then re-save any still-dirty scenes.
    ///
    /// Expect a wall of Visual Scripting deserialization errors on the FIRST open after this
    /// runs, and do not save anything until they stop. A graph records each unit by namespace and
    /// type, so the rewrite changes what every unit is called; while an asset-import worker
    /// cannot resolve the new name yet, Visual Scripting swaps the unit for
    /// <c>Unity.VisualScripting.MissingType</c> and keeps the original in `formerType` /
    /// `formerValue`. That swap renumbers the JSON `$id`s, and FullSerializer requires a
    /// definition to precede its reference, so the visible error is
    /// "Object definition has not been encountered for object with id=N ... have you reordered or
    /// modified the serialized data?" — alarming, and a consequence of the substitution rather
    /// than of damaged data. Most of them clear on their own, and the log then says
    /// "Missing unit type ... was found. Converted ... back".
    ///
    /// **Some do not clear by reopening**, because the failure is cached in the asset's imported
    /// artifact rather than in the asset. Observed on this repo: one prefab kept failing on a
    /// second open, in the main process while the window layout was being restored, with no
    /// recovery message at all. What fixes that one is a **Reimport** on the asset (right-click in
    /// the Project window), which deserializes it afresh with the assemblies loaded. Reopening the
    /// editor does not.
    ///
    /// So the order is: reimport, then OPEN the graph and check the units are real nodes and not
    /// "Missing Type", and only then save. **Saving first is the one way to make the loss
    /// permanent**: the units get written out as `MissingType` and the graph really has lost them.
    /// The asset itself is untouched until that save, so there is no hurry.
    ///
    /// **Reimport every affected asset, not one.** Each asset caches its own import artifact, so
    /// clearing one says nothing about the others — and the errors arrive a few at a time, as
    /// whatever is loaded happens to touch them, which makes it easy to believe the last reimport
    /// fixed the problem. It did not; it fixed that asset. Find them all before deciding you are
    /// done: the editor log names the missing type in a `formerType` entry, and grepping the
    /// project for that type name lists every asset that records it.
    ///
    /// The tool is idempotent: a second run finds nothing to change.
    /// </summary>
    public class VirtuademyRenameMigrator : EditorWindow
    {
        private class Entry
        {
            public string Path;
            public int Hits;
            public bool Selected = true;
        }

        // The old brand token is split so this file never matches its own patterns
        // (neither when the repo-side rename scripts run, nor when the tool scans itself
        // in a project where packages are embedded).
        private static readonly string OldBrand = "Reflec" + "tis";
        private const string NewBrand = "Virtuademy";
        private const string WindowTitle = "Package rename migrator";

        // Ordered: specific mappings first, then the generic namespace rule.
        private static readonly (string oldValue, string newValue)[] LiteralMap =
        {
            (OldBrand + ".SDK." + OldBrand + "Api", NewBrand + ".SDK.PlatformApi"),
            (OldBrand.ToLowerInvariant() + "-sdk-" + OldBrand.ToLowerInvariant() + "api", NewBrand.ToLowerInvariant() + "-sdk-platformapi"),
            (OldBrand + "-SDK-" + OldBrand + "Api", NewBrand + "-SDK-PlatformApi"),
            (OldBrand + ".SDK." + OldBrand + "BrowserCommunication", NewBrand + ".SDK.BrowserCommunication"),
            ("com.anotherealitysrl." + OldBrand.ToLowerInvariant() + "-", "com.anotherealitysrl." + NewBrand.ToLowerInvariant() + "-"),
            (OldBrand + "-SDK-", NewBrand + "-SDK-"),
            (OldBrand + "-CreatorKit-", NewBrand + "-CreatorKit-"),
            (OldBrand + "-MinigamesTemplate", NewBrand + "-MinigamesTemplate"),
            (OldBrand + "-PLG-", NewBrand + "-PLG-"),
        };

        private static readonly Regex GenericNamespaceRule =
            new(@"(?<![A-Za-z0-9_])" + OldBrand + @"\.", RegexOptions.Compiled);
        private static readonly Regex EditorNamespaceRule =
            new(@"(?<![A-Za-z0-9_])" + OldBrand + @"Editor\.", RegexOptions.Compiled);

        // The Environments rename (2026-09-09): the authoring package stopped being
        // "CreatorKit Worlds Core" and became "SDK Environments" — package id, three assembly
        // names and every namespace under the old prefix.
        //
        // Applied AFTER the brand rules, which is what lets one pass serve both hops: a project
        // still on the old brand has its old-brand authoring namespace turned into the
        // new-brand one by the generic rule above and is then caught here, while a project that
        // already took the brand rename is caught directly. Ordered longest-first, because the
        // runtime assembly's name is a prefix of the editor one's and replacing the short one
        // first would glue "Editor" onto the new name.
        //
        // The tokens are split for the same reason OldBrand is: this file must not match its own
        // table when the tool scans the project it is running in.
        private static readonly string OldWorlds = "Virtuademy.Creator" + "Kit.Worlds";
        private static readonly string OldWorldsPackage = "Virtuademy-Creator" + "Kit-Worlds-Core";
        private static readonly string OldWorldsId = "virtuademy-creator" + "kit-worlds-core";

        private static readonly string OldApi = "Virtuademy.SDK.Platform" + "Api";

        private static readonly string OldModels = "Virtuademy.SDK.Environments.Client" + "Models";
        private static readonly string OldInteraction = "Virtuademy.SDK.Environments.Inter" + "action";
        private static readonly string OldPlaceholders = "Virtuademy.SDK.Environments.Place" + "holders";
        private static readonly string OldSpawner = "Virtuademy.SDK.Environments.Object" + "Spawner";
        private static readonly string OldDialogs = "Virtuademy.SDK.Dia" + "logs";
        private static readonly string OldGraphs = "Virtuademy.SDK.Gra" + "phs";
        private static readonly string OldTasks = "Virtuademy.SDK.Ta" + "sks";
        private static readonly string OldScripting = "Virtuademy.Scripting" + "Api";

        private static readonly (string oldValue, string newValue)[] EnvironmentsMap =
        {
            (OldWorlds + ".CoreHybridCLREditor", "Virtuademy.SDK.Environments.HybridCLREditor"),
            (OldWorlds + ".CoreEditor", "Virtuademy.SDK.Environments.Editor"),
            (OldWorlds + ".Core", "Virtuademy.SDK.Environments"),
            (OldWorlds, "Virtuademy.SDK.Environments"),
            (OldWorldsPackage, "Virtuademy-SDK-Environments"),
            (OldWorldsId, "virtuademy-sdk-environments"),

            // PlatformApi named a package that no longer exists: it became
            // Virtuademy-SDK-Library on 2026-09-10, and the DTOs it was named after moved to the
            // contracts package on the same day. The namespace outlived both.
            //
            // The assembly entry has to come first: the old namespace is a prefix of the old
            // assembly name, so rewriting the shorter one first would land the assembly on the
            // right value only by luck of the substring — and would do the wrong thing the
            // moment the two stop sharing a prefix. Neither is spelled out here, for the same
            // reason the tokens above are split: this file must not match its own table.
            (OldApi + ".Wire", "Virtuademy.SDK.ApiData.Wire"),
            (OldApi, "Virtuademy.SDK.ApiData"),

            // The client models split in two on 2026-09-14. What an authored world may see is
            // now a view in Virtuademy.ScriptingApi; the fuller model kept the CM name and the
            // ClientModels namespace and went to the application, which a creator does not
            // install. A graph that reached a CM type through Expose or InvokeMember recorded
            // its full name, so those names are rewritten onto the view.
            //
            // Member names are deliberately untouched: the views carry the same ones the nodes
            // always read, so a rewritten graph resolves its members without a second rule. A
            // graph that reached past that surface — a user's preferences, a session's
            // permissions — has no view to land on and must be re-authored; there is nothing to
            // migrate it to.
            //
            // The namespace alone is NOT in this table, and must not be: it still exists, on
            // the application side. Only these five full names move.
            (OldModels + ".CMUser", "Virtuademy.ScriptingApi.UserView"),
            (OldModels + ".CMSession", "Virtuademy.ScriptingApi.SessionView"),
            (OldModels + ".CMEnvironment", "Virtuademy.ScriptingApi.EnvironmentView"),
            (OldModels + ".CMExperience", "Virtuademy.ScriptingApi.ExperienceView"),
            (OldModels + ".CMTag", "Virtuademy.ScriptingApi.TagView"),

            // The interaction contracts moved on 2026-09-14 for the same reason the models did,
            // from the other direction: they had to become nameable. Virtuademy.SDK is a denied
            // namespace prefix in the script whitelist, and a deny by prefix beats every allow,
            // so a member taking one of these was a member no interpreted script could call.
            //
            // As above, the namespace itself is NOT in this table: four more types stay behind
            // in it. Only these three names move.
            // These were three full type names while four more types stayed in the namespace. On
            // 2026-09-14 the rest followed, along with every placeholder and the spawner
            // contracts, so the three namespaces are empty and move whole — one entry each
            // instead of a hundred type names, and a graph that named any of them is rewritten
            // whether or not anyone thought to list it.
            //
            // What made this safe is that the namespaces are empty *everywhere*, the platform's
            // own code included. That was not true of the earlier moves, which is why those are
            // still spelled out one type at a time above.
            (OldInteraction, "Virtuademy.Environments.ScriptingApi.Interaction"),
            (OldPlaceholders, "Virtuademy.Environments.ScriptingApi.Placeholders"),
            (OldSpawner, "Virtuademy.Environments.ScriptingApi.ObjectSpawner"),

            // The dialog engine and the graph structure are not the platform's, and moving them
            // into its surface would have said they were. They were renamed instead, which takes
            // them out of a denied prefix without coupling two reusable packages to anything:
            // SPACS.Dialogs and SPACS.Graphs, beside SPACS.Utility.
            //
            // Whole namespaces again, and for the same reason as the three above: nothing is left
            // behind in either.
            (OldDialogs, "SPACS.Dialogs"),
            (OldGraphs, "SPACS.Graphs"),
            (OldTasks, "SPACS.Tasks"),

            // One rule covers the tasks package because the mapping is uniform: SPACS.Tasks,
            // .Detectors, .UI, .Utils, .XRDetectors, and TasksNetworked / TasksXRKit all fall out
            // of the same substring. The assembly names land right too, which is what the asmdef
            // references and the `asm:` fields of a SerializeReference need.
            //
            // One thing it deliberately does not cover: the editor namespace is SPACS.TasksEditor,
            // outside the SPACS.Tasks prefix so that whitelisting the prefix cannot reach editor
            // types, while the editor *assembly* stays SPACS.Tasks.Editor. A substring rule cannot
            // tell those two apart. Rewriting `Virtuademy.SDK.Tasks.Editor` to SPACS.Tasks.Editor
            // is right for an asmdef reference and wrong for a `using`, so a creator who wrote
            // editor code against our editor namespace gets a compile error naming the type rather
            // than a silent miss. That is the safe half of the trade, and it is the same shape the
            // Dialogs entry above already has.

            // Not the ChatBot namespace: Virtuademy.SDK.Core.ChatBot still holds IChatBotSystem,
            // in the framework package, and only this one type left it.
            ("Virtuademy.SDK.Core.ChatBot.EChatBotVoice",
             "Virtuademy.Environments.ScriptingApi.ChatBot.EChatBotVoice"),

            // Four groups crossed from the shared surface to the world one on 2026-09-14. The line
            // had been drawn as "what an external application also needs", which nothing enforced
            // and nothing could: Install is internal to the platform application's own assembly,
            // so no other host can put an implementation behind either interface. Redrawn as what
            // only the platform knows against what the player provides, these four are the
            // player's — the screen it draws, the panels over it, the language, the device.
            //
            // As above, the namespace itself is NOT in this table: the views and the analytic
            // statements stay in it. Only these four names move.
            (OldScripting + ".ILocalizationApi",
             "Virtuademy.Environments.ScriptingApi.ILocalizationApi"),
            (OldScripting + ".IPlatformApi",
             "Virtuademy.Environments.ScriptingApi.IPlatformApi"),
            (OldScripting + ".IScreenApi",
             "Virtuademy.Environments.ScriptingApi.IScreenApi"),
            (OldScripting + ".IHelpApi",
             "Virtuademy.Environments.ScriptingApi.IHelpApi"),
        };

        private static readonly string[] TextExtensions =
        {
            ".cs", ".asmdef", ".asmref", ".json", ".uxml", ".uss", ".tss",
            ".unity", ".prefab", ".asset", ".md", ".txt",
        };

        private static readonly string[] YamlExtensions = { ".unity", ".prefab", ".asset" };

        private readonly List<Entry> entries = new();
        private Vector2 scrollPosition;
        private bool hasScanned;
        private bool deleteLockFile = true;

        [MenuItem("Virtuademy/Update routines/Package rename migration")]
        public static void Open()
        {
            VirtuademyRenameMigrator window = GetWindow<VirtuademyRenameMigrator>(false, WindowTitle, true);
            window.minSize = new Vector2(560, 320);
            window.Show();
            window.ScanProject();
        }

        #region GUI

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                if (GUILayout.Button("Rescan project", EditorStyles.toolbarButton, GUILayout.Width(110)))
                {
                    ScanProject();
                }

                GUILayout.FlexibleSpace();

                using (new EditorGUI.DisabledScope(entries.Count == 0))
                {
                    if (GUILayout.Button("Select all", EditorStyles.toolbarButton, GUILayout.Width(70)))
                    {
                        entries.ForEach(e => e.Selected = true);
                    }
                    if (GUILayout.Button("Select none", EditorStyles.toolbarButton, GUILayout.Width(80)))
                    {
                        entries.ForEach(e => e.Selected = false);
                    }
                }
            }

            if (!hasScanned)
            {
                EditorGUILayout.HelpBox("Press \"Rescan project\" to search for outdated package references.", MessageType.Info);
                return;
            }

            if (entries.Count == 0)
            {
                EditorGUILayout.HelpBox("No outdated reference found. The project is already migrated.", MessageType.Info);
                return;
            }

            EditorGUILayout.Space(4);
            EditorGUILayout.LabelField(
                $"{entries.Count} file(s), {entries.Sum(e => e.Hits)} occurrence(s). " +
                "Review the list: exclude files whose matches are narrative content rather than type references.",
                EditorStyles.miniLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            foreach (Entry entry in entries)
            {
                using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    entry.Selected = EditorGUILayout.Toggle(entry.Selected, GUILayout.Width(18));
                    EditorGUILayout.LabelField(entry.Path, EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"{entry.Hits}", GUILayout.Width(40));
                    if (GUILayout.Button("Ping", GUILayout.Width(44)))
                    {
                        UnityEngine.Object asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(entry.Path);
                        if (asset != null)
                        {
                            EditorGUIUtility.PingObject(asset);
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.Space(4);
            deleteLockFile = EditorGUILayout.ToggleLeft(
                "Delete Packages/packages-lock.json so UPM re-resolves the renamed package ids (recommended)",
                deleteLockFile);

            EditorGUILayout.HelpBox(
                "Files are rewritten in place. Make sure the project is committed to version control " +
                "(or backed up) before applying.",
                MessageType.Warning);

            int selectedCount = entries.Count(e => e.Selected);
            using (new EditorGUI.DisabledScope(selectedCount == 0))
            {
                if (GUILayout.Button($"Apply rename to {selectedCount} file(s)", GUILayout.Height(30)))
                {
                    ApplySelected();
                }
            }
            EditorGUILayout.Space(4);
        }

        #endregion

        #region Scan / apply

        private void ScanProject()
        {
            entries.Clear();

            try
            {
                int index = 0;
                List<string> candidates = CandidateFiles().ToList();
                foreach (string path in candidates)
                {
                    EditorUtility.DisplayProgressBar(WindowTitle, path, (float)index++ / candidates.Count);

                    string text = ReadTextOrNull(path);
                    if (text == null)
                    {
                        continue;
                    }

                    int hits = CountHits(text);
                    if (hits > 0)
                    {
                        entries.Add(new Entry { Path = path.Replace('\\', '/'), Hits = hits });
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            hasScanned = true;
            Repaint();
        }

        private void ApplySelected()
        {
            List<Entry> selected = entries.Where(e => e.Selected).ToList();

            if (!EditorUtility.DisplayDialog(
                    WindowTitle,
                    $"Rewrite {selected.Count} file(s) in place ({selected.Sum(e => e.Hits)} occurrence(s))?\n\n" +
                    "Make sure the project is committed / backed up first.",
                    "Apply", "Cancel"))
            {
                return;
            }

            int changedFiles = 0, failed = 0;

            try
            {
                for (int i = 0; i < selected.Count; i++)
                {
                    Entry entry = selected[i];
                    EditorUtility.DisplayProgressBar(WindowTitle, entry.Path, (float)i / selected.Count);

                    try
                    {
                        string text = ReadTextOrNull(entry.Path);
                        if (text == null)
                        {
                            continue;
                        }

                        string rewritten = Rewrite(text);
                        if (!string.Equals(rewritten, text, StringComparison.Ordinal))
                        {
                            File.WriteAllText(entry.Path, rewritten, new UTF8Encoding(HasUtf8Bom(entry.Path)));
                            changedFiles++;
                        }
                    }
                    catch (Exception e)
                    {
                        failed++;
                        Debug.LogError($"[{WindowTitle}] Failed to rewrite {entry.Path}: {e}");
                    }
                }

                if (deleteLockFile && File.Exists("Packages/packages-lock.json"))
                {
                    File.Delete("Packages/packages-lock.json");
                    Debug.Log($"[{WindowTitle}] Deleted Packages/packages-lock.json (will be regenerated by UPM).");
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            RebuildVisualScriptingUnits();

            EditorUtility.DisplayDialog(
                WindowTitle,
                $"Done.\n\nRewritten: {changedFiles}\nFailed: {failed}" +
                (failed > 0 ? "\n\nSee the Console for details." : string.Empty) +
                "\n\nUnity will now recompile. Afterwards, open your world scenes once and re-save them " +
                "so the migrated data is reserialized.",
                "OK");

            ScanProject();
        }

        private static IEnumerable<string> CandidateFiles()
        {
            foreach (string path in Directory.EnumerateFiles("Assets", "*", SearchOption.AllDirectories))
            {
                string extension = Path.GetExtension(path).ToLowerInvariant();
                if (TextExtensions.Contains(extension))
                {
                    yield return path;
                }
            }

            // The UPM manifest carries the package ids. (The lock file is deleted instead.)
            if (File.Exists("Packages/manifest.json"))
            {
                yield return "Packages/manifest.json";
            }
        }

        /// <summary>Reads the file as text; returns null for binary content (NUL bytes,
        /// or a scene/prefab/asset that is not text-serialized YAML).</summary>
        private static string ReadTextOrNull(string path)
        {
            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(path);
            }
            catch (IOException)
            {
                return null;
            }

            int probe = Math.Min(bytes.Length, 8000);
            for (int i = 0; i < probe; i++)
            {
                if (bytes[i] == 0)
                {
                    return null;
                }
            }

            string text = new UTF8Encoding(false).GetString(bytes);
            if (text.Length > 0 && text[0] == '\uFEFF')
            {
                text = text.Substring(1);
            }

            string extension = Path.GetExtension(path).ToLowerInvariant();
            if (YamlExtensions.Contains(extension) && !text.StartsWith("%YAML", StringComparison.Ordinal))
            {
                return null;
            }

            return text;
        }

        private static bool HasUtf8Bom(string path)
        {
            using FileStream stream = File.OpenRead(path);
            return stream.Length >= 3 && stream.ReadByte() == 0xEF && stream.ReadByte() == 0xBB && stream.ReadByte() == 0xBF;
        }

        private static int CountHits(string text)
        {
            // Mirror Apply exactly: literal replacements first, then count what the generic
            // rules would still match on the intermediate text (avoids double counting).
            int hits = 0;
            foreach ((string oldValue, string newValue) in LiteralMap)
            {
                int index = 0;
                while ((index = text.IndexOf(oldValue, index, StringComparison.Ordinal)) >= 0)
                {
                    hits++;
                    index += oldValue.Length;
                }

                text = text.Replace(oldValue, newValue);
            }

            hits += GenericNamespaceRule.Matches(text).Count;
            text = GenericNamespaceRule.Replace(text, NewBrand + ".");
            hits += EditorNamespaceRule.Matches(text).Count;
            text = EditorNamespaceRule.Replace(text, NewBrand + "Editor.");

            foreach ((string oldValue, string newValue) in EnvironmentsMap)
            {
                int index = 0;
                while ((index = text.IndexOf(oldValue, index, StringComparison.Ordinal)) >= 0)
                {
                    hits++;
                    index += oldValue.Length;
                }

                text = text.Replace(oldValue, newValue);
            }

            return hits;
        }

        private static string Rewrite(string text)
        {
            foreach ((string oldValue, string newValue) in LiteralMap)
            {
                text = text.Replace(oldValue, newValue);
            }

            text = GenericNamespaceRule.Replace(text, NewBrand + ".");
            text = EditorNamespaceRule.Replace(text, NewBrand + "Editor.");

            foreach ((string oldValue, string newValue) in EnvironmentsMap)
            {
                text = text.Replace(oldValue, newValue);
            }

            return text;
        }

        /// <summary>Rebuilds the Visual Scripting node library so the renamed unit types are
        /// picked up. Done via reflection so this assembly does not depend on Visual Scripting.</summary>
        private static void RebuildVisualScriptingUnits()
        {
            try
            {
                Type unitBase = AppDomain.CurrentDomain.GetAssemblies()
                    .Select(a => a.GetType("Unity.VisualScripting.UnitBase"))
                    .FirstOrDefault(t => t != null);

                unitBase?.GetMethod("Rebuild", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
                    ?.Invoke(null, null);

                if (unitBase != null)
                {
                    Debug.Log($"[{WindowTitle}] Visual Scripting node library rebuilt.");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[{WindowTitle}] Could not rebuild the Visual Scripting node library automatically " +
                                 $"({e.Message}). Run it manually: Edit > Project Settings > Visual Scripting > Regenerate Nodes.");
            }
        }

        #endregion
    }
}
