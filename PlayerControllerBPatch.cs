using GameNetcodeStuff;
using HarmonyLib;

namespace Dash
{
    [HarmonyPatch]
    internal class PlayerControllerBPatch
    {
        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        public static void OnUpdate(PlayerControllerB __instance)
        {
            if (InputUtilsCompat.Enabled && InputUtilsCompat.UseDashKey)
                return;

            if (StartOfRound.Instance.localPlayerController == null)
                return;

            if (StartOfRound.Instance.localPlayerController.playerClientId != __instance.playerClientId)
                return; // Not us

            Plugin.dashHandler.OnUpdate();
        }

        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        [HarmonyPostfix]
        public static void ConnectClientToPlayerObject()
        {
            Plugin.dashHandler.RegisterDashKey(); // Re-register in case the config changed
        }
    }
}
