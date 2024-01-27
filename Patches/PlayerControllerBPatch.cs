using DunGen;
using GameNetcodeStuff;
using HarmonyLib;
using OpenTheNoor.Managers;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

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

        public static void playOpenTheNoorNoise(AudioSource __audioSource, AudioClip[] sounds, float volume = 0.5f)
        {
            RoundManager.PlayRandomClip(__audioSource, sounds, true, volume);
        }
    }
}
