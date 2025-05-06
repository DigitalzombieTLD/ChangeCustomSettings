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


        public static void SwitchCustomSettingsPanel(bool enable)
        {
            if (enable)
            {
                GUI.cachedDifficultyString = GameManager.GetExperienceModeManagerComponent().GetCurrentCustomModeString();

                customSettingsPanel.GetComponent<Panel_CustomXPSetup>().SetCustomModeString(GUI.cachedDifficultyString);
                //customSettingsPanel.GetComponent<Panel_CustomXPSetup>().SetUIFromCurrentCustomXP();
                customSettingsPanel.GetComponent<Panel_CustomXPSetup>().SetValuesToMatchXPMode(); //
            }

            GUI.openedFromSettingsMenu = enable;
            customSettingsPanel.SetActive(enable);
            optionsPanel.SetActive(!enable);

            //GameManager.GetExperienceModeManagerComponent().SetCurrentCustomModeString(cachedDifficultyString);
            //customSettingsPanel.GetComponent<Panel_CustomXPSetup>().SetUIFromCurrentCustomXP();

            //customSettingsPanel.GetComponent<Panel_CustomXPSetup>().SetUIFromCurrentCustomXP();
        }

        public static void OpenCustomSettingsPanel()
        {
            MelonLogger.Msg("Current difficulty setting: " + StringUtils.AddDashes(GUI.cachedDifficultyString));
            SwitchCustomSettingsPanel(true);
        }
    }
}