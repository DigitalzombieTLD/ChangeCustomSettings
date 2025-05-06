using MelonLoader;
using UnityEngine;
using Il2CppInterop;
using Il2CppInterop.Runtime.Injection; 
using System.Collections;
using Il2Cpp;
using UnityEngine.UI;
using Il2CppInterop.Runtime;
using UnityEngine.Events;

namespace ChangeCustomSettings
{
    [HarmonyLib.HarmonyPatch(typeof(Panel_CustomXPSetup), "Initialize")]
    public class Panel_CustomXPSetupPatch
    {
        public static void Postfix(ref Panel_CustomXPSetup __instance)
        {
            CopyButton button = __instance.gameObject.GetComponent<CopyButton>();
            if (button == null)
            {
                button = __instance.gameObject.AddComponent<CopyButton>();
            }
            GUI.customSettingsPanel = __instance.gameObject;
        }
    }


    [HarmonyLib.HarmonyPatch(typeof(PlayerManager), "TeleportPlayerAfterSceneLoad")]
    public class TeleportPlayerAfterSceneLoadPatch
    {
        public static void Postfix(ref PlayerManager __instance)
        {
            GUI.cachedDifficultyString = GameManager.GetExperienceModeManagerComponent().GetCurrentCustomModeString();                       
        }
    }
}