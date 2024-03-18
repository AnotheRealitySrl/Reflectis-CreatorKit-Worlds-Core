# Release notes

## Unreleased

### Changed

- InteractablePlaceholder: Now accepts script machines instead of scriptable actions to define custom logic

### Added

- new VisualScriptingNodes: Added Block interactable behaviours block by selection, pan character camera, disable character mesh, teleport player, open close tutorial

### Deprecated

### Removed

- Removed awaitable scriptable actions

### Fixed


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
