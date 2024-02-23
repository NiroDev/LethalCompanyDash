using BepInEx.Configuration;
using UnityEngine;

namespace Dash
{
    public sealed class Config
    {
        #region Properties

        public float StaminaCost { get; set; }
        public float Cooldown { get; set; }
        public float Precision { get; set; }
        public float Power { get; set; }
        public float Speed { get; set; }

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
            StaminaCost = Plugin.BepInExConfig().Bind("General", "StaminaCost", 0.15f, new ConfigDescription("Stamina cost for one dash.", new AcceptableValueRange<float>(0f, 1f))).Value;
            Cooldown = Plugin.BepInExConfig().Bind("General", "Cooldown", 0.75f, new ConfigDescription("Cooldown in seconds.", new AcceptableValueRange<float>(0f, 2f))).Value;
            Precision = Plugin.BepInExConfig().Bind("General", "Precision", 0.2f, new ConfigDescription("Time in which a key of the double-tap has to follow to the previous one. Higher = Easier.", new AcceptableValueRange<float>(0.1f, 0.5f))).Value;
            Power = Plugin.BepInExConfig().Bind("General", "Power", 15f, new ConfigDescription("The power of a dash.", new AcceptableValueRange<float>(1f, 100f))).Value;
            Speed = Plugin.BepInExConfig().Bind("General", "Speed", 0.4f, new ConfigDescription("The duration / speed of the dash.", new AcceptableValueRange<float>(0.1f, 1f))).Value;
        }
    }
}

