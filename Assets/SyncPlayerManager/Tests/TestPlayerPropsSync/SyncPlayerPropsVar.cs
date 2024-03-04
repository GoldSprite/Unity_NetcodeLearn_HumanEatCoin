using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace GoldSprite.TestSyncTemp
{

    [Serializable]
    public class SyncPlayerPropsVar : INetworkSerializable
    {
        public uint TransportId = 0;
        public string PlayerName = "";
        public Vector3 PlayerPos = Vector3.zero;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                serializer.GetFastBufferReader().ReadValueSafe(out TransportId);
                serializer.GetFastBufferReader().ReadValueSafe(out PlayerName);
                serializer.GetFastBufferReader().ReadValueSafe(out PlayerPos);
            }
            else
            {
                serializer.GetFastBufferWriter().WriteValueSafe(TransportId);
                serializer.GetFastBufferWriter().WriteValueSafe(PlayerName);
                serializer.GetFastBufferWriter().WriteValueSafe(PlayerPos);
            }
        }
    }

    [Serializable]
    public class LocalPlayerProps
    {
        public uint TransportId = 0;
        public string PlayerName = "";
        public Vector3 PlayerPos = Vector3.zero;
    }

}
