# Release notes

## v10.0.0

### Changed
- **The scripting entry point is `VirtuademyEnvironments`, not `World`.** Two things ruled the
  old name out: "Worlds" is leaving the application name — the concept survives on the wire,
  where `WorldDTO` keeps it, but it stops being the word a creator reads on every line — and the
  name carried nothing that said which platform it belonged to.

  Plain `Virtuademy` was the first choice and is ruled out by the compiler, not by taste. Every
  SDK namespace is rooted at `Virtuademy`, so in `Virtuademy.Player` the namespace wins over the
  type and the line fails with CS0234 — in a creator script in the global namespace exactly as
  much as inside the SDK. Reaching it would take
  `global::Virtuademy.Environments.ScriptingApi.Virtuademy.Player`.

  This is a breaking change for authored scripts, taken now because nothing can break: the facade
  is two days old and no script has ever reached publication, because the deployed whitelist still
  named the pre-rename assembly and the deploy path refused every one of them. There is **no
  migrator entry**, deliberately — `World` is far too common a token to rewrite in a creator
  project by substitution (three unrelated `World.` call sites in the application prove it), and
  there is nothing out there to migrate.

  The assembly and the namespace are unchanged, so `policy.json` and the operator overrides need
  no edit: the whitelist is keyed on those, never on a type name.
- **`Virtuademy.SDK.PlatformApi` is `Virtuademy.SDK.ApiData`.** The old name said where the types
  came from — a package that no longer exists under that name — rather than what they are. Three
  analytics graphs shipped in this package name two of those types in their serialized `$type`
  values and were rewritten with them, so they resolve rather than deserialize as `MissingType`.
- The rename migrator gained the two entries that carry a creator project across that rename. The
  assembly entry runs before the namespace entry, because the namespace is a prefix of the
  assembly name and the shorter rule would otherwise consume it.
- **The package is `Virtuademy-SDK-Environments`.** Business ruled out the name "Creator Kit" and this package was the last place it was a wire symbol rather than prose. The id is `com.anotherealitysrl.virtuademy-sdk-environments`, and the three assemblies are `Virtuademy.SDK.Environments`, `Virtuademy.SDK.Environments.Editor` and `Virtuademy.SDK.Environments.HybridCLREditor`.
- Every namespace under the old `Virtuademy.CreatorKit.Worlds` prefix moved to `Virtuademy.SDK.Environments`, by one substitution rule. The `.Core.` segment the merge left behind is gone with it: `...Worlds.Core.ClientModels` is now `...Environments.ClientModels`, and the three namespace pairs the merge had left duplicated (`Placeholders`, the package root, `Editor`) are single namespaces again. That merge was verified collision-free first — no two types of the same name met.
- `Virtuademy.Environments.ScriptingApi` keeps its name. It is the one assembly `policy.json` whitelists by name, that file is deployed, and a published script records it.
- The rename migrator handles both hops in one pass now, and its menu entry is `Package rename migration`. A creator project on the old brand is carried through the brand rename and then this one; a project already on the new brand is caught directly.

