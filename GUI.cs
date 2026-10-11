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
	public static class GUI
	{
        public static GameObject customSettingsPanel;
        public static GameObject optionsPanel;

        public static bool menuButtonAdded = false;
        public static string buttonId = "DifficultySettings";
        public static int buttonValue = 0xDEAD;
        public static bool foundButton = false;
        public static bool openedFromSettingsMenu = false;

        public static string cachedDifficultyString;


        // The custom code the panel showed when it was opened - used to detect "no changes"
        public static string? openedWithString;

        public static void SwitchCustomSettingsPanel(bool enable)
        {
            Panel_CustomXPSetup panel = customSettingsPanel.GetComponent<Panel_CustomXPSetup>();

            if (enable)
            {
                ExperienceModeManager emm = GameManager.GetExperienceModeManagerComponent();
                GameModeConfig config = ExperienceModeManager.s_CurrentGameMode;
                bool alreadyEdited = EditedSaves.IsEdited(config);

                // A save started on a preset (Pilgrim/Voyageur/Stalker/Interloper) does not have a
                // meaningful custom string yet - GetCurrentCustomModeString() returns leftovers from the
                // last custom game. Select the save's preset so the panel fills in the real values.
                ExperienceModeType type = alreadyEdited ? EditedSaves.OriginalModeType()
                    : (config != null && config.m_XPMode != null ? config.m_XPMode.m_ModeType : ExperienceModeType.Custom);
                int presetIndex = PresetIndex(type);

                if (!alreadyEdited && presetIndex >= 0 && panel.m_XPModePopupList != null)
                {
                    panel.m_XPModePopupList.m_CurrentIndex = presetIndex;
                    panel.m_XPModePopupList.Refresh();
                    panel.SetValuesToMatchXPMode();
                    MelonLogger.Msg("Opened with this save's " + type + " settings.");
                }
                else
                {
                    panel.SetCustomModeString(emm.GetCurrentCustomModeString());
                    panel.SetUIFromCurrentCustomXP();
                    MelonLogger.Msg("Opened with this save's custom settings.");
                }

                cachedDifficultyString = emm.GetCurrentCustomModeString();
                openedWithString = StringUtils.RemoveDashes(cachedDifficultyString);

                // The preset picker only makes sense when starting a new game - hide it.
                // (The "base difficulty" picker for loot stays visible.)
                if (panel.m_XPModePopupList != null)
                {
                    panel.m_XPModePopupList.gameObject.SetActive(false);
                }
            }

            openedFromSettingsMenu = enable;
            customSettingsPanel.SetActive(enable);
            optionsPanel.SetActive(!enable);
        }

        private static int PresetIndex(ExperienceModeType type)
        {
            if (type == ExperienceModeType.Pilgrim) return 0;
            if (type == ExperienceModeType.Voyageur) return 1;
            if (type == ExperienceModeType.Stalker) return 2;
            if (type == ExperienceModeType.Interloper) return 3;
            return -1;
        }

        public static void OpenCustomSettingsPanel()
        {
            MelonLogger.Msg("Current difficulty setting: " + StringUtils.AddDashes(GUI.cachedDifficultyString));
            SwitchCustomSettingsPanel(true);
        }
    }
}