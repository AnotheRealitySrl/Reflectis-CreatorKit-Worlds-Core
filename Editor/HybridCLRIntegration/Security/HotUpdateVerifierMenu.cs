using System.IO;

using UnityEditor;

using UnityEngine;

namespace Virtuademy.CreatorKit.Worlds.Core.HybridCLR.Editor
{
    /// <summary>
    /// Editor entry points to run the static verifier on a compiled hot-update DLL. Each first
    /// downloads the shared whitelist policy (same source as the backend), then verifies and
    /// prints one clickable Console entry per violation, plus the SHA-256.
    /// </summary>
    public static class HotUpdateVerifierMenu
    {
        private const string MenuRoot = "Virtuademy Worlds/Creator Kit/Security/";

        //[MenuItem(MenuRoot + "Verify HotUpdate Script DLL (auto → Console)")]
        public static async void VerifyAuto()
        {
            string path = HotUpdateDllLocator.ResolveDefaultDllPath(out _);
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                Debug.LogWarning("[HotUpdateSecurity] Compiled HotUpdate assembly not found under " +
                                 "Library/ScriptAssemblies. Let Unity compile the scripts first.");
                return;
            }

            HotUpdatePolicy policy = await FetchPolicyOrWarnAsync();
            if (policy != null)
                RunAndReport(path, policy);
        }

        //[MenuItem(MenuRoot + "Verify Hot-Update DLL…")]
        public static async void VerifyPickedDll()
        {
            string path = EditorUtility.OpenFilePanel(
                "Select hot-update DLL to verify", Application.dataPath, "dll,bytes");
            if (string.IsNullOrEmpty(path))
                return;

            HotUpdatePolicy policy = await FetchPolicyOrWarnAsync();
            if (policy != null)
                RunAndReport(path, policy);
        }

        //[MenuItem(MenuRoot + "Verify Built Hot-Update DLLs (all platforms)")]
        public static async void VerifyBuiltDlls()
        {
            string root = Path.Combine(
                Directory.GetParent(Application.dataPath)!.FullName,
                "HybridCLRData", "HotUpdateDlls");

            if (!Directory.Exists(root))
            {
                Debug.LogWarning($"[HotUpdateSecurity] Compile output dir not found: {root}. " +
                                 "Build the hot-update DLLs first (HybridCLR › CompileDll).");
                return;
            }

            HotUpdatePolicy policy = await FetchPolicyOrWarnAsync();
            if (policy == null)
                return;

            bool any = false;
            foreach (string dll in Directory.GetFiles(root, "HotUpdate*.dll", SearchOption.AllDirectories))
            {
                any = true;
                RunAndReport(dll, policy);
            }

            if (!any)
                Debug.LogWarning($"[HotUpdateSecurity] No HotUpdate*.dll found under {root}.");
        }

        private static async System.Threading.Tasks.Task<HotUpdatePolicy> FetchPolicyOrWarnAsync()
        {
            HotUpdatePolicyFetcher.FetchResult fetch = await HotUpdatePolicyFetcher.FetchAsync();
            if (!fetch.Ok)
            {
                Debug.LogError("[HotUpdateSecurity] Policy unavailable — check blocked (fail-closed). " + fetch.Error);
                return null;
            }
            if (fetch.Source == HotUpdatePolicyFetcher.SourceKind.Cached)
                Debug.LogWarning("[HotUpdateSecurity] Using CACHED policy (network fetch failed).");
            return fetch.Policy;
        }

        private static void RunAndReport(string path, HotUpdatePolicy policy)
        {
            byte[] bytes = File.ReadAllBytes(path);
            // VerifyFile (path) lets Cecil load the sibling PDB → violations carry file:line.
            VerificationResult result = HotUpdateAssemblyVerifier.VerifyFile(path, policy);
            string sha = HotUpdateDllLocator.Sha256Hex(bytes);

            // One clickable Console entry per violation (with sha in the summary header).
            HotUpdateDllLocator.LogResult(result, path, sha);
        }
    }
}
