using UnityEngine.AddressableAssets.Initialization;

namespace Virtuademy
{
    /// <summary>
    /// The two values a published world's catalog needs at load time — the tenant's content base
    /// URL and the world id — and the names the catalog records them under.
    /// </summary>
    /// <remarks>
    /// <para>
    /// A world's <c>RemoteLoadPath</c> is a template:
    /// <c>{Catalog.BaseUrl}/{Catalog.WorldId}/[PlayerVersionOverride]/[BuildTarget]</c>.
    /// Addressables expands each <c>{name}</c> through <see cref="AddressablesRuntimeProperties"/>,
    /// which looks the name up in its explicit table first and only then tries to read it as a
    /// static property by reflection. <see cref="Publish"/> fills the table, so these names are
    /// <b>not</b> types and carry no brand: a wire contract, like the Photon prefab keys. That is
    /// the point. Until 2026-09-21 the template named a real type,
    /// <c>Virtuademy.AddressablesVariables</c> (and <c>Reflectis.AddressablesVariables</c> before
    /// the brand rename), which tied every published catalog to a C# identifier and broke every
    /// world each time that identifier moved.
    /// </para>
    /// <para>
    /// Catalogs built against the old names keep loading through the deprecated compatibility
    /// types beside this class, which record their use in <see cref="LegacyCatalogProbe"/>; the
    /// application turns that into a diagnostic, so the worlds still to rebuild are known rather
    /// than guessed.
    /// </para>
    /// </remarks>
    public static class CatalogVariables
    {
        /// <summary>Runtime variable holding the tenant's content base URL.</summary>
        public const string BaseUrlName = "Catalog.BaseUrl";

        /// <summary>Runtime variable holding the world id, or <c>Tenant</c> for the tenant catalog.</summary>
        public const string WorldIdName = "Catalog.WorldId";

        public static string BaseUrl { get; private set; }
        public static string WorldId { get; private set; }

        /// <summary>
        /// Publishes both values for the catalog about to be loaded. Clears the property cache
        /// first: Addressables also caches what it resolved by reflection, and a value cached for
        /// the previous world must not leak into the next.
        /// </summary>
        public static void Publish(string baseUrl, string worldId)
        {
            BaseUrl = baseUrl;
            WorldId = worldId;

            AddressablesRuntimeProperties.ClearCachedPropertyValues();
            AddressablesRuntimeProperties.SetPropertyValue(BaseUrlName, baseUrl);
            AddressablesRuntimeProperties.SetPropertyValue(WorldIdName, worldId);

            LegacyCatalogProbe.Reset();
        }

        /// <summary>The form a runtime variable takes inside an Addressables path template.</summary>
        public static string Placeholder(string name) => "{" + name + "}";
    }
}
