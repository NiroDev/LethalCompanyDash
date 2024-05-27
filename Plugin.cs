using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace Dash
{
    [BepInPlugin(modGUID, modName, modVersion)]
    [BepInDependency("com.rune580.LethalCompanyInputUtils", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "Niro.Dash";
        public const string modName = "Dash";
        public const string modVersion = "1.1.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static Plugin instance;

        static internal ManualLogSource mls;

        static internal DashHandler dashHandler = new DashHandler();

        public static ConfigFile BepInExConfig() { return instance.Config; }

        public void Awake()
        {
            // entry point when mod load
            if ( instance == null)
                instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);
            Dash.Config.Instance.Setup();

            mls.LogMessage("Plugin " + modName + " loaded!");

            harmony.PatchAll(typeof(Config.SyncLimitations));
            harmony.PatchAll(typeof(PlayerControllerBPatch));
        }
    }
}
