using HarmonyLib;
using Unity.Netcode;

namespace OpenTheNoor.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void AddToPrefabs(ref GameNetworkManager __instance)
        {
            __instance.GetComponent<NetworkManager>().AddNetworkPrefab(OpenTheNoorBase.Instance.netManagerPrefab);
        }
    }
}