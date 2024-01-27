using OpenTheNoor.Patches;
using Unity.Netcode;
using UnityEngine;

namespace OpenTheNoor.Managers
{
    public class NetworkManagerOpenTheNoor : NetworkBehaviour
    {
        public static NetworkManagerOpenTheNoor Instance;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void MakeOpenTheNoorNoiseServerRpc(NetworkBehaviourReference __door)
        {
            MakeOpenTheNoorNoiseClientRpc(__door);
        }

        [ClientRpc]
        public void MakeOpenTheNoorNoiseClientRpc(NetworkBehaviourReference __door)
        {
            AudioSource audioSource = ((DoorLock)__door).doorLockSFX;

            if (audioSource != null)
            {
                PlayerControllerBPatch.playOpenTheNoorNoise(audioSource, OpenTheNoorBase.SoundFX.ToArray());
            }

        }
    }
}