### Breaking
- **Every published world must be rebuilt.** A built Addressable bundle records each component by assembly, namespace and class, and all three changed.
- **Every Visual Scripting graph must be migrated**, in a creator's project and in this repo. A graph records each unit as namespace plus type — no assembly — so the namespace change is the one that reaches graphs. Run `Virtuademy Worlds/Creator Kit update routines/Package rename migration`, then `Regenerate Nodes`. **Expect deserialization errors on the first open afterwards, and do not save until they stop**: while a worker cannot resolve a renamed unit yet, Visual Scripting substitutes `MissingType` and that renumbers the JSON `$id`s, which surfaces as "Object definition has not been encountered for object with id=N". Most clear by themselves once the assemblies load — the log then says the missing type "was found" — but some are cached in the asset's imported artifact and survive a reopen: those need a **Reimport** on the asset, then a look at the graph to confirm the units are real nodes, and only then a save — **on every affected asset**, since each caches its own import artifact and the errors surface a few at a time. Saving before that check is the one way to make the loss permanent. Verified on this repo: 55 graph-bearing assets, every `$ref` still preceded by its `$id` after the rename, and no compile error.
- Every `using Virtuademy.CreatorKit.Worlds…` in a creator's own scripts breaks, and so does any asmdef that names one of the three assemblies as a string rather than a GUID. The migrator rewrites both.
- A project that depends on `com.anotherealitysrl.virtuademy-creatorkit-worlds-core` in its `manifest.json` no longer resolves it. Existing registry releases are unaffected: each pins the git repo at a tag, and that tag still carries the old id.
- Five namespaces the package declares are **not** renamed and are not part of this change: `Virtuademy.SDK.Core`, `...Core.ApplicationManagement`, `...Core.ChatBot`, `...Core.Networking` and `...Core.NetworkingSystem`. Those five types moved here from SDK-Core keeping their original namespaces so consumers' `using` directives kept working, and renaming them now would break the very thing they exist to preserve.

## v9.0.0

### Changed
- **Five sibling packages were absorbed into this one**, which is now the whole authoring surface: `Virtuademy-CreatorKit-Worlds-Placeholders` (4.1.0), `-VisualScripting` (2.4.0), `-Tasks` (2.2.1), `-Dialogs` (1.2.0) and `-Analytics` (3.2.0). They live under `Runtime/<Name>` and `Editor/<Name>`; their READMEs and changelogs are kept under `Documentation~/legacy-packages/`. Nothing was rewritten — 389 scripts and the assets beside them moved as they were.
- **Twelve assemblies became three.** Everything runtime compiles into `Virtuademy.CreatorKit.Worlds.Core`, everything editor into `Virtuademy.CreatorKit.Worlds.CoreEditor`, and `Virtuademy.CreatorKit.Worlds.CoreHybridCLREditor` stays separate because it carries a `HYBRIDCLR_INSTALLED` define constraint the rest must not inherit. Assembly *names* are unchanged, so no consumer reference had to be rewritten: every asmdef reference in the project is written as a GUID, and a moved asmdef keeps its GUID.
- Core's own placeholder base classes moved from `Runtime/Placeholders` to `Runtime/PlaceholderBases`, which frees the name for the placeholder set that came in and says what they are.
- The Splines, Quiz and task-scene samples are declared here now, so they stay importable from the Package Manager.

### Breaking
- **Every published world must be rebuilt.** A built Addressable bundle records each component by assembly, namespace and class, and five assemblies stopped existing. Source projects are unaffected — script references resolve by GUID from the `.meta`.
- A project that referenced one of the five packages by id in its `manifest.json` no longer resolves it. There is one package to depend on.
- Namespaces did **not** change in this release, so no `using` in a creator's own scripts breaks, and no Visual Scripting graph does either (a graph records its units by namespace and type, never by assembly).

## v8.1.0

### Added
- Added `BlockedByOtherUser` (64) to `IInteractable.EBlockedState`, for collaborative tools that need to signal "another user is currently in here" without borrowing a flag that already belongs to another feature. The quiz used to reuse `BlockedByLockObject` for this, which meant its own recompute erased the lock an operator had set from the contextual menu. Purely additive: every `allowedBlockedStates` mask in the project is a negative "allow all except", so the new bit is permitted by default everywhere and only the options that must react to it need updating.

## v8.0.0

### Added
- Added `SpawnProjectAssetAsync` signature to `IReflectisApplicationManager`.
- Automatic setup and automatic list management for spawnable objects.
- SFTP support, tenant environments and platform selection in the addressables/env upload windows.
- Moved input actions into core.
- Added dependencies on Reflectis SDK ReflectisApi and TenantConfiguration.

### Changed
- Improved addressables management with a dependency on tenant selection.
- Renamed keys for avatar prefabs.

### Fixed
- Fixed spawnable object editor flow and addressables window first setup.
- Fixed session participants and single-world access handling.

