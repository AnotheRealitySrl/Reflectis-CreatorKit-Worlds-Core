namespace Reflectis
{
    /// <summary>
    /// The name Addressables knows these variables by in every world built before the
    /// rename. Kept resolvable so those worlds keep loading.
    /// </summary>
    /// <remarks>
    /// A world's catalog records its runtime variables by the <b>fully qualified type
    /// name</b> — see <c>AddressablesManagementWindow</c>, which composes them as
    /// <c>$"{typeof(AddressablesVariables)}.{property}"</c>. Every catalog published
    /// before the type moved to the <c>Virtuademy</c> namespace therefore asks for
    /// <c>Reflectis.AddressablesVariables.BaseUrl</c> and <c>…WorldId</c>. Without a type
    /// answering to that name the variable is never substituted and Addressables tries to
    /// open a bundle at a path still containing the placeholder, which fails as
    /// <c>Invalid path in AssetBundleProvider</c>.
    /// <para>
    /// These are delegating properties, not a second pair of fields: both names must read
    /// and write the same values, since the runtime sets them through
    /// <see cref="Virtuademy.AddressablesVariables"/> while an old catalog reads them
    /// through this one.
    /// </para>
    /// <para>
    /// Retire this only once no published world resolves through it — that means every
    /// world rebuilt, creator content included, not merely the ones we build ourselves.
    /// </para>
    /// </remarks>
    public static class AddressablesVariables
    {
        public static string BaseUrl
        {
            get => Virtuademy.AddressablesVariables.BaseUrl;
            set => Virtuademy.AddressablesVariables.BaseUrl = value;
        }

        public static string WorldId
        {
            get => Virtuademy.AddressablesVariables.WorldId;
            set => Virtuademy.AddressablesVariables.WorldId = value;
        }
    }
}
