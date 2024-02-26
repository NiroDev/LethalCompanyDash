using GameNetcodeStuff;
using HarmonyLib;

namespace Dash
{
    [HarmonyPatch]
    internal class PlayerControllerBPatch
    {
        public static DashHandler dashHandler = new DashHandler();

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        public static void OnUpdate(PlayerControllerB __instance)
        {
            if (StartOfRound.Instance.localPlayerController == null)
                return; // Not initialized yet

            if (StartOfRound.Instance.localPlayerController.playerClientId != __instance.playerClientId)
                return; // Not us

            if (!Config.Instance.Enabled.Value)
                return;

            if (Config.Instance.ToSize.Value > 0f)
            {
                var playerSize = StartOfRound.Instance.localPlayerController.gameObject.transform.localScale.x;
                if (playerSize < Config.Instance.FromSize.Value || playerSize > Config.Instance.ToSize.Value)
                    return;
            }

            dashHandler.OnUpdate();
        }
    }
}