## v7.1.1

### Added
- Added function to set the system language

## v7.0.1

### Added

- Fixed compile issue in `BreakingChangeSolver`.

## v7.0.0

### Changed

- Changed `JoinSession` method signature in `JoinAndLoadSession` of `IReflectisApplicationManager`.

### Added

- Added catalog info and isTenant in `CMEnvironment`.
- Added optional isTenant name in GetExperienceByAddressableName method of `IClientModelSystem`.

## v6.0.0

### Changed

- Revised all `CMWorld`, `CMEvent`, `CMEnvironment` entities to support new structure based on Experiences and Sessions.
- Change legacy signatures of `GetWorldFolders`, `GetWorldAssets` and `SearchWorldAssets` signatures in `IClientModelSystem`.

### Added

- Added `TextBox` option to `EPrefabIdentifier` in `IObjectSpawnerSystem`.
- Added `ManageMySessions `entry in `EFacetIdentifier` of `CMPermission`.
- Add some `[CreateProperty]` decorators in `CMUser` and `CMWorld`.

## v5.0.1

### Fixed

- Fixed issue of `AddressablesConfigurationWindow` not creating addressables settings if missing.
- Fixed issue of unused addressables entries not being cleaned up by `AddressablesConfigurationWindow`.

## v5.0.0

### Changed

- Redesigned the addressables management window, now the configuration and the management of the scenes is more user-friendly:
  the user references the scenes directly without dealing with notion of catalogs and builds them all at once.
- Removed null definitions from `CMEvent`.

## v4.2.0

### Added

- Add missing splines namespaces in `CreatorKitUpgradeWindow`

## v4.1.0

### Added

- Added CreatorKitUpgradeWindow to allow migrate scenes to Reflectis Worlds 2025.1.x versions.

### Fixed

- Add `SelectedInteractable` reference in `IVisualScriptingInteractionSystem` interface.
- Added extra checks to prevent a null reference in case a visual scripting interactable has already been removed from scene.

## v4.0.0

### Changed

- Changed package name, from Virtuademy-SDK-CreatorKit to Reflecits-CreatorKit-Core, and updated namespaces according to new package name.

### Added

- Added ClientModels, Help, ObjectSpawner, ApplicationManagement and SceneObjects modules, previously located in SDK.
  ApplicationManagement does not contain all the logic contained in SDK, but only the part that is specific to Virtuademy.

## v3.8.0

### Added

- Added a "UI starter kit" consisting of a spritesheed useful for faster prototyping.

### Fixed

- Removed an unused camera from pan transform in POI placeholder.
- Add open and close callbacks for POI placeholder

## v3.7.2

### Fixed

- Fixed `AddressablesBundleScriptableObject` configure button.

## v3.7.1

### Fixed

- Fixed package version in package.json

## v3.7.0

### Fixed

- Updated visual scripting graph `SendMultiAnswerQuizData` to correctly send the score to the BackOffice for the analytics.
- Improved animations of ChatBot's humanoid templates.

### Added

- Added new visual scripting node `LoadDefaultEvent`, that that loads the default event. Using this node is equivalent to pressing the "home" button on the HUD (or on the VR tablet).

## v3.6.0

### Added

- Added new visual scripting node `GetLocalPlayerID`, that returns the player ID, which is a numeric identifier that is unique for the user in the current shard context.
- Added new `ChatBotPlaceholder` that allows to set up a chatbot with multimodal (audio/text) input and output.

### Fixed

- Updated visual scripting node `OnOtherPlayerEntered` and `OnOtherPlayerLeft` to correct faulty management of networking events and prevent null reference exceptions after leaving and joining events multiple times.

### Deprecated

- Environment thumbnails loading has been deprecated. Now it is necessary to load thumbnails from the backoffice instead of using the addressable system

## v3.5.0

### Added

