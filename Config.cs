using Newtonsoft.Json;
using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using static Unity.Netcode.CustomMessagingManager;

namespace Dash
{
    public sealed class Config
    {
        #region Properties

        public ConfigEntry<bool> Enabled { get; set; }
        public ConfigEntry<float> StaminaCost { get; set; }
        public ConfigEntry<float> Cooldown { get; set; }
        public ConfigEntry<float> Precision { get; set; }
        public ConfigEntry<float> Power { get; set; }
        public ConfigEntry<float> Speed { get; set; }
        public ConfigEntry<float> FromSize { get; set; }
        public ConfigEntry<float> ToSize { get; set; }

        private static Config instance = null;
        public static Config Instance
        {
            get
            {
                if (instance == null)
                    instance = new Config();

                return instance;
            }
        }
        #endregion

        public void Setup()
        {
            Enabled = Plugin.BepInExConfig().Bind("General", "Enabled", true, "Enable / Disable this mod (Syncing with host!)");
            StaminaCost = Plugin.BepInExConfig().Bind("General", "StaminaCost", 0.15f, new ConfigDescription("Stamina cost for one dash.", new AcceptableValueRange<float>(0f, 1f)));
            Cooldown = Plugin.BepInExConfig().Bind("General", "Cooldown", 0.75f, new ConfigDescription("Cooldown in seconds.", new AcceptableValueRange<float>(0f, 2f)));
            Precision = Plugin.BepInExConfig().Bind("General", "Precision", 0.2f, new ConfigDescription("Time in which a key of the double-tap has to follow to the previous one. Higher = Easier.", new AcceptableValueRange<float>(0.1f, 0.5f)));
            Power = Plugin.BepInExConfig().Bind("General", "Power", 15f, new ConfigDescription("The power of a dash.", new AcceptableValueRange<float>(1f, 100f)));
            Speed = Plugin.BepInExConfig().Bind("General", "Speed", 0.4f, new ConfigDescription("The duration / speed of the dash.", new AcceptableValueRange<float>(0.1f, 1f)));
            FromSize = Plugin.BepInExConfig().Bind("General", "FromSize", 0f, "Player minimum size to be able to dash (value included). Default player size is 1.");
            ToSize = Plugin.BepInExConfig().Bind("General", "ToSize", 0f, "Player maximum size to be able to dash (value included). Default player size is 1. Use 0 to enable dashing for any size.");
        }

        [HarmonyPatch]
        public class SyncLimitations
        {
            #region Constants
            private const string DISABLED_RECEIVE = Plugin.modGUID + "_" + "DisabledReceive";
            private const string DISABLED_REQUEST = Plugin.modGUID + "_" + "DisabledRequest";
            #endregion

            #region Networking
            [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
            [HarmonyPostfix]
            public static void Initialize()
            {
                if (NetworkManager.Singleton.IsServer || NetworkManager.Singleton.IsHost)
                {
                    NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(DISABLED_REQUEST, new HandleNamedMessageDelegate(DisabledRequest));
                }
                else
                {
                    NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(DISABLED_RECEIVE, (ulong clientId, FastBufferReader reader) => { Instance.Enabled.Value = false; Plugin.mls.LogMessage("Disabled by host."); });
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(DISABLED_REQUEST, 0uL, new FastBufferWriter(0, Allocator.Temp), NetworkDelivery.ReliableSequenced);
                }
            }

            public static void DisabledRequest(ulong clientId, FastBufferReader reader)
            {
                if(!Instance.Enabled.Value)
                    NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(DISABLED_RECEIVE, clientId, new FastBufferWriter(0, Allocator.Temp), NetworkDelivery.ReliableSequenced);
            }
            #endregion
        }
    }
}

