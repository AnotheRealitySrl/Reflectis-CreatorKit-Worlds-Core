using Virtuademy.SDK.Environments.Analytics;
using Virtuademy.SDK.Environments.Interaction;
using Virtuademy.SDK.Environments.Placeholders;
using Virtuademy.SDK.Core;
using Virtuademy.SDK.Core.ApplicationManagement;
using Virtuademy.ScriptingApi;


using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Events;

namespace Virtuademy.SDK.Environments
{
    /// <summary>
    /// Everything an authored world can ask the platform to do.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>This is the whole boundary between a world and the application that runs it.</b> The
    /// members below are the operations the shipped Visual Scripting nodes and placeholders actually
    /// perform — derived from all 77 call sites, not designed in the abstract — flattened so that a
    /// node asks for a result and never for a system. Nothing in this package names a system, a
    /// system contract or the system manager any more; the implementation lives in the main project
    /// and is the only thing that knows which system answers which member.
    /// </para>
    /// <para>
    /// <b>Why flat and not a set of system accessors.</b> An interface handing out
    /// <c>IClientModelSystem</c> or <c>ICharacterControllerSystem</c> would move the same coupling
    /// behind one indirection: the package would still name every contract and still decide which
    /// system to orchestrate for a given effect. Here the package states the effect —
    /// "teleport the player", "translate this key", "is this session multiplayer" — and the
    /// application decides how. That is also what makes the surface reviewable: it is a list of
    /// capabilities, and it only grows when a node needs something new.
    /// </para>
    /// <para>
    /// <b>What may appear in a signature.</b> Types this package owns (the <c>CM*</c> models,
    /// placeholders, the interaction and spawner contracts), the platform's wire DTOs, engine types,
    /// and primitives. Never a system, never the framework. Deep property walks the nodes used to do
    /// — <c>CurrentSession.Experience.Environment.Name</c>,
    /// <c>AvatarInstance.CharacterReference.HeadReference</c> — became members here, because a node
    /// walking a chain of models is a node that knows how the application is built.
    /// </para>
    /// <para>
    /// The models are returned as themselves where returning them *is* the node's purpose — the
    /// "Get CMUser" node exists to give an author a <c>CMUser</c> — and flattened everywhere a node
    /// only consumed one value out of them.
    /// </para>
    /// </remarks>
    public interface IVirtuademyFramework
    {
        #region Session, experience and environment

        /// <summary>The session this world is running in, or null outside one.</summary>
        SessionView CurrentSession { get; }

        /// <summary>The environment of the running experience.</summary>
        EnvironmentView CurrentEnvironment { get; }

        /// <summary>The addressable name of the running environment.</summary>
        string CurrentEnvironmentName { get; }

        /// <summary>Whether the running environment is authored as multiplayer.</summary>
        bool IsCurrentEnvironmentMultiplayer { get; }

        /// <summary>Whether the current session actually runs multiplayer.</summary>
        bool IsCurrentSessionMultiplayer { get; }

        /// <summary>The platform's id for the current session, or empty outside one.</summary>
        string SessionId { get; }

        /// <summary>
        /// Whether the shard the local player is in accepts newcomers. Null when there is no shard,
        /// which a node must be able to tell apart from "closed".
        /// </summary>
        bool? IsCurrentShardOpen { get; }

        /// <summary>Opens or closes the local player's shard to newcomers.</summary>
        Task SetCurrentShardOpen(bool open);

        /// <summary>
        /// The experience published under <paramref name="addressableName"/>, or null when the
        /// tenant has none. Used by the scene-change nodes to find out whether a scene exists before
        /// trying to go there.
        /// </summary>
        Task<ExperienceView> FindExperienceByAddressableName(string addressableName);

        #endregion

        #region Users

        /// <summary>The local player.</summary>
        UserView LocalUser { get; }

        /// <summary>Another user by platform id.</summary>
        Task<UserView> GetUser(int userId);

        #endregion

        #region Save data and leaderboards

        /// <summary>The local player's saved value for <paramref name="key"/>, or null.</summary>
        object GetMySaveData(string key);

        /// <summary>Stores a value against <paramref name="key"/> for the local player.</summary>
        void SetMySaveData(string key, object value);

        /// <summary>Removes the local player's value for <paramref name="key"/>.</summary>
        void DeleteMySaveData(string key);

        /// <summary>Submits a leaderboard record for the local player.</summary>
        Task CreateLeaderboardRecord(string leaderboardKey, float value);

        #endregion

        #region The local player's body

        /// <summary>The character's transform, or null before the player is embodied.</summary>
        Transform PlayerTransform { get; }

        /// <summary>The head transform, for anything that follows the player's gaze.</summary>
        Transform PlayerHeadTransform { get; }

        /// <summary>The left interactor transform. Null on platforms with no hands.</summary>
        Transform PlayerLeftHandTransform { get; }

        /// <summary>The right interactor transform. Null on platforms with no hands.</summary>
        Transform PlayerRightHandTransform { get; }

        /// <summary>Moves the player to a pose. Does not fade — see <see cref="FadeToBlack"/>.</summary>
        void MovePlayer(Pose pose);

