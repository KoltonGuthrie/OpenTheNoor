using BepInEx;
using BepInEx.Logging;
using OpenTheNoor.Patches;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OpenTheNoor.Managers;
using System.Reflection;
using BepInEx.Configuration;

namespace OpenTheNoor
{

    [BepInPlugin(modGUID, modName, modVersion)]
    public class OpenTheNoorBase : BaseUnityPlugin
    {
        private const string modGUID = "Kolton12O.OpenTheNoor";
        private const string modName = "OpenTheNoor";
        private const string modVersion = "1.1.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        public static OpenTheNoorBase Instance;

        internal ManualLogSource mls;

        internal static List<AudioClip> SoundFX;
        internal static AssetBundle Bundle;

        public GameObject netManagerPrefab;

        public ConfigEntry<float> volume;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }

            loadConfig();

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            mls.LogInfo("Open The Noor has awaken!");

            string FolderLocation = Instance.Info.Location;
            FolderLocation = FolderLocation.TrimEnd((modName + ".dll").ToCharArray());
            Bundle = AssetBundle.LoadFromFile(FolderLocation + "openthenoor");

            SoundFX = new List<AudioClip>();

            if (Bundle != null)
            {
                mls.LogInfo("Successfully loaded asset bundle! :)");
                netManagerPrefab = Bundle.LoadAsset<GameObject>("Assets/OpenTheNoor/NetworkManagerOpenTheNoor.prefab");
                netManagerPrefab.AddComponent<NetworkManagerOpenTheNoor>();
                SoundFX = Bundle.LoadAllAssets<AudioClip>().ToList();
            }
            else
            {
                mls.LogError("Failed to load asset bundle! :(");
            }

            harmony.PatchAll();

        }

        void loadConfig()
        {
            volume = Config.Bind<float>("General", "volume", 0.5f, "The volume that the sound will play at for you.");
        }
    }
}
