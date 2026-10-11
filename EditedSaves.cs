using MelonLoader;
using Il2Cpp;
using Il2CppTLD.Gameplay;

namespace ChangeCustomSettings
{
    // Keeps edited settings on the save they were made for.
    //
    // The save file itself keeps its original mode type (e.g. "Stalker"), so on load the game
    // rebuilds Stalker settings and the edit would be lost. We remember the custom string per save
    // in UserData/ChangeCustomSettings_saves.txt and re-apply it after the save has loaded.
    //
    // Only the m_XPMode of the current GameModeConfig is swapped (never s_CurrentGameMode itself),
    // and it is restored when returning to the main menu, so new games on a preset stay untouched.
    public static class EditedSaves
    {
        private static readonly string StoreFile = Path.Combine(MelonLoader.Utils.MelonEnvironment.UserDataDirectory, "ChangeCustomSettings_saves.txt");

        private static GameModeConfig? modifiedConfig;
        private static ExperienceMode? originalXPMode;

        public static bool IsEdited(GameModeConfig? config)
        {
            return modifiedConfig != null && config != null && modifiedConfig == config;
        }

        public static ExperienceModeType OriginalModeType()
        {
            return originalXPMode != null ? originalXPMode.m_ModeType : ExperienceModeType.Custom;
        }

        public static void ApplyCustomString(string str)
        {
            ExperienceModeManager emm = GameManager.GetExperienceModeManagerComponent();
            emm.SetCurrentCustomModeString(str);

            CustomExperienceMode custom = GameManager.GetCustomMode();
            CustomExperienceMode.CreateCustomModeFromString(custom, str);

            GameModeConfig config = ExperienceModeManager.s_CurrentGameMode;
            if (config != null)
            {
                if (modifiedConfig == null)
                {
                    modifiedConfig = config;
                    originalXPMode = config.m_XPMode;
                }
                config.m_XPMode = custom;
            }

            GUI.cachedDifficultyString = str;
        }

        // Called after a save has finished loading (and after every region change)
        public static void OnGameLoaded()
        {
            if (GameManager.IsMainMenuActive() || IsEdited(ExperienceModeManager.s_CurrentGameMode))
            {
                return;
            }

            string saveName = SaveGameSystem.GetCurrentSaveName();
            string? stored = Lookup(saveName);
            if (stored == null)
            {
                return;
            }

            // The save stores the custom string too - if it doesn't match, this save slot now holds
            // a different game than the one that was edited, so leave it alone.
            string inSave = StringUtils.RemoveDashes(GameManager.GetExperienceModeManagerComponent().GetCurrentCustomModeString());
            if (inSave != stored)
            {
                MelonLogger.Msg("Save " + saveName + " is a different game than the one edited before - leaving it alone.");
                Remember(saveName, null);
                return;
            }

            ApplyCustomString(stored);
            MelonLogger.Msg("Re-applied edited settings for " + saveName + ": " + StringUtils.AddDashes(stored));
        }

        public static void OnSceneLoaded(string sceneName)
        {
            if (sceneName != null && sceneName.Contains("MainMenu") && modifiedConfig != null)
            {
                modifiedConfig.m_XPMode = originalXPMode;
                modifiedConfig = null;
                originalXPMode = null;
                MelonLogger.Msg("Restored the original difficulty preset for new games.");
            }
        }

        // ---- tiny store: one line per save, "<saveName>=<customString>"
        private static string? Lookup(string saveName)
        {
            if (saveName == null || !File.Exists(StoreFile))
            {
                return null;
            }

            foreach (string line in File.ReadAllLines(StoreFile))
            {
                int eq = line.IndexOf('=');
                if (eq > 0 && line.Substring(0, eq) == saveName)
                {
                    return line.Substring(eq + 1);
                }
            }
            return null;
        }

        public static void Remember(string saveName, string? str)
        {
            if (saveName == null)
            {
                return;
            }

            string text = "";
            if (File.Exists(StoreFile))
            {
                foreach (string line in File.ReadAllLines(StoreFile))
                {
                    int eq = line.IndexOf('=');
                    if (eq > 0 && line.Substring(0, eq) != saveName)
                    {
                        text += line + "\n";
                    }
                }
            }
            if (str != null)
            {
                text += saveName + "=" + str + "\n";
            }
            File.WriteAllText(StoreFile, text);
        }
    }
}
