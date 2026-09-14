using System;

namespace Virtuademy.SDK.Environments
{
    /// <summary>
    /// How an authored world reaches <see cref="ILegacyWorldFramework"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The implementation is installed once by the application that hosts the world, before the
    /// first scene loads. Nodes and placeholders read <see cref="Current"/>; nothing in a world's
    /// own content installs anything.
    /// </para>
    /// <para>
    /// It lives in namespace <c>Virtuademy.SDK.Environments</c>, the common ancestor of every
    /// namespace in this package, so every call site resolves it by namespace walk-up with no
    /// <c>using</c> added.
    /// </para>
    /// <para>
    /// With nothing installed, <see cref="Current"/> throws and says so. That is deliberately louder
    /// than the framework lookup it replaces, which returned null and left a
    /// <see cref="NullReferenceException"/> to surface one line later inside whatever the caller did
    /// with the result.
    /// </para>
    /// </remarks>
    public static class VirtuademyFramework
    {
        private static ILegacyWorldFramework current;

        /// <summary>Whether a host application has installed the framework yet.</summary>
        public static bool IsInstalled => current != null;

        /// <summary>
        /// The framework this world is running against.
        /// </summary>
        /// <exception cref="InvalidOperationException">Nothing has installed an implementation.</exception>
        public static ILegacyWorldFramework Current
        {
            get
            {
                if (current == null)
                {
                    throw new InvalidOperationException(
                        "No Virtuademy framework is installed, so the world cannot reach the platform. " +
                        "A Virtuademy application installs one before the first scene loads; check " +
                        "VirtuademyFramework.IsInstalled if this code can run outside one.");
                }

                return current;
            }
        }

        /// <summary>
        /// Registers the host application's implementation. Called once, by the application.
        /// </summary>
        public static void Install(ILegacyWorldFramework framework)
        {
            current = framework ?? throw new ArgumentNullException(nameof(framework));
        }
    }
}
