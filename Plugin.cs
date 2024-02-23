using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace Dash
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        private const string modGUID = "Niro.Dash";
        private const string modName = "Dash";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin instance;

        static internal ManualLogSource mls;

        public static ConfigFile BepInExConfig() { return instance.Config; }

        void Awake()
        {
            // entry point when mod load
            if ( instance == null)
                instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Dash.Config.Instance.Setup();

            mls.LogMessage("Plugin " + modName + " loaded!");

            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }
}