        /// <summary>Shows or hides the local player's own avatar meshes.</summary>
        void ShowOwnAvatar(bool visible);

        /// <summary>Shows or hides everybody else's avatars. Rendering only.</summary>
        void ShowOtherAvatars(bool visible);

        #endregion

        #region Input and camera

        /// <summary>
        /// Enables or disables the player's movement input, leaving every other input setting as it
        /// is.
        /// </summary>
        void EnablePlayerMovement(bool enable);

        /// <summary>Restores the project's default input settings.</summary>
        void ApplyDefaultInputSettings();

        /// <summary>
        /// Leaves the camera fixed and takes every input away from the player: no movement, no
        /// dragging, no zoom.
        /// </summary>
        /// <remarks>
        /// These three <c>Use…CameraInput</c> members replaced a single one that took the
        /// platform's <c>InputSettings</c> object, which a world then had to fill in — three nodes
        /// and a placeholder each constructed one out of five booleans whose meaning is not
        /// discoverable from a graph. Each of the three names an arrangement the platform actually
        /// supports, and the settings object stays where it belongs, inside the application.
        /// </remarks>
        void UseStaticCameraInput(bool constrainRotation);

        /// <summary>
        /// Lets the player rotate the camera by dragging, and nothing else — no movement, no zoom.
        /// </summary>
        void UseDragRotationCameraInput(bool constrainRotation);

        /// <summary>
        /// Gives the camera back its full range — dragging, third person and zoom — while the
        /// player still cannot move.
        /// </summary>
        void UseFreeCameraInput(bool constrainRotation);

        /// <summary>Sets the camera's rotation speed on both axes.</summary>
        void ChangeCameraSpeed(float xSpeed, float ySpeed);

        /// <summary>Switches to the first-person camera.</summary>
        void SetFirstPersonCamera();

        /// <summary>Switches to the third-person camera.</summary>
        void SetThirdPersonCamera();

        /// <summary>Moves the camera to a point and leaves it there.</summary>
        Task MoveCameraToPoint(Transform target);

        /// <summary>
        /// Enters the pan state around <paramref name="target"/>. The bounds default to the same
        /// values the character controller has always used, so a node that only has a target passes
        /// one argument.
        /// </summary>
        Task EnterCameraPan(Transform target,
                            float maxZoom = 0.0001f,
                            float minZoom = 1f,
                            float maxYRotation = 85f,
                            float minYRotation = -85f,
                            float maxXRotation = 180f,
                            float minXRotation = -180f,
                            bool cameraInteraction = false);

        /// <summary>Leaves the pan state and gives movement back to the player.</summary>
        Task ExitCameraPan();

        #endregion

        #region Fade

        /// <summary>Fades the view to black. <paramref name="onDone"/> runs when it has.</summary>
        void FadeToBlack(Action onDone = null);

        /// <summary>Fades the view back in. <paramref name="onDone"/> runs when it has.</summary>
        void FadeFromBlack(Action onDone = null);

        #endregion

        #region Localization

        /// <summary>
        /// Whether the application provides localization at all. The language nodes check this
        /// because a world can run in a host that has none.
        /// </summary>
        bool IsLocalizationAvailable { get; }

        /// <summary>The language in use, by name.</summary>
        string CurrentLanguage { get; }

        /// <summary>The language in use, as its code.</summary>
        string CurrentLanguageCode { get; }

        /// <summary>The language in use before the last change, by name.</summary>
        string PreviousLanguage { get; }

        /// <summary>The language in use before the last change, as its code.</summary>
        string PreviousLanguageCode { get; }

        /// <summary>Every language this tenant offers.</summary>
        List<string> AvailableLanguages { get; }

        /// <summary>The translation authored for <paramref name="key"/>.</summary>
        string Translate(string key);

        /// <summary>Switches the language for the whole application.</summary>
        void SetLanguage(string language);

        /// <summary>Raised after the language changes, carrying the new language.</summary>
        UnityEvent<string> LanguageChanged { get; }

        #endregion

        #region Networking

        /// <summary>Whether this client is the one the others follow.</summary>
        bool IsMasterClient { get; }

        /// <summary>A clock every client in the session agrees on.</summary>
        /// <remarks>
        /// Node: <c>Reflectis Networking: Get current network time</c>. The fallback lives here
        /// rather than in the node: the shared clock exists only when the application is online
        /// *and* the session is multiplayer, and deciding that is not something a world should have
        /// to know.
        /// </remarks>
        double SharedNetworkTime { get; }

        /// <summary>Raised when another player joins the local player's shard.</summary>
        UnityEvent<PlayerData> OtherPlayerEntered { get; }

        /// <summary>Raised when another player leaves it.</summary>
        UnityEvent<PlayerData> OtherPlayerLeft { get; }

