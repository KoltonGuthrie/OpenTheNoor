using GameNetcodeStuff;
using HarmonyLib;
using OpenTheNoor.Managers;
using System;
using UnityEngine;

namespace OpenTheNoor.Config
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
                    if (__door.isLocked)
                    {
                        if (Config.Instance.PLAY_FOR_ALL_PLAYERS)
                        {
                            NetworkManagerOpenTheNoor.Instance.MakeOpenTheNoorNoiseServerRpc(__door);
                        } else
                        {
                            AudioSource audioSource = __door.doorLockSFX;

                            if (audioSource != null)
                            {
                                playOpenTheNoorNoise(audioSource, OpenTheNoorBase.SoundFX.ToArray());
                            }

                        }

                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PlayerControllerB), "ConnectClientToPlayerObject")]
        public static void InitializeLocalPlayer()
        {
            if (Config.IsHost)
            {
                Config.MessageManager.RegisterNamedMessageHandler("ModName_OnRequestConfigSync", Config.OnRequestSync);
                Config.Synced = true;

                return;
            }

            Config.Synced = false;
            Config.MessageManager.RegisterNamedMessageHandler("ModName_OnReceiveConfigSync", Config.OnReceiveSync);
            Config.RequestSync();
        }

        public static void playOpenTheNoorNoise(AudioSource __audioSource, AudioClip[] sounds)
        {
            RoundManager.PlayRandomClip(__audioSource, sounds, true, Config.Default.VOLUME);
        }
    }
}
