using System;

using Virtuademy.ScriptingApi;

namespace Virtuademy.Environments.ScriptingApi
{
    /// <summary>
    /// What only an authored environment can do: the avatar, the world around it, the tools in it,
    /// and the objects a multiplayer session keeps in step.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>The other half of the surface.</b> <see cref="IVirtuademyFramework"/> carries what an
    /// external app embedded in Virtuademy needs too — session, language, save data, device, screen,
    /// help. The four groups here have no meaning outside a world: an external app has no avatar
    /// rig, no placeholders, no spawned objects and no synced ownership. Keeping them apart is what
    /// lets that app install the contracts package without installing the authoring one.
    /// </para>
    /// <para>
    /// <b>Two entry points, not one.</b> A creator writes <c>IVirtuademyGameplay.Current.Player</c>
    /// and <c>IVirtuademyFramework.Current.Session</c>; the split says which of the two an
    /// expression depends on, which is the thing a shared surface would hide.
    /// </para>
    /// <para>
    /// <b>Written to the interpreter's budget</b>, like its sibling: no <c>Task</c>, no generic
    /// member, no <c>Nullable&lt;T&gt;</c>, and nothing from a namespace the whitelist refuses.
    /// </para>
    /// </remarks>
    public interface IVirtuademyGameplay
    {
        private static IVirtuademyGameplay current;

        /// <summary>
        /// Whether the world runtime is behind this surface. False in an authoring project, where
        /// the contracts compile but nothing answers them.
        /// </summary>
        static bool IsAvailable => current != null;

        /// <summary>The world this code is running in.</summary>
        /// <exception cref="InvalidOperationException">
        /// Nothing has installed an implementation. Check <see cref="IsAvailable"/> first if the
        /// code can run outside a world.
        /// </exception>
        static IVirtuademyGameplay Current => current ?? throw new InvalidOperationException(
            "No Virtuademy world runtime is installed, so this code cannot reach the world around "
            + "it. A Virtuademy application installs one before the first scene loads; check "
            + "IVirtuademyGameplay.IsAvailable if it can run outside one.");

        /// <summary>Raised when an implementation is installed, for code that initialised first.</summary>
        static event Action Installed;

        /// <summary>
        /// Registers the application's implementation. Internal by design: a script references this
        /// assembly in full, and a public installer would let one script replace the surface every
        /// other script is calling.
        /// </summary>
        internal static void Install(IVirtuademyGameplay gameplay)
        {
            current = gameplay ?? throw new ArgumentNullException(nameof(gameplay));

            Installed?.Invoke();
        }

        /// <summary>The local player: where they are, what they can do, what is visible.</summary>
        IPlayerApi Player { get; }

        /// <summary>The world around the script: placeholders, spawned objects, transitions.</summary>
        ISceneApi Scene { get; }

        /// <summary>The player's tools and the feedback they produce.</summary>
        IToolsApi Tools { get; }

        /// <summary>
        /// Ownership of the objects a multiplayer session keeps in step: asking for the right to
        /// drive one, giving it back, and hearing when it changed hands.
        /// </summary>
        ISyncApi Sync { get; }
    }
}
