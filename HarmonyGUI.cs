using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using System.Collections;
using Il2Cpp;
using UnityEngine.UI;
using Il2CppInterop.Runtime;
using UnityEngine.Events;
using HarmonyLib;
using static Il2Cpp.BasicMenu;
using Il2CppTLD.Placement;
using Il2CppTLD.Gameplay;

namespace ChangeCustomSettings
{
    //private const int MOD_SETTINGS_ID = 0x4d53; // "MS" in hex


    [HarmonyPatch(typeof(Panel_OptionsMenu), "ConfigureMenu", new Type[0])]
    public static class AddModSettingsButton
    {
        private static void Postfix(Panel_OptionsMenu __instance)
        {
            BasicMenu basicMenu = __instance.m_BasicMenu;
            if (basicMenu == null)
                return;

            AddAnotherMenuItem(basicMenu, __instance); // We need one more than they have...
            BasicMenu.BasicMenuItemModel firstItem = basicMenu.m_ItemModelList[0];
            int itemIndex = basicMenu.GetItemCount();

            foreach (BasicMenuItemModel item in basicMenu.m_ItemModelList)
            {
                if (item.m_Id == "DifficultySettings")
                {
                    return;
                }
            }

            basicMenu.AddItem("DifficultySettings", 0xDEAD, itemIndex, "Difficulty Settings", "Change custom difficulty of current game", null,
                    new Action(() => GUI.OpenCustomSettingsPanel()), firstItem.m_NormalTint, firstItem.m_HighlightTint);
        }


        private static void AddAnotherMenuItem(BasicMenu basicMenu, Panel_OptionsMenu optionsPanel)
        {
            GameObject menuItem = GameObject.Find("SCRIPT_InterfaceManager/_GUI_Common/Camera/Anchor/Panel_OptionsMenu/Pages/MainTab/MenuRoot2/Menu/Left_Align/Grid/CustomSettings MenuItem");
           
            if (menuItem != null)
            {
                return;
            }
            
            GameObject newButtonBackground = GameObject.Instantiate(optionsPanel.m_MainMenuItem_PrivacyBackground);
            newButtonBackground.name = "DifficultySettingsBackground";
            optionsPanel.m_MainMenuItemsBackground.Add(newButtonBackground);
            newButtonBackground.SetActive(false);            

            GameObject gameObject = NGUITools.AddChild(basicMenu.m_MenuGrid.gameObject, basicMenu.m_BasicMenuItemPrefab);
            gameObject.name = "DifficultySettings MenuItem";
            BasicMenuItem item = gameObject.GetComponent<BasicMenuItem>();
            BasicMenu.BasicMenuItemView view = item.m_View;
            int itemIndex = basicMenu.m_MenuItems.Count;
            EventDelegate onClick = new EventDelegate(new Action(() => basicMenu.OnItemClicked(itemIndex)));
            view.m_Button.onClick.Add(onClick);
            EventDelegate onDoubleClick = new EventDelegate(new Action(() => basicMenu.OnItemDoubleClicked(itemIndex)));
            view.m_DoubleClickButton.m_OnDoubleClick.Add(onDoubleClick);
            basicMenu.m_MenuItems.Add(view);
        }
    }


    [HarmonyPatch(typeof(Panel_CustomXPSetup), "OnCancel", new Type[0])]
    public static class OnCancelPatch
    {
        public static bool Prefix(Panel_CustomXPSetup __instance)
        {            
            if(GUI.openedFromSettingsMenu)
            {
                if (__instance.m_ShareExperiencePopupObj.activeInHierarchy)
                {                    
                    return true;
                }
                else
                {
                    __instance.GetComponent<Panel_CustomXPSetup>().SetCustomModeString(GUI.cachedDifficultyString);

                    GameManager.GetExperienceModeManagerComponent().SetCurrentCustomModeString(GUI.cachedDifficultyString);
                    GUI.customSettingsPanel.GetComponent<Panel_CustomXPSetup>().SetUIFromCurrentCustomXP();
                    GUI.SwitchCustomSettingsPanel(false);
                }

               
                return false; // Skip the original method
            }
            else
            {
                return true; // Continue with the original method
            }

        }
    }

    [HarmonyPatch(typeof(Panel_CustomXPSetup), "OnContinue", new Type[0])]
    public static class OnContinuePatch
    {
        public static bool Prefix(Panel_CustomXPSetup __instance)
        {
            if (GUI.openedFromSettingsMenu)
            {              
                CustomSetting.SetGameModeConfig(__instance);

                GameManager.GetExperienceModeManagerComponent().SetCurrentCustomModeString(GUI.cachedDifficultyString);
                GUI.customSettingsPanel.GetComponent<Panel_CustomXPSetup>().SetUIFromCurrentCustomXP();
                return false; // Skip the original method
            }
            else
            {
                return true; // Continue with the original method
            }

        }
    }

    [HarmonyPatch(typeof(Panel_OptionsMenu), "Initialize", new Type[0])]
    public class Panel_OptionsMenuInitPatch
    {
        public static void Postfix(ref Panel_OptionsMenu __instance)
        {
            GUI.optionsPanel = __instance.gameObject;
        }
    }   

    [HarmonyLib.HarmonyPatch(typeof(Panel_OptionsMenu), "Enable", new Type[] { typeof(bool) })]
    public class EnablePatch1
    {
        public static void Prefix(ref Panel_OptionsMenu __instance, ref bool enable)
        {
            GameManager.GetExperienceModeManagerComponent().SetCurrentCustomModeString(GUI.cachedDifficultyString);
            GUI.customSettingsPanel.GetComponent<Panel_CustomXPSetup>().SetUIFromCurrentCustomXP();



            for (int i = 0; i < __instance.m_MainMenuItemsBackground.Count; i++)
            {
                if (__instance.m_MainMenuItemsBackground[i] == null)
                {
                    __instance.m_MainMenuItemsBackground[i] = __instance.m_MainMenuItemsBackground[0];
                }
            }
        }
    }    
}