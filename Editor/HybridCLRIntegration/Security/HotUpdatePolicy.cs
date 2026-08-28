using System;
using System.Collections.Generic;
using System.Linq;

namespace Virtuademy.CreatorKit.Worlds.Core.HybridCLR.Editor
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

        /// <summary>
        /// Members that are only dangerous when they are handed a NAME or a TYPE: the family of
        /// GetComponent, FindObjectOfType, StartCoroutine and friends, each of which has a generic
        /// overload a creator writes every day and a string or System.Type overload that turns it
        /// into a lookup by name.
        ///
        /// Name alone cannot separate the two — <c>GetComponent&lt;Rigidbody&gt;()</c> and
        /// <c>GetComponent("AuthenticationSystem")</c> are the same member — so the decision is
        /// taken on the call's signature. The generic form carries no parameter and passes; the
        /// dispatching form carries a string or a Type and does not.
        ///
        /// Declaring type full name -&gt; member names.
        /// </summary>
        public Dictionary<string, HashSet<string>> DeniedDispatchMembers = new(StringComparer.Ordinal);

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

        public bool IsDynamicDispatch(string declaringTypeFullName, string memberName,
                                     IEnumerable<string> parameterTypeFullNames)
        {
            if (declaringTypeFullName == null || memberName == null
                || !DeniedDispatchMembers.TryGetValue(declaringTypeFullName, out HashSet<string> dispatchers)
                || !dispatchers.Contains(memberName))
            {
                return false;
            }

            return parameterTypeFullNames != null
                && parameterTypeFullNames.Any(x => x == "System.String" || x == "System.Type");
        }

        public bool IsMemberDenied(string declaringTypeFullName, string memberName)
            => declaringTypeFullName != null && memberName != null && DeniedMembers != null
               && DeniedMembers.TryGetValue(declaringTypeFullName, out HashSet<string> denied)
               && denied != null && denied.Contains(memberName);

        private static bool NamespaceMatches(string ns, string prefix)
            => ns == prefix || ns.StartsWith(prefix + ".", StringComparison.Ordinal);
    }
}
