using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HybridCLR.Editor.Settings;

using UnityEditor;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.HybridCLR.Editor
{
    /// <summary>
    /// Resolves the project's hot-update assembly automatically (no manual browsing). The
    /// authoritative source is <c>HybridCLRSettings.hotUpdateAssemblyDefinitions</c> — the
    /// exact asmdef(s) HybridCLR compiles — so we match whatever name the setupper produced
    /// (e.g. <c>HotUpdate_&lt;productGUID&gt;</c>), NOT a guessed "HotUpdate".
    /// </summary>
    internal static class HotUpdateDllLocator
    {
        [Serializable] private class AsmdefName { public string name; }

        public static string ProjectRoot => Directory.GetParent(Application.dataPath)!.FullName;

        public static string ScriptAssembliesDll(string assemblyName)
            => Path.Combine(ProjectRoot, "Library", "ScriptAssemblies", assemblyName + ".dll");

        public static string HotUpdateDll(string assemblyName, string target)
            => Path.Combine(ProjectRoot, "HybridCLRData", "HotUpdateDlls", target, assemblyName + ".dll");

        private static IEnumerable<string> RegisteredHotUpdateNames()
        {
            HybridCLRSettings settings = null;
            try { settings = HybridCLRSettings.Instance; } catch { /* HybridCLR not ready */ }

            var defs = settings?.hotUpdateAssemblyDefinitions;
            if (defs == null)
                yield break;

            List<string> others = new();
            foreach (UnityEditorInternal.AssemblyDefinitionAsset def in defs)
            {
                foreach (string name in NamesOf(def))
                {
                    if (name.StartsWith("HotUpdate", StringComparison.OrdinalIgnoreCase))
                        yield return name;
                    else
                        others.Add(name);
                }
            }
            foreach (string name in others)
                yield return name;
        }

        private static IEnumerable<string> NamesOf(UnityEditorInternal.AssemblyDefinitionAsset def)
        {
            if (def == null)
                yield break;

            string jsonName = null;
            try { jsonName = JsonUtility.FromJson<AsmdefName>(def.text)?.name; } catch { }
            if (!string.IsNullOrEmpty(jsonName))
                yield return jsonName;

            if (!string.IsNullOrEmpty(def.name) && def.name != jsonName)
                yield return def.name;
        }

        public static string ResolveAssemblyName()
            => RegisteredHotUpdateNames().FirstOrDefault();

        public static string ResolveDefaultDllPath(out string assemblyName)
        {
            string saDir = Path.Combine(ProjectRoot, "Library", "ScriptAssemblies");

            foreach (string name in RegisteredHotUpdateNames())
            {
                string dll = ScriptAssembliesDll(name);
                if (File.Exists(dll)) { assemblyName = name; return dll; }
            }

            if (Directory.Exists(saDir))
            {
                string newest = Directory.GetFiles(saDir, "HotUpdate*.dll")
                    .OrderByDescending(File.GetLastWriteTimeUtc)
                    .FirstOrDefault();
                if (newest != null) { assemblyName = Path.GetFileNameWithoutExtension(newest); return newest; }
            }

            assemblyName = null;
            return null;
        }

        public static string ResolveTargetDllPath(string target, out string assemblyName)
        {
            string dir = Path.Combine(ProjectRoot, "HybridCLRData", "HotUpdateDlls", target);

            foreach (string name in RegisteredHotUpdateNames())
            {
                string dll = HotUpdateDll(name, target);
                if (File.Exists(dll)) { assemblyName = name; return dll; }
            }

            if (Directory.Exists(dir))
            {
                string newest = Directory.GetFiles(dir, "HotUpdate*.dll")
                    .OrderByDescending(File.GetLastWriteTimeUtc)
                    .FirstOrDefault();
                if (newest != null) { assemblyName = Path.GetFileNameWithoutExtension(newest); return newest; }
            }

            assemblyName = ResolveAssemblyName();
            return string.IsNullOrEmpty(assemblyName) ? null : HotUpdateDll(assemblyName, target);
        }

        /// <summary>SHA-256 of the bytes as lowercase hex (informational, shown in the log).</summary>
        public static string Sha256Hex(byte[] data)
        {
            using System.Security.Cryptography.SHA256 sha = System.Security.Cryptography.SHA256.Create();
            byte[] hash = sha.ComputeHash(data);
            System.Text.StringBuilder sb = new(hash.Length * 2);
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>Runs the verifier against the given policy and writes the outcome to the
        /// Unity Console.</summary>
        public static VerificationResult VerifyAndLog(string dllPath, HotUpdatePolicy policy)
        {
            VerificationResult result = HotUpdateAssemblyVerifier.VerifyFile(dllPath, policy);
            LogResult(result, dllPath);
            return result;
        }

        /// <summary>
        /// Writes the result to the Console as ONE entry per violation. Each entry embeds a
        /// console hyperlink to the offending <c>file:line</c> (when a PDB is present), so
        /// clicking it opens the script at that line.
        /// </summary>
        public static void LogResult(VerificationResult result, string dllPath, string sha256 = null)
        {
            string name = Path.GetFileName(dllPath);
            string shaLine = string.IsNullOrEmpty(sha256) ? string.Empty : $"\n  sha256: {sha256}";

            if (result.Passed)
            {
                Debug.Log($"[HotUpdateSecurity] {name} — PASSED (no policy violations).\n  path: {dllPath}{shaLine}");
                return;
            }

            Debug.LogError($"[HotUpdateSecurity] {name} — REJECTED: {result.Violations.Count} " +
                           $"policy violation(s) (see entries below).\n  path: {dllPath}{shaLine}");

            foreach (Violation v in result.Violations)
                Debug.LogError(FormatViolation(v));
        }

        private static string FormatViolation(Violation v)
        {
            string rel = v.SourceFile != null ? ToProjectRelative(v.SourceFile) : null;

            string where = (rel != null && v.SourceLine.HasValue)
                ? $"<a href=\"{rel}\" line=\"{v.SourceLine.Value}\">{rel}:{v.SourceLine.Value}</a>"
                : v.Where;

            return $"[HotUpdateSecurity] [{v.Kind}] {v.Detail}\n  → {where}";
        }

        private static string ToProjectRelative(string absolute)
        {
            try
            {
                string full = Path.GetFullPath(absolute).Replace('\\', '/');
                string root = ProjectRoot.Replace('\\', '/').TrimEnd('/') + "/";
                if (full.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                    return full.Substring(root.Length);

                int idx = full.IndexOf("/Assets/", StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                    return full.Substring(idx + 1);
            }
            catch { /* unusable path */ }
            return null;
        }
    }
}
