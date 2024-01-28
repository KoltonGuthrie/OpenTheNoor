using BepInEx.Configuration;
using System;
using Unity.Collections;
using Unity.Netcode;

namespace OpenTheNoor.Config
{
    [Serializable]
    public class Config : SyncedInstance<Config>
    {
        private const float VOLUME_DEFAULT = 0.5f;
        private const bool PLAY_FOR_ALL_PLAYERS_DEFAULT = true;

        public float VOLUME;
        public bool PLAY_FOR_ALL_PLAYERS;

        public Config(ConfigFile cfg)
        {
            InitInstance(this);

            VOLUME = cfg.Bind<float>("ClientSide", "volume", VOLUME_DEFAULT, "The volume that the sound will play at for you.").Value;
            PLAY_FOR_ALL_PLAYERS = cfg.Bind<bool>("ServerSide", "playForAllPlayers", PLAY_FOR_ALL_PLAYERS_DEFAULT, "Play the sound for other players.\nThis will allow all players to hear the sound when another player attempts to open a door.").Value;

        }

        public static void RequestSync()
        {
            if (!IsClient) return;

            using FastBufferWriter stream = new(IntSize, Allocator.Temp);
            MessageManager.SendNamedMessage("ModName_OnRequestConfigSync", 0uL, stream);
        }

        public static void OnRequestSync(ulong clientId, FastBufferReader _)
        {
            if (!IsHost) return;

            OpenTheNoorBase.Instance.mls.LogInfo($"Config sync request received from client: {clientId}");

            byte[] array = SerializeToBytes(Instance);
            int value = array.Length;

            using FastBufferWriter stream = new(value + IntSize, Allocator.Temp);

            try
            {
                stream.WriteValueSafe(in value, default);
                stream.WriteBytesSafe(array);

                MessageManager.SendNamedMessage("ModName_OnReceiveConfigSync", clientId, stream);
            }
            catch (Exception e)
            {
                OpenTheNoorBase.Instance.mls.LogInfo($"Error occurred syncing config with client: {clientId}\n{e}");
            }
        }

        public static void OnReceiveSync(ulong _, FastBufferReader reader)
        {
            if (!reader.TryBeginRead(IntSize))
            {
                OpenTheNoorBase.Instance.mls.LogError("Config sync error: Could not begin reading buffer.");
                return;
            }

            reader.ReadValueSafe(out int val, default);
            if (!reader.TryBeginRead(val))
            {
                OpenTheNoorBase.Instance.mls.LogError("Config sync error: Host could not sync.");
                return;
            }

            byte[] data = new byte[val];
            reader.ReadBytesSafe(ref data, val);

            SyncInstance(data);

            OpenTheNoorBase.Instance.mls.LogInfo("Successfully synced config with host.");
        }

    }

}
