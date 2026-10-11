using MelonLoader;
using HarmonyLib;
using Il2Cpp;

namespace ChangeCustomSettings
{
    // Adds a "Save Game" entry to the Esc pause menu (can be turned off in Mod Settings)
    [HarmonyPatch(typeof(Panel_PauseMenu), "ConfigureMenu")]
    public static class PauseMenuSavePatch
    {
        private const string ItemId = "CCSSaveGame";

        public static void Postfix(Panel_PauseMenu __instance)
        {
            if (!Settings.SaveGameButton)
            {
                return;
            }

            BasicMenu menu = __instance.m_BasicMenu;
            if (menu == null || menu.GetItemCount() == 0)
            {
                return;
            }

            foreach (BasicMenu.BasicMenuItemModel item in menu.m_ItemModelList)
            {
                if (item.m_Id == ItemId)
                {
                    return;
                }
            }

            BasicMenu.BasicMenuItemModel first = menu.m_ItemModelList[0];
            menu.AddItem(ItemId, 0xDEAE, menu.GetItemCount(), "Save Game", "Save your game right now", null,
                new Action(SaveNow), first.m_NormalTint, first.m_HighlightTint);
        }

        private static void SaveNow()
        {
            if (GameManager.IsMainMenuActive() || !Settings.SaveGameButton)
            {
                return;
            }
            GameManager.SaveGameAndDisplayHUDMessage();
            MelonLogger.Msg("Saved from the pause menu.");
        }
    }
}
