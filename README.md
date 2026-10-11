# ChangeCustomSettings v1.2.0

- Lets you change the difficulty settings on the currently loaded savegame.
- Adds copy-to-clipboard button to the code sharing windows
	
- Difficulty settings can be found in the games settings menu (Options -> Difficulty Settings)

- Switches Loper/Stalker/etc mode to custom on change (no custom to loper/stalker/etc)

- The game is saved automatically when you press Confirm
- Might have unintended consequences

## Save Game button

- Adds a "Save Game" entry to the Esc pause menu so you can save anywhere.
- With ModSettings installed it can be turned off under Options -> Mod Settings -> Change Custom Settings.

## 1.2.0 - The Long Dark 2.55 (Unity 6) / MelonLoader 0.7.2

- Fixed: crash on startup ("Failed to decompress data for the AssetBundle 'Memory'") - the embedded
  asset bundle is now loaded from a file in UserData instead of from memory.
- Fixed: Confirm did nothing (`m_AvailableGameModes` no longer exists; Custom mode is now found via
  `GetAvailableGameModes()`).
- Fixed: game froze after saving (`s_CurrentGameMode` must not be replaced; only its `m_XPMode` is
  swapped, and restored again on the main menu so new games are unaffected).
- Fixed: the settings screen opened with leftover values instead of the save's real preset values.
- Fixed: changes were lost after quitting and reloading. Edited saves are remembered in
  `UserData/ChangeCustomSettings_saves.txt` and re-applied on load.
- Fixed: the copy button had no visible label (the game no longer ships TextMeshPro).
- Confirm without changing anything leaves the save untouched.
