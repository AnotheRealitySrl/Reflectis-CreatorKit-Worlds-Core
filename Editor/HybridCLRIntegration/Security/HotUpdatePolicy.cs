using System;
using System.Collections.Generic;
using System.Linq;

namespace Reflectis.CreatorKit.Worlds.Core.HybridCLR.Editor
{
    /// <summary>
    /// Data-driven whitelist policy, deserialized from the platform's policy.json (fetched
    /// over the public GET). This is the SAME data the backend verifier uses — the client
    /// never hardcodes the lists. Only this small evaluation logic is mirrored on both sides.
    ///
    /// WHITELIST, fail-closed. Precedence: deny-type &gt; allow-type &gt; deny-namespace &gt; allow-namespace.
    /// </summary>
    public sealed class HotUpdatePolicy
    {
        public HashSet<string> AllowedAssemblies = new(StringComparer.Ordinal);
        public HashSet<string> AllowedExactNamespaces = new(StringComparer.Ordinal);
        public List<string> AllowedNamespacePrefixes = new();
        public HashSet<string> AllowedTypeFullNames = new(StringComparer.Ordinal);
        public List<string> DeniedNamespacePrefixes = new();
        public HashSet<string> DeniedTypeFullNames = new(StringComparer.Ordinal);
        public Dictionary<string, HashSet<string>> DeniedMembers = new(StringComparer.Ordinal);

        public bool IsAssemblyAllowed(string assemblySimpleName)
            => assemblySimpleName != null && AllowedAssemblies != null
               && AllowedAssemblies.Contains(assemblySimpleName);

        public bool IsTypeAllowed(string typeNamespace, string typeFullName)
        {
            typeNamespace ??= string.Empty;

            if (typeFullName != null && DeniedTypeFullNames != null && DeniedTypeFullNames.Contains(typeFullName))
                return false;
            if (typeFullName != null && AllowedTypeFullNames != null && AllowedTypeFullNames.Contains(typeFullName))
                return true;
            if (DeniedNamespacePrefixes != null && DeniedNamespacePrefixes.Any(p => NamespaceMatches(typeNamespace, p)))
                return false;
            if (typeNamespace.Length == 0)
                return true;
            if (AllowedExactNamespaces != null && AllowedExactNamespaces.Contains(typeNamespace))
                return true;
            return AllowedNamespacePrefixes != null && AllowedNamespacePrefixes.Any(p => NamespaceMatches(typeNamespace, p));
        }

        public bool IsMemberDenied(string declaringTypeFullName, string memberName)
            => declaringTypeFullName != null && memberName != null && DeniedMembers != null
               && DeniedMembers.TryGetValue(declaringTypeFullName, out HashSet<string> denied)
               && denied != null && denied.Contains(memberName);

        private static bool NamespaceMatches(string ns, string prefix)
            => ns == prefix || ns.StartsWith(prefix + ".", StringComparison.Ordinal);
    }
}
