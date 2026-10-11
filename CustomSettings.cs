using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection;
using System.Collections;
using Il2Cpp;
using System.Reflection;
using UnityEngine.EventSystems;
using Il2CppTLD.Gameplay;


namespace ChangeCustomSettings
{
	public static class CustomSetting
	{
       public static void SetGameModeConfig(Panel_CustomXPSetup customXPPanel)
       {
            GUI.SwitchCustomSettingsPanel(false);

            if (GameManager.IsMainMenuActive())
            {
                MelonLogger.Msg("Not in a game - nothing changed. Load a save to edit its settings.");
                return;
            }

            ExperienceModeManager experienceModeManager = GameManager.GetExperienceModeManagerComponent();
            string str = StringUtils.RemoveDashes(experienceModeManager.GetCurrentCustomModeString());

            if (str == GUI.openedWithString)
            {
                GUI.cachedDifficultyString = str;
                MelonLogger.Msg("No changes made - settings left as they were.");
                return;
            }

            // Note: s_CurrentGameMode is NOT replaced any more (the old code used
            // m_AvailableGameModes[4], which no longer exists in 2.55, and swapping the whole
            // GameModeConfig makes SaveShouldBePending() throw -> game freezes after saving).
            EditedSaves.ApplyCustomString(str);

            string saveName = SaveGameSystem.GetCurrentSaveName();
            EditedSaves.Remember(saveName, str);
            MelonLogger.Msg("New difficulty setting applied to " + saveName + ": " + StringUtils.AddDashes(str));

            GameManager.SaveGameAndDisplayHUDMessage();
        }
    }
}