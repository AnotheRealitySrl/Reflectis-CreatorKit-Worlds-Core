using Virtuademy.ScriptingApi;
using System;

namespace Virtuademy.Environments.ScriptingApi
{
    /// <summary>
    /// The entry point an authored script uses to reach the world it runs in.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This assembly is the <b>only</b> first-party assembly an interpreted script may reference.
    /// The server-side whitelist (<c>policy.json</c> in <c>SPACS-Virtuademy/DllVerification</c>)
    /// allows the assembly name <c>Virtuademy.Environments.ScriptingApi</c> and the matching
    /// namespace prefix, and denies <c>Virtuademy.SDK</c>, <c>Virtuademy.CreatorKit</c>,
    /// <c>Virtuademy.Worlds</c>, <c>Virtuademy.Core</c> and <c>Virtuademy.ClientModels</c>
    /// outright. So whatever is not on this surface is not reachable from a script, and adding a
    /// member here is the only way to widen what scripts can do.
    /// </para>
    /// <para>
    /// <b>Nothing here introduces a capability the platform did not already grant.</b> Every member
    /// maps onto one of <c>IVirtuademyFramework</c>'s, which was itself derived from what the
    /// shipped Visual Scripting nodes and placeholders do. A graph and a script get the same reach.
    /// </para>
    /// <para>
    /// <b>Four constraints the whitelist and the interpreter put on the shape of this API</b>, all
    /// read off the policy rather than chosen:
    /// </para>
    /// <list type="bullet">
    /// <item><description>
    /// <c>System.Threading</c> is denied, so a script cannot name <c>Task</c> and cannot use
    /// <c>async</c>/<c>await</c>. Everything asynchronous here takes a <see cref="Action"/> or
    /// returns nothing.
    /// </description></item>
    /// <item><description>
    /// No first-party type may appear in a signature. That is why nothing here hands back a
    /// <c>CMUser</c> or a placeholder: the models are flattened to the primitives a script actually
    /// reads — <see cref="ISessionApi.LocalUserId"/>, <see cref="ISessionApi.LocalUserName"/> — the
    /// same flattening <c>IVirtuademyFramework</c> did to the nodes' property walks.
    /// </description></item>
    /// <item><description>
    /// No generic member, and no generic delegate over a value type. A generic instantiated only
    /// from interpreted code has no AOT counterpart and fails at load, not at compile time. This is
    /// why the other-player events are absent: they would need <c>Action&lt;int&gt;</c>.
    /// </description></item>
    /// <item><description>
    /// No <c>Nullable&lt;T&gt;</c>, for the same reason — hence
    /// <see cref="ISessionApi.HasShard"/> beside <see cref="ISessionApi.IsShardOpen"/> instead of a
    /// <c>bool?</c>.
    /// </description></item>
    /// </list>
    /// <para>
    /// <b>Availability.</b> The implementation is installed by the application at startup, before
    /// the first scene loads. A script running where it is absent gets an
    /// <see cref="InvalidOperationException"/> naming the group it asked for, rather than a null
    /// reference.
    /// </para>
    /// </remarks>
    public static class VirtuademyEnvironments
    {
        private static IWorldBackend backend;

        /// <summary>
        /// Whether the world runtime is present. False in a project that ships this scripting
        /// surface without the platform behind it.
        /// </summary>
        public static bool IsAvailable => backend != null;

        /// <summary>The local player: where they are, what they can do, what is visible.</summary>
        public static IPlayerApi Player => Group(backend?.Player, nameof(Player));

        /// <summary>The active language and the strings authored against it.</summary>
        public static ILocalizationApi Localization => Group(backend?.Localization, nameof(Localization));

        /// <summary>Read-only facts about the session this world is running in.</summary>
        public static ISessionApi Session => Group(backend?.Session, nameof(Session));

        /// <summary>Fading the view in and out.</summary>
        public static IScreenApi Screen => Group(backend?.Screen, nameof(Screen));

        /// <summary>The local player's own saved values, and the leaderboards.</summary>
        public static ISaveDataApi SaveData => Group(backend?.SaveData, nameof(SaveData));

        /// <summary>Which kind of device the world is running on.</summary>
        public static IPlatformApi Platform => Group(backend?.Platform, nameof(Platform));

        /// <summary>The help panel, when the host provides one.</summary>
        public static IHelpApi Help => Group(backend?.Help, nameof(Help));

        /// <summary>The world around the script: placeholders, spawned objects, transitions.</summary>
        public static ISceneApi Scene => Group(backend?.Scene, nameof(Scene));

        /// <summary>The player's tools and the feedback they produce.</summary>
        public static IToolsApi Tools => Group(backend?.Tools, nameof(Tools));

        /// <summary>
        /// Ownership of the objects a multiplayer session keeps in step: asking for the right to
        /// drive one, giving it back, and hearing when it changed hands.
        /// </summary>
        public static ISyncApi Sync => Group(backend?.Sync, nameof(Sync));

        /// <summary>
        /// Installs the implementation. Internal by design: a script can reference this assembly in
        /// full, so a public installer would let one script replace the surface every other script
        /// is calling.
        /// </summary>
        internal static void Install(IWorldBackend implementation)
        {
            backend = implementation ?? throw new ArgumentNullException(nameof(implementation));
        }

        /// <remarks>
        /// Generic, and safe to be: it is private and called only from this assembly, which is
        /// compiled ahead of time. The no-generics rule is about what interpreted code can
        /// instantiate.
        /// </remarks>
        private static T Group<T>(T api, string name) where T : class
        {
            if (api == null)
            {
                throw new InvalidOperationException(
                    $"VirtuademyEnvironments.{name} is not available: the Virtuademy runtime is not present in " +
                    "this project, or it has not finished starting up. Check VirtuademyEnvironments.IsAvailable first " +
                    "if the script can run outside one.");
            }

            return api;
        }
    }
}