- Added new Point of Interest (POI) component placeholders. A POI is an interactable UI element that, once selected, display a panel with info like text, images, video, and links.
- Added new visual scripting event nodes `OnOtherPlayerEntered` and `OnOtherPlayerLeft`, that can detect players entering/leaving the Reflectis event where the local player is currently staying. These nodes also expose two identifiers related to the user that just entered/left: `User Id`, which is a unique identifier related to the Reflectis profile, and `Player Id`, that is a numeric identifier that is unique only in the context of the current shard.
- Added new visual scripting node `ChangeScene`. It can be used to move local player to a static Reflectis event by providing the name of the environment used by the target event.
- Added new visual scripting node `SendMultiAnswerQuizData`. It can be used to send quiz results to the server to display its data on the analytics table.
- Added new visual scripting node `SetCurrentShardOpenState`. It can be used to open/close the current shard (i.e. the shard where the local user is). Closing a shard will prevent more players to join it, and as long as it stays closed it will be treated by Reflectis as if it reached max capacity.
- Added new visual scripting node `GetCurrentShardOpenState`. It can be used to get the state of the current shard: true if it's open, false if it's closed.

## v3.4.0

### Added

- Added `DiagnosticGenerateExperienceIDUnit` to generate unique experienceID
- Added `DiagnosticSendDataUnit` to allow creators to send diagnostic data
- Added `SwitchNetworkMasterUnit` to switch visual scripting flow base on if the client is the master client or not
- Added `OnSceneUnloadEventUnit` to launch events on scene change
- Added `GetCMUserByIDNode` to get a CMUser data by using its ID.
- Added `URLImageToTexture` to apply a sprite to an image from a URL link.
- Added `ProfileImageURL` to the exposed data of the already existing `CollectPlayerDataNode` node.
- Added new visual scripting node `InitializePlaceholder`. It can be used to initialize the placeholder component of a GameObject that has been instantiated dynamically.
- Added `ConstraintSource` type to visual scripting type library.
- Added Scriptable Object class to create addressable bundles to auto-setup when needed.

### Fixed

- Updated visual scripting node `EnableOtherCharacters` so that it uses the new method `EnableOtherAvatarsMeshes` from the AvatarSystem. The node now works as intended (up until now it was considered as not implemented).
- Updated synced var change unit view to improve their usage.

## v3.3.1

### Fixed

- Fixed synced variables parameters (`OnSyncedVariableInit`,`OnSyncedVariableEventUnit`,`OnSyncedVariablesEventNodes`), removed unused value input and unuesed isSynced variable

## v3.3.0

### Added

- Added `OnSelectedGenericInteractableChangeNode` that will be triggered whenever the current selected generic interactable changes, and its argument is a reference to the new selected item. The event is also fired when the user clicks on an empty area, in which case the reference is null.
- Added `SetFirstPersonCameraModeNode` and `SetThirdPersonCameraModeNode`. They can be used to switch the character controller to first person or third person view respectively.

## v3.2.0

### Added

- Added `startPaused` variable to `BigScreenPlaceholder` to switch off autoplay on start.
- Added `onVideoPlayed` and `onVideoPaused` callbacks in `BigScreenPlaceholder`.
- Added `OnVideoPlayedEventUnit` and `OnVideoPausedEventUnit` scripts to provide callbacks for video player play/pause events when triggered by visual scripting.

## v3.1.3

### Added

- Added localization logic to write and translate custom keys.

### Fixed

- Fixed the configuration of the scale of the environmental dashboard (now support also the collider the dashboard itself).

## v3.1.2

### Fixed

- Added cache for placeholder types in `SceneComponentsMapper`, to improve loading times and avoid operation involving reflection when possible.

## v3.1.1

### Fixed

- Fixed missing validity check of build and load paths in the top-level Addressables settings, which may result in catalogs being built in the wrong folder.

## v3.1.0

### Added

