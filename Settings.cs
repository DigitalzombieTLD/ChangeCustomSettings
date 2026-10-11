using MelonLoader;
using System.Runtime.CompilerServices;

namespace ChangeCustomSettings
{
    // ModSettings is optional: with it installed you get an on/off switch for the pause-menu
    // Save Game button; without it the button is simply always on.
    internal static class Settings
    {
        public static bool SaveGameButton = true;

        public static void OnLoad()
        {
            try
            {
                RegisterModSettings();
            }
            catch (Exception e)
            {
                MelonLogger.Msg("ModSettings not available (" + e.GetType().Name + ") - the Save Game button is always on.");
            }
        }

        // Kept in its own method so the ModSettings types are only touched when it is actually loaded.
        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void RegisterModSettings()
        {
            ChangeCustomSettingsSettings options = new ChangeCustomSettingsSettings();
            options.AddToModSettings("Change Custom Settings");
            SaveGameButton = options.saveGameButton;
        }
    }

    internal class ChangeCustomSettingsSettings : ModSettings.JsonModSettings
    {
        [ModSettings.Name("Save Game button in pause menu")]
        [ModSettings.Description("Show a Save Game button in the Esc pause menu that saves your game anywhere, indoors or outdoors.")]
        public bool saveGameButton = true;

        protected override void OnConfirm()
        {
            base.OnConfirm();
            Settings.SaveGameButton = saveGameButton;
        }
    }
}
