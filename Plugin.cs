using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using OpenTheNoor.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace OpenTheNoor.Config
{

    [BepInPlugin(MOD_GUID, MOD_NAME, MOD_VERSION)]

    public class OpenTheNoorBase : BaseUnityPlugin
    {
        public const string MOD_GUID = "Kolton12O.OpenTheNoor";
        public const string MOD_NAME = "OpenTheNoor";
        public const string MOD_VERSION = "1.1.2";

        private readonly Harmony harmony = new Harmony(MOD_GUID);

        public static new Config Config { get; internal set; }

        public static OpenTheNoorBase Instance;

        internal ManualLogSource mls;
        internal static List<AudioClip> SoundFX;
        internal static AssetBundle Bundle;

        public GameObject netManagerPrefab;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(MOD_GUID);

            Config = new(base.Config);

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

            mls.LogInfo("Open The Noor has awaken!");

            string FolderLocation = Instance.Info.Location;
            FolderLocation = FolderLocation.TrimEnd((MOD_NAME + ".dll").ToCharArray());
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
    }
}
