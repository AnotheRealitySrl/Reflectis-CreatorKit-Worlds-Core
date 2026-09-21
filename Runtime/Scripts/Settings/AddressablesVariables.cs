using System;

namespace Virtuademy
{
    /// <summary>
    /// Deprecated: the name catalogs published between the 2026-08 brand rename and 2026-09-21
    /// record their runtime variables under. Kept resolvable so those worlds keep loading.
    /// </summary>
    /// <remarks>
    /// Addressables reaches this type by reflection only, from a catalog whose
    /// <c>RemoteLoadPath</c> still says <c>{Virtuademy.AddressablesVariables.BaseUrl}</c>. The
    /// getters hand back what <see cref="CatalogVariables.Publish"/> published and tell
    /// <see cref="LegacyCatalogProbe"/> they were read. No code should name this type; new
    /// catalogs use <see cref="CatalogVariables"/>. Retire it, together with its
    /// <c>Reflectis</c> twin, once the <c>LegacyAddressablesCatalog</c> diagnostic has been silent
    /// long enough to trust that every published world — creator content included — was rebuilt.
    /// </remarks>
    [Obsolete("Legacy catalog variable, resolved by Addressables through reflection only. Use CatalogVariables.")]
    public static class AddressablesVariables
    {
        public static string BaseUrl
        {
            get
            {
                LegacyCatalogProbe.Record("Virtuademy.AddressablesVariables.BaseUrl");
                return CatalogVariables.BaseUrl;
            }
        }

        public static string WorldId
        {
            get
            {
                LegacyCatalogProbe.Record("Virtuademy.AddressablesVariables.WorldId");
                return CatalogVariables.WorldId;
            }
        }
    }
}
