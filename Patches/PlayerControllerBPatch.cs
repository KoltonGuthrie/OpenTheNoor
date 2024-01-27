using GameNetcodeStuff;
using HarmonyLib;
using OpenTheNoor.Managers;
using UnityEngine;

namespace OpenTheNoor.Patches
{

    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {

        [HarmonyPatch(typeof(PlayerControllerB), "Interact_performed")]
        [HarmonyPrefix]
        private static void onInteract(PlayerControllerB __instance)
        {
            if (__instance.hoveringOverTrigger != null)
            {
                DoorLock __door = __instance.hoveringOverTrigger.GetComponentInParent<DoorLock>();
                if (__door != null)
                {
                    if(__door.isLocked)
                    {
                        NetworkManagerOpenTheNoor.Instance.MakeOpenTheNoorNoiseServerRpc(__door);
                    }
                }
            }
        }

        public static void playOpenTheNoorNoise(AudioSource __audioSource, AudioClip[] sounds)
        {
            RoundManager.PlayRandomClip(__audioSource, sounds, true, OpenTheNoorBase.Instance.volume.Value);
        }
    }
}
