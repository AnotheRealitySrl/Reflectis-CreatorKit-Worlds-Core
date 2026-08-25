using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json.Linq;

using UnityEditor;
using UnityEditor.Build;

using UnityEngine;

namespace Reflectis.CreatorKit.Worlds.Core.HybridCLR.Editor
{
    /// <summary>
    /// Digest of everything that decides what the hot-update assembly compiles to. It goes into
    /// the assembly's own name, which is what makes the whole publish chain work:
    ///
    ///   * same source => same name => the backend recognises the code it already has and the
    ///     import is a no-op, so nothing is re-uploaded and no second copy is stored;
    ///   * different source => different name => two worlds can sit at two different versions
    ///     and the same player process can hold both, which it could not do if one name meant
    ///     two sets of bytes (an assembly cannot be unloaded from Mono/IL2CPP);
    ///   * and locally, the name is the cache key: if the DLLs for that name are already on
    ///     disk, there is nothing to compile.
    ///
    /// Taken over the SOURCE and never over the compiled bytes. HybridCLR compiles once per build
    /// target and the compiler stamps a PE timestamp, two MVID GUIDs and a debug checksum into
    /// every output, so hashing DLLs would mint a new identity on every rebuild of unchanged code
    /// — exactly what this exists to avoid.
    ///
    /// Deliberately over-inclusive. Every input that could change the emitted IL is in, and a few
    /// that usually do not (the whole packages lock file rather than the versions of the packages
    /// actually referenced). The two failure directions are not symmetric: an input we forgot
    /// means a stale DLL published as new — silent, and wrong at runtime — while an input we
    /// include too eagerly only costs a rebuild nobody needed.
    /// </summary>
    public static class HotUpdateFingerprint
    {
        /// <summary>Characters of the digest that end up in the assembly name.</summary>
        private const int Length = 8;

        /// <summary>
        /// Field separator inside the canonical string. Printable on purpose: a space would be
        /// indistinguishable from one inside a path, and trailing whitespace is the kind of thing
        /// editors and tools quietly strip — which would change every fingerprint at once.
        /// </summary>
        private const char Separator = '|';

        private const string PackagesLock = "Packages/packages-lock.json";

        /// <summary>
        /// Lowercase hex digest of the current source, <see cref="Length"/> characters long.
        /// Returns null when the hot-update folder does not exist yet — the caller is not set up.
        /// </summary>
        public static string Compute(string hotUpdateFolder, IEnumerable<BuildTarget> targets)
        {
            if (string.IsNullOrEmpty(hotUpdateFolder) || !Directory.Exists(hotUpdateFolder))
                return null;

            StringBuilder canonical = new();

            // 1) The code. Sorted by relative path so the digest does not depend on how the file
            //    system happens to enumerate, and hashed per file so a rename shows up as a
            //    change even when the bytes move unchanged.
            foreach (string file in SourceFiles(hotUpdateFolder))
            {
                string relative = file.Substring(hotUpdateFolder.Length).TrimStart('/', '\\').Replace('\\', '/');
                canonical.Append(relative).Append(Separator).Append(TextDigest(file)).Append('\n');
            }

            // 2) The asmdefs, minus their own "name" field — which is where this digest ends up,
            //    so including it would make the input depend on the output. Everything else in
            //    there does change the compilation: references, defineConstraints,
            //    versionDefines, allowUnsafeCode, the platform lists.
            //
            //    The one at the folder root is held to the same rule twice over: its FILE NAME is
            //    HotUpdate_<productGUID>_<this digest>.asmdef, so feeding the path in would make
            //    the fingerprint a function of itself — rename, recompute, rename again, and the
            //    build gate asks for another build forever. Its identity is therefore fixed to a
            //    constant and only its content counts. Nested asmdefs are not renamed by the
            //    setup, so their path stays part of the input.
            foreach (string asmdef in Directory.GetFiles(hotUpdateFolder, "*.asmdef", SearchOption.AllDirectories)
                                               .Select(Normalize)
                                               .OrderBy(x => x, StringComparer.Ordinal))
            {
                string relative = asmdef.Substring(hotUpdateFolder.Length).TrimStart('/', '\\').Replace('\\', '/');
                bool isRootAsmdef = relative.IndexOf('/') < 0;

                canonical.Append(isRootAsmdef ? "<root>" : relative)
                         .Append(Separator).Append(AsmdefDigest(asmdef)).Append('\n');
            }

            // 3) The compiler and what it is told. A Unity upgrade changes the Roslyn behind the
            //    compilation and the API surface it compiles against; the defines decide which
            //    branches of the creator's own code are even emitted, per target.
            canonical.Append("unity").Append(Separator).Append(Application.unityVersion).Append('\n');

            foreach (BuildTarget target in targets ?? Enumerable.Empty<BuildTarget>())
            {
                canonical.Append("defines").Append(Separator).Append(target).Append(Separator)
                         .Append(Defines(target)).Append('\n');
            }

            // 4) The resolved package set. Over-inclusive on purpose: what matters is the version
            //    of every package the assembly references, and the lock file is the one place
            //    that states them all without having to resolve the reference graph by hand.
            if (File.Exists(PackagesLock))
                canonical.Append("packages").Append(Separator).Append(TextDigest(PackagesLock)).Append('\n');

            return Hex(Encoding.UTF8.GetBytes(canonical.ToString())).Substring(0, Length);
        }

