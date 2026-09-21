using System;

namespace Reflectis
{
    /// <summary>
    /// Deprecated: the name catalogs published before the 2026-08 brand rename record their
    /// runtime variables under. Kept resolvable so those worlds keep loading.
    /// </summary>
    /// <remarks>
    /// Same mechanism and same retirement condition as <see cref="Virtuademy.AddressablesVariables"/>:
    /// Addressables reads these by reflection from a catalog whose <c>RemoteLoadPath</c> still
    /// says <c>{Reflectis.AddressablesVariables.BaseUrl}</c>; without a type answering to that
    /// name the placeholder is never substituted and the bundle path fails as
    /// <c>Invalid path in AssetBundleProvider</c>. The getters report to
    /// <see cref="Virtuademy.LegacyCatalogProbe"/> so the application can say which world is
    /// still on the old catalog.
    /// </remarks>
    [Obsolete("Legacy catalog variable (pre-rename worlds), resolved by Addressables through reflection only. Use Virtuademy.CatalogVariables.")]
    public static class AddressablesVariables
    {
        public static string BaseUrl
        {
            get
            {
                Virtuademy.LegacyCatalogProbe.Record("Reflectis.AddressablesVariables.BaseUrl");
                return Virtuademy.CatalogVariables.BaseUrl;
            }
        }

        public static string WorldId
        {
            get
            {
                Virtuademy.LegacyCatalogProbe.Record("Reflectis.AddressablesVariables.WorldId");
                return Virtuademy.CatalogVariables.WorldId;
            }
        }
    }
}
