using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Virtuademy.CreatorKit.Worlds.Installer.Editor
{
    /// <summary>
    /// Project-wide migration for the Reflectis -> Virtuademy brand rename of the SDK and
    /// Creator Kit packages (namespaces, assembly names, package ids).
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
        private const string WindowTitle = "Virtuademy rename migrator";

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

        [MenuItem("Virtuademy Worlds/Creator Kit update routines/Reflectis -> Virtuademy rename")]
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
                EditorGUILayout.HelpBox("Press \"Rescan project\" to search for old-brand references.", MessageType.Info);
                return;
            }

            if (entries.Count == 0)
            {
                EditorGUILayout.HelpBox("No old-brand reference found. The project is already migrated.", MessageType.Info);
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
            hits += EditorNamespaceRule.Matches(text).Count;
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
