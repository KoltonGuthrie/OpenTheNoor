using HarmonyLib;
using Unity.Netcode;
using UnityEngine;

namespace OpenTheNoor.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("Start")]
        static void spawnNetManager(StartOfRound __instance)
        {
            if(__instance.IsHost)
            {
                GameObject go = GameObject.Instantiate(OpenTheNoorBase.Instance.netManagerPrefab);
                go.GetComponent<NetworkObject>().Spawn();
            }
        }
    }
}
