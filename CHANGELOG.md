# Release notes

## v2.0.0

### Changed

- Massive SPACS -> Reflectis refactor
- Reorganized package structure
- Reimplemented `BigScreen` placeholder (with prefab) for an easier configuration and improved flexibility
- Reimplemented `TeleportPoint` logic (now `SceneChanger`)
- Reimplemented `ScriptableActions` to support async/await paradigm, and to support the selected interactable as a parameter of the action

### Added

- `INetworkedPlaceholder` interface to distinguish network placeholder from local one
- Environmental Dashboard prefab with placeholder
- `InteractionPlaceholder` which supports the interaction provided by the `SDK.Interaction` module
- Add visual scripting modules
- WebView placeholders (both for screen-space and 3D WebViews)
- Added window for configure network placeholder IDs
- `SpawnAddressable` placeholder for spawning addressable assets
- `InteractableOwnershipPlaceholder` for ownership management
- `HelpSpawnObjectPlaceholder`
- `GoToPreviousEventButtonAdder`
- `GoToPreviousEventOnCollisionPlaceholder`
- `MascotteNameSetCheckPlaceholder`
- `RPMAvatarWebViewButtonPlaceholder`

### Deprecated

- Environmental player List prefab
- Environmental tutorial prefab

### Removed

- Legacy `BigScreen` prefab and placeholder

## v1.0.0

Initial release
