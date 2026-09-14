using System;

using UnityEngine;

using Virtuademy.ScriptingApi;

namespace Virtuademy.Environments.ScriptingApi
{
    /// <summary>
    /// The world around the script: its placeholders, the objects spawned into it, its transitions,
    /// and the way out of it.
    /// </summary>
    public interface ISceneApi
    {
        /// <summary>
        /// Resolves the placeholders on <paramref name="target"/> — turning the authored stand-ins
        /// into the real thing — optionally on its children too.
        /// </summary>
        /// <remarks>Node: <c>Reflectis Placeholder: Initialize Placeholder</c>.</remarks>
        void InitializePlaceholders(GameObject target, bool includeChildren = false);

        /// <summary>
        /// Shows or hides the objects spawned into the world. <paramref name="except"/> is left
        /// alone, which is how a script keeps the object it is attached to visible.
        /// </summary>
        /// <remarks>Node: <c>Reflectis Scene: Enable Spawned Objects</c>.</remarks>
        void ShowSpawnedObjects(bool visible, GameObject except = null);

        /// <summary>
        /// Runs the transition an object provides, entering it or leaving it. Which component
        /// provides a transition is the application's business.
        /// </summary>
        /// <remarks>Node: <c>Reflectis Transition Provider: Do Transition</c>.</remarks>
        void RunTransition(GameObject target, bool enter);

        /// <summary>
        /// Leaves this world and returns to the lobby. Nothing after this call is guaranteed to
        /// run: the scene is on its way out.
        /// </summary>
        /// <remarks>Node: <c>Reflectis Platform: Load Lobby</c>.</remarks>
        void ReturnToLobby();
    
        /// <summary>
        /// Spawns an addressable the creator shipped with the project, optionally under
        /// <paramref name="parent"/>, and hands the instance to <paramref name="onSpawned"/>.
        /// </summary>
        /// <remarks>Used by the dialog panel spawner.</remarks>
        WorldOperation SpawnProjectAsset(string objectKey, Transform parent, Action<GameObject> onSpawned = null);

        /// <summary>
        /// Spawns the platform's general container, the one prefab of the platform's own that a
        /// world ever asks for. Takes no prefab identifier for that reason: naming the platform's
        /// catalogue here would put fifteen internal prefabs in the authoring surface for one value.
        /// </summary>
        /// <remarks>
        /// Fire and forget, unlike <see cref="SpawnProjectAsset"/>: the node behind it already
        /// discarded the instance it awaited. Node: <c>Spawn Spawnable Object</c>.
        /// </remarks>
        void SpawnContainer(Vector3 position, Quaternion rotation, bool onNetwork = true, object[] data = null);

        /// <summary>
        /// The environment is up and everything in it has been set up: placeholders initialised,
        /// spawned objects registered, movement given back to the player.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is the moment the <c>Reflectis Scene: On Setup</c> node fires, not the earlier
        /// <c>On Load</c> one. That earlier moment is real but it is before the placeholders
        /// exist, which is almost never what a script that says "when the scene is ready" means,
        /// so it is not published here under a name that would mislead.
        /// </para>
        /// <para>
        /// A script has no other way to know: the whitelist denies
        /// <c>UnityEngine.SceneManagement.SceneManager</c>, so without this there is nothing to
        /// observe.
        /// </para>
        /// </remarks>
        event Action Ready;

        /// <summary>
        /// The environment is being torn down. The last moment at which anything in it can be
        /// read, and the place to save what should outlive it — <c>SaveData.Set</c> is synchronous,
        /// so a value written here does land.
        /// </summary>
        /// <remarks>
        /// <b>A handler cannot hold the teardown up.</b> The equivalent node can: the application
        /// awaits every graph flow before continuing, and a plain event has nowhere to report that
        /// it is not finished. Anything that must complete before the world goes has to be
        /// synchronous, or be a graph.
        /// </remarks>
        event Action Unloading;
}
}
