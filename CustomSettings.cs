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

            // getting new string
            GUI.cachedDifficultyString = GameManager.GetExperienceModeManagerComponent().GetCurrentCustomModeString();
            //GameManager.GetExperienceModeManagerComponent().GetCurrentExperienceMode().m_ModeType = ExperienceModeType.Custom;
            //ExperienceModeManager.m_ExperienceModeManagerSaveDataProxy.CurrentModeType = ExperienceModeType.Custom;
            ExperienceModeManager.s_CurrentGameMode = ExperienceModeManager.Instance.m_AvailableGameModes[4];

            //SaveGameSlots.UpdateSlotGameMode(SaveGameSystem.GetCurrentSaveName(), GameManager.GetExperienceModeManagerComponent().GetCurrentExperienceMode().m_ModeType, ExperienceModeType.Custom);


            //////

            string str = StringUtils.RemoveDashes(GUI.cachedDifficultyString);
           
            ExperienceModeManager experienceModeManager = GameManager.GetExperienceModeManagerComponent();
            experienceModeManager.SetCurrentCustomModeString(str);           

            CustomExperienceMode cem = GameManager.GetCustomMode();      
            CustomExperienceMode.CreateCustomModeFromString(cem, str);
            GUI.customSettingsPanel.GetComponent<Panel_CustomXPSetup>().SetValuesToMatchXPMode(); //
            //customXPPanel.SetCustomXPSettingsFromUI();

            MelonLogger.Msg("New difficulty setting applied: " + StringUtils.AddDashes(str));
            MelonLogger.Msg("Saving the game and reloading is highly recommended!");
        }
    }
}