        /// <summary>Every C# file that compiles into the hot-update assembly or one nested in it.</summary>
        private static IEnumerable<string> SourceFiles(string hotUpdateFolder)
        {
            return Directory.GetFiles(hotUpdateFolder, "*.cs", SearchOption.AllDirectories)
                            .Select(Normalize)
                            .OrderBy(x => x, StringComparer.Ordinal);
        }

        private static string AsmdefDigest(string path)
        {
            try
            {
                JObject asmdef = JObject.Parse(File.ReadAllText(path));
                asmdef.Remove("name");
                return Hex(Encoding.UTF8.GetBytes(asmdef.ToString(Newtonsoft.Json.Formatting.None)));
            }
            catch (Exception)
            {
                // Unparseable: fall back to the raw bytes. A malformed asmdef will fail the
                // compilation anyway, and a digest that changes with it is still honest.
                return TextDigest(path);
            }
        }

        /// <summary>
        /// Digest of a text file with line endings normalised, so a checkout that brings CRLF does
        /// not read as a code change on a machine that had LF.
        /// </summary>
        private static string TextDigest(string path)
        {
            try
            {
                string text = File.ReadAllText(path).Replace("\r\n", "\n").Replace("\r", "\n");
                return Hex(Encoding.UTF8.GetBytes(text));
            }
            catch (Exception e)
            {
                // An unreadable file must not silently drop out of the digest: fold the error in,
                // so the fingerprint changes and nothing is reused on a guess.
                return Hex(Encoding.UTF8.GetBytes("unreadable:" + path + ":" + e.GetType().Name));
            }
        }

        private static string Defines(BuildTarget target)
        {
            try
            {
                NamedBuildTarget named = NamedBuildTarget.FromBuildTargetGroup(
                    BuildPipeline.GetBuildTargetGroup(target));

                string[] defines = PlayerSettings.GetScriptingDefineSymbols(named)
                    .Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .OrderBy(x => x, StringComparer.Ordinal)
                    .ToArray();

                // Sorted: the define list is a set, and the editor does not promise an order.
                return string.Join(";", defines);
            }
            catch (Exception e)
            {
                return "unavailable:" + e.GetType().Name;
            }
        }

        private static string Normalize(string path) => path.Replace('\\', '/');

        private static string Hex(byte[] data)
        {
            // SHA256.Create/ComputeHash and not SHA256.HashData: the editor assemblies are built
            // against the .NET Framework 4.8 profile, where the .NET 5+ static does not exist.
            using SHA256 sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(data);

            StringBuilder sb = new(hash.Length * 2);
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));

            return sb.ToString();
        }
    }
}