- Added `OnSceneLoadEventNode` that will be triggered during scene loading before placholders' mapping. If marked as coroutine will wait the flows to finish while in fade in.
- Added `OnSceneSetupEventNode` that will be triggered during scene setup. If marked as coroutine will wait the flows to finish while in fade in.
- Added `OnSceneSetupCompletedEventNode` that will be triggered after the setup fadeout is completed.
- Added scale on `InteractablePlaceholder` in manipulable mode (UX feelds since there is no bounding box).
- Added color picker and non proportional scale to `InteractablePaceholder` in contextual menu mode (non proportialScale needs manipulation mode to work properly).
- Added `OnSyncedVariableInit` node that will be triggered when entering the scene. It outputs the value of the synced variable, whether or not it has changed and an event.

## Fixed

- Fixed an issue of the Addressables configuration window not updating properly the load and build paths of the Addressables groups.
- Fixed an issue of the Addressables configuration window not updating the value of a profile variable if such variable was defined but empty.
- Fixed issue of `automaticSetup` and `isNetworking` fields of `InteractablePlaceholder` not being shown in the inspector.
- Fixed the synced position of the synced object when player enter the environment.
- Improved `InteractablePlaceholder`'s custom inspector.
- Disabled CRC check option in Addressables groups.

## v3.0.0

### Changed

- An `InteractablePlaceholder` now accepts viual scripting state machines instead of scriptable actions to handle the generic interaction.
- Improved name conventions of public variables and properties of existing visual scripting nodes.

### Added

- Added `ScreenName` field to `BigScreenPlaceholder` to identify big screens by a custom name instead of its `GameObjsect`'s name.
- Added new visual scripting nodes: interactable behaviours blocking by selection, pan character camera, disable character mesh, teleport player, open-close tutorial.
- Added visual scripting nodes which collect data from current event, current environment and current user.
- Added an `ActionPlayerMap` defining XR inputs (useful for recognizing user input by visual scripting).
- Added visual scripting nodes which detect manipulation start/end of a `Manipulable` object.
- Implemented a utility to auto-setup visual scripting nodes.

### Removed

- Removed awaitable scriptable actions in favour of visual scripting machines.
- Removed legacy `ToastInteractablePlaceholder`.

### Fixed

- Fixed `NetworkPlaceholdersManager` editor window scrolling.
- Fixed `SyncedObject`'s custom inspector not showing the info of the base class.
- Improved `InteractablePlaceholder`'s custom inspector.

## v2.0.0

### Changed

- Massive SPACS -> Reflectis refactor.
- Reorganized package structure.
- Reimplemented `BigScreen` placeholder (with prefab) for an easier configuration and improved flexibility.
- Reimplemented `TeleportPoint` logic (now called `SceneChanger`).
- Reimplemented `ScriptableActions` to support async/await paradigm, and to support the selected interactable as a parameter of the action.

### Added

- `INetworkedPlaceholder` interface to distinguish network placeholder from local one.
- Added an environmental dashboard prefab with placeholder.
- `InteractionPlaceholder` which supports the interaction provided by the `SDK.Interaction` module.
- Add first integration with visual scripting, with nodes for ownership management, variables syncronization and invoking of networked events.
- WebView placeholders (both for screen-space and 3D WebViews).
- Added window to configure network placeholder IDs.
- `SpawnAddressable` placeholder for spawning addressable assets.
- `InteractableOwnershipPlaceholder` for ownership management of an interactable object.
- `HelpSpawnObjectPlaceholder` which allows for spawning an object on help system calls.
- `GoToPreviousEventButtonAdder` which allows to add a button to the menu that teleports the player to the previous event.
- `GoToPreviousEventOnCollisionPlaceholder` which allows to create a teleport point to the previous event.
- `MascotteNameSetCheckPlaceholder` which allows to check if the mascotte name has been set at scene start. If not, it calls the help at start.
- `RPMAvatarWebViewButtonPlaceholder` which allows to open the `ReadyPlayerMeWebView`.

### Deprecated

- Environmental player List prefab
- Environmental tutorial prefab

### Removed

- Legacy `BigScreen` prefab and placeholder

## v1.0.0

Initial release
