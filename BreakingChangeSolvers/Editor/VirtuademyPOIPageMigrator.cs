using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;

namespace Virtuademy.BreakingChangeSolvers
{
    /// <summary>
    /// Replaces the framework's GenericHookComponent as the page marker of a POI with
    /// <c>POIPagePlaceholder</c>, in every scene and prefab of the project.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A POI finds its pages by a marker component under its "Pages" container. The marker was
    /// GenericHookComponent with id "POIPage", which lives in Virtuademy-SystemCore — a package a
    /// creator project stopped installing when the authoring packages stopped referencing the
    /// framework. From then on the marker was a missing script in a creator project, a world
    /// published from it carried no marker, and every POI in that world came up empty. The
    /// package's own POI prefab now uses <c>POIPagePlaceholder</c>, which fixes every POI that is
    /// an unmodified instance of it; what this pass fixes is everything a creator made
    /// themselves: pages duplicated under "Pages" (the documented way to add one), unpacked
    /// POIs, and copies of the prefab.
    /// </para>
    /// <para>
    /// The rewrite is textual, like the rename pass, because the old script cannot be loaded in
    /// the project that needs the migration. A MonoBehaviour whose script is GenericHookComponent
    /// and whose <c>id</c> is <c>POIPage</c> gets the new script GUID and loses the old fields; its
    /// fileID stays the same, so every reference to the component survives.
    /// </para>
    /// <para>
    /// Other GenericHookComponents are removed <b>only when the script does not resolve</b> —
    /// that is, in a creator project, where they are dead components already and a published
    /// bundle carries nothing of them. The package placed two more of them (the POI and Mirror
    /// "PanTransform" hooks, the Mirror "TeleportTarget"); nothing at runtime ever read either.
    /// A dead hook that anything else in the file points at (an added component of a prefab
    /// instance, for example) is left alone rather than leave a dangling reference. In a project
    /// where the framework is installed nothing is removed, and only page markers are swapped.
    /// </para>
    /// <para>
    /// This has no menu entry of its own: it is part of the v2026.5 -> v2026.6 update routine,
    /// driven by <c>VirtuademyRenameMigrator</c>. Idempotent: a second run finds nothing.
    /// </para>
    /// </remarks>
    public static class VirtuademyPOIPageMigrator
    {
        public class Entry
        {
            public string Path;
            public int Pages;
            public int DeadHooks;
        }

        /// <summary>GenericHookComponent.cs in Virtuademy-SystemCore.</summary>
        public const string HookScriptGuid = "8ac8c34edcf51e141a65eee59c6ead0c";

        /// <summary>POIPagePlaceholder.cs in Virtuademy-SDK-Environments.</summary>
        public const string PageScriptGuid = "c43499aa47f74460903c3907c61043b9";

        private const string PageHookId = "POIPage";

