using System.Collections.Generic;

namespace Virtuademy
{
    /// <summary>
    /// Records which legacy catalog variables were resolved while the current catalog loaded.
    /// </summary>
    /// <remarks>
    /// Addressables resolves a variable it has no explicit value for by reflection, reading a
    /// static property named like the variable. The deprecated <c>AddressablesVariables</c> types
    /// exist only to answer those reads, and their getters report here. Because
    /// <see cref="CatalogVariables.Publish"/> clears the Addressables cache and this probe before
    /// each catalog, a hit means exactly one thing: the catalog being loaded was published before
    /// the variables were renamed. Nothing about the load itself changes; the application reads
    /// <see cref="Tokens"/> afterwards and reports.
    /// </remarks>
    public static class LegacyCatalogProbe
    {
        private static readonly HashSet<string> tokens = new();

        /// <summary>The legacy variable names the current catalog asked for.</summary>
        public static IReadOnlyCollection<string> Tokens => tokens;

        public static bool Hit => tokens.Count > 0;

        public static void Record(string token) => tokens.Add(token);

        public static void Reset() => tokens.Clear();
    }
}
