using Virtuademy.ScriptingApi;

namespace Virtuademy.Environments.ScriptingApi
{
    /// <summary>
    /// What the application hands to <see cref="VirtuademyEnvironments.Install"/>. Internal: a script may call the
    /// groups but may not supply them, and adding a group to the surface means adding a property
    /// here rather than changing an installer signature.
    /// </summary>
    internal interface IWorldBackend
    {
        IPlayerApi Player { get; }

        ILocalizationApi Localization { get; }

        ISessionApi Session { get; }

        IScreenApi Screen { get; }

        ISaveDataApi SaveData { get; }

        IPlatformApi Platform { get; }

        IHelpApi Help { get; }

        ISceneApi Scene { get; }

        IToolsApi Tools { get; }

        ISyncApi Sync { get; }
    }
}