        /// <summary>
        /// Raised when the local client takes ownership of a synced object, carrying the object.
        /// </summary>
        /// <remarks>
        /// Node: <c>Reflectis Synced Object: On Owner Changed</c>. These three carry a
        /// <see cref="GameObject"/> rather than the synced-object component because ownership is a
        /// fact about the object, and every consumer either has the GameObject already or wants it.
        /// <para>
        /// Unlike the other events here, the application does not forward these from a system —
        /// there is no ownership system. The per-object network bridge raises them, which is the
        /// same signal it already fans out to the graph nodes.
        /// </para>
        /// </remarks>
        UnityEvent<GameObject> SyncedObjectOwnerChanged { get; }

        /// <summary>Raised when the local client loses ownership of a synced object.</summary>
        /// <remarks>Node: <c>Reflectis Synced Object: On Owner Lost</c>.</remarks>
        UnityEvent<GameObject> SyncedObjectOwnerLost { get; }

        /// <summary>Raised when a request for ownership was refused.</summary>
        /// <remarks>Node: <c>Reflectis Synced Object: On Owner Request Failed</c>.</remarks>
        UnityEvent<GameObject> SyncedObjectOwnershipRequestFailed { get; }

        #endregion

        #region The world's own lifecycle

        /// <summary>
        /// Spawns one of the project's addressable assets and parents it where asked.
        /// </summary>
        /// <remarks>
        /// These six members replaced <c>IReflectisApplicationManager.Instance</c>, a static
        /// singleton on an interface that seven files in this package reached into. It was the same
        /// arrangement <c>SM.GetSystem&lt;T&gt;()</c> was — a world resolving the application and
        /// then orchestrating it — and it survived the first pass because a census of
        /// <c>SM.GetSystem</c> call sites cannot see a static property.
        /// </remarks>
        Task<GameObject> SpawnProjectAsset(string objectKey, Transform parent = null);

        /// <summary>
        /// Resolves the placeholders on <paramref name="target"/>, optionally on its children too.
        /// </summary>
        Task InitializePlaceholders(GameObject target, bool includeChildren = false);

        /// <summary>
        /// Shows or hides the objects spawned into the world, except the ones passed in.
        /// </summary>
        void EnableSpawnedObjects(bool enable, List<GameObject> except = null);

        /// <summary>
        /// Leaves this world and joins <paramref name="experience"/>. False when the join fails.
        /// </summary>
        Task<bool> JoinExperience(ExperienceView experience, bool multiplayer);

        /// <summary>Leaves this world and returns to the lobby.</summary>
        Task LoadLobby();

        #endregion

        #region Platform

        /// <summary>Which platform the world is running on.</summary>
        ESupportedPlatform RuntimePlatform { get; }

        #endregion

        #region Analytics

        /// <summary>Mints the id that ties a run of an experience together.</summary>
        Task GenerateExperienceGuid(string key);

        /// <summary>Emits an analytic.</summary>
        void SendAnalytic(EAnalyticVerb verb, AnalyticDTO analytic);

        #endregion

        #region Help and tutorial

        /// <summary>
        /// Whether the application provides the help panel. The tutorial nodes check this for the
        /// same reason the language ones do.
        /// </summary>
        bool IsHelpAvailable { get; }

        /// <summary>Raised once the help panel has finished closing.</summary>
        UnityEvent HelpClosed { get; }

        /// <summary>Opens the help panel.</summary>
        Task OpenHelp();

        /// <summary>Closes it.</summary>
        Task CloseHelp();

        #endregion

        #region Inventory, tools, spawning, menus

        /// <summary>
        /// Puts a pickable into the container inventory. False when it would not fit, which is what
        /// the node reports back to the graph.
        /// </summary>
        bool AddPickableToInventory(PickablePlaceholder pickable);

        /// <summary>Spawns the right/wrong feedback at a point.</summary>
        void DisplayPickFeedback(Transform spawnTransform, bool correct);

        /// <summary>Sets the opacity of the tool inventory.</summary>
        void SetInventoryAlpha(float alpha);

        /// <summary>
        /// Spawns the platform's general container at a point, carrying the payload a spawnable
        /// placeholder needs to resolve itself.
        /// </summary>
        /// <remarks>
        /// The general container is the only one of the platform's prefabs a world ever asks for -
        /// the spawn node hard-codes it, and even keys its payload "GeneralContainerSpawn" - so this
        /// takes no prefab identifier. Naming the platform's prefab catalogue here would have put a
        /// list of fifteen internal prefabs in the authoring surface for the sake of one value.
        /// </remarks>
        Task<GameObject> SpawnGeneralContainer(Vector3 position,
                                               Quaternion rotation,
                                               bool onNetwork = true,
                                               object[] data = null);

        /// <summary>
        /// Runs the transition a <c>GameObject</c> provides, entering it or leaving it.
        /// </summary>
        /// <remarks>
        /// The node used to fetch the platform's transition component off the object itself and
        /// call it. Which component provides a transition is the application's business; a world
        /// says "transition this object".
        /// </remarks>
        void DoTransition(GameObject target, bool enter);

        /// <summary>Hides the contextual menu, whatever it is currently attached to.</summary>
        Task HideContextualMenu();

        /// <summary>Raised when the selected interactable changes.</summary>
        UnityEvent<IVisualScriptingInteractable> SelectedInteractableChanged { get; }

        #endregion
    }
}