        private static readonly Regex DocumentHeader =
            new(@"^--- !u!(?<class>\d+) &(?<id>-?\d+)(?<rest>[^\r\n]*)\r?$", RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex HookScriptLine =
            new(@"^  m_Script: \{fileID: 11500000, guid: " + HookScriptGuid + @", type: 3\}\r?$", RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex HookIdLine =
            new(@"^  id: (?<value>[^\r\n]*)\r?$", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>The two serialized fields of GenericHookComponent, dropped from a swapped marker.</summary>
        private static readonly Regex HookFieldLines =
            new(@"^  (id|transformRef): [^\r\n]*\r?\n", RegexOptions.Multiline | RegexOptions.Compiled);

        private static readonly Regex EditorClassIdentifierLine =
            new(@"^  m_EditorClassIdentifier: [^\r\n]*\S[^\r\n]*\r?$", RegexOptions.Multiline | RegexOptions.Compiled);

        /// <summary>True when GenericHookComponent cannot be loaded here, i.e. its instances are dead.</summary>
        public static bool HookScriptIsMissing()
            => string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(HookScriptGuid));

        public static List<Entry> Scan()
        {
            bool removeDeadHooks = HookScriptIsMissing();
            List<Entry> entries = new();

            foreach (string path in CandidateFiles())
            {
                string text = ReadYamlOrNull(path);
                if (text == null || !text.Contains(HookScriptGuid))
                {
                    continue;
                }

                Migrate(text, removeDeadHooks, out int pages, out int deadHooks);
                if (pages + deadHooks > 0)
                {
                    entries.Add(new Entry { Path = path.Replace('\\', '/'), Pages = pages, DeadHooks = deadHooks });
                }
            }

            return entries;
        }

        /// <summary>Rewrites the given files in place; returns how many changed. Re-reads each
        /// file, so it is safe to run after another pass has rewritten the same files.</summary>
        public static int Apply(IEnumerable<Entry> entries, out int failed)
        {
            bool removeDeadHooks = HookScriptIsMissing();
            int changed = 0;
            failed = 0;

            foreach (Entry entry in entries)
            {
                try
                {
                    string text = ReadYamlOrNull(entry.Path);
                    if (text == null)
                    {
                        continue;
                    }

                    string migrated = Migrate(text, removeDeadHooks, out _, out _);
                    if (!string.Equals(migrated, text, StringComparison.Ordinal))
                    {
                        File.WriteAllText(entry.Path, migrated, new UTF8Encoding(false));
                        changed++;
                    }
                }
                catch (Exception e)
                {
                    failed++;
                    Debug.LogError($"[POI page migration] Failed to rewrite {entry.Path}: {e}");
                }
            }

            return changed;
        }

        /// <summary>
        /// The whole transformation, on the text of one YAML scene or prefab. Pure, so it can be
        /// checked outside the editor.
        /// </summary>
        public static string Migrate(string text, bool removeDeadHooks, out int pages, out int deadHooks)
        {
            pages = 0;
            deadHooks = 0;

            List<Match> headers = DocumentHeader.Matches(text).Cast<Match>().ToList();
            if (headers.Count == 0)
            {
                return text;
            }

            StringBuilder output = new(text.Length);
            output.Append(text, 0, headers[0].Index);
            List<string> removedIds = new();

            for (int i = 0; i < headers.Count; i++)
            {
                int start = headers[i].Index;
                int end = i + 1 < headers.Count ? headers[i + 1].Index : text.Length;
                string document = text.Substring(start, end - start);
                string fileId = headers[i].Groups["id"].Value;

                bool isHook = headers[i].Groups["class"].Value == "114"
                    && !headers[i].Groups["rest"].Value.Contains("stripped")
                    && HookScriptLine.IsMatch(document);

                if (!isHook)
                {
                    output.Append(document);
                    continue;
                }

                Match id = HookIdLine.Match(document);
                if (id.Success && id.Groups["value"].Value.Trim() == PageHookId)
                {
                    document = HookScriptLine.Replace(document,
                        m => m.Value.Replace(HookScriptGuid, PageScriptGuid));
                    document = HookFieldLines.Replace(document, string.Empty);
                    document = EditorClassIdentifierLine.Replace(document,
                        m => "  m_EditorClassIdentifier: " + (m.Value.EndsWith("\r") ? "\r" : string.Empty));
                    pages++;
                    output.Append(document);
                    continue;
                }

                // A dead hook something else still points at stays, rather than dangle: the only
                // reference a removable one may have is its own GameObject's component list.
                if (removeDeadHooks && OnlyReferencedByItsGameObject(text, fileId))
                {
                    removedIds.Add(fileId);
                    deadHooks++;
                    continue;
                }

                output.Append(document);
            }

            string result = output.ToString();
            foreach (string fileId in removedIds)
            {
                result = Regex.Replace(result,
                    @"^  - (component|114): \{fileID: " + fileId + @"\}\r?\n", string.Empty, RegexOptions.Multiline);
            }

            return result;
        }

        private static bool OnlyReferencedByItsGameObject(string text, string fileId)
        {
            int all = Regex.Matches(text, @"\{fileID: " + fileId + @"[,}]").Count;
            int componentList = Regex.Matches(text,
                @"^  - (component|114): \{fileID: " + fileId + @"\}\r?$", RegexOptions.Multiline).Count;
            return componentList == 1 && all == 1;
        }

        private static IEnumerable<string> CandidateFiles()
            => Directory.EnumerateFiles("Assets", "*", SearchOption.AllDirectories)
                .Where(p => p.EndsWith(".unity", StringComparison.OrdinalIgnoreCase)
                         || p.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase));

        private static string ReadYamlOrNull(string path)
        {
            string text;
            try
            {
                text = File.ReadAllText(path, new UTF8Encoding(false));
            }
            catch (IOException)
            {
                return null;
            }

            return text.StartsWith("%YAML", StringComparison.Ordinal) ? text : null;
        }
    }
}
