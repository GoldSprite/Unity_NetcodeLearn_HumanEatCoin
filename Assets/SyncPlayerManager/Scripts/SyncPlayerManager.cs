using GoldSprite.LobbyRoomUI;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using Unity.Sync.Relay.Model;
using Unity.Sync.Relay.Transport.Netcode;
using UnityEngine;
using static GoldSprite.MySyncPlayerManager.NetworkGameManager;

namespace GoldSprite.MySyncPlayerManager
{
    public class SyncPlayerManager : NetworkBehaviour
    {
        [SerializeField]
        private GameObject playerRole;
        private NetworkGameManager nwManager => NetworkGameManager.Instance;
        private NetworkManager networkManager;
        private RelayTransportNetcode netTrans;
        private RelayRoom relayRoom;
        private RelayPlayer localPlayer;
        private string playerName => localPlayer.Name;
        private bool isLocalPlayer => IsLocalPlayer;

        private NetworkVariable<uint> syncTransportId = new();


        private void Start()
        {
            networkManager = NetworkManager.Singleton;
            netTrans = networkManager.GetComponent<RelayTransportNetcode>();
            relayRoom = netTrans.GetRoomInfo();

            name = "SyncPlayerManager";
            //DontDestroyOnLoad(gameObject);
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            transform.SetParent(nwManager.GameElementsParent);

            NetworkGameManager.StartGameEvent += () =>
            {
                if (IsOwner)
                {
                    if (IsServer)
                    {
                        localPlayer.Properties[GameStatu.Key] = GameStatu.ToInitial;
                        //sendProps_ClientRPC(IsServer, localPlayer.TransportId, )
                    }
                }
            };
        }


        private void Update()
        {
            try
            {
                localPlayer = netTrans.GetCurrentPlayer();
                HandleSelfPlayerEvent();

                if (IsLocalPlayer)
                {
                    if (IsServer)
                    {
                        sendProps_ClientRPC(IsServer, localPlayer.TransportId, EncodeProps(localPlayer.Properties));
                    }
                    else
                    {
                        sendProps_ServerRPC(IsServer, localPlayer.TransportId, EncodeProps(localPlayer.Properties));
                    }
                }
            }
            catch (Exception)
            {
                //Debug.Log("未加入房间, 无CurrentPlayer");
            }

        }

        private void HandleSelfPlayerEvent()
        {
            if(syncTransportId.Value == 0)
            {
                Debug.Log("playerRole还未初始化.");
            }
            else
            {
                var player = nwManager.roomPlayers[syncTransportId.Value];
                var props = player.Properties;
                var gameStatu = props[GameStatu.Key];
                if (gameStatu == GameStatu.ToInitial)  //游戏角色待初始化
                {
                    props[GameStatu.Key] = GameStatu.Gaming;

                    playerRole.SetActive(true);
                    if (IsLocalPlayer)
                        LobbyRoomManager.Instance.CGGameWindow();
                    Debug.Log($"{(IsServer ? "主机" : "客户端")} 玩家[{syncTransportId.Value}]-{player.Name}, 初始化游戏角色完成, GameStatu: {props[GameStatu.Key]}.");
                }
            }
        }

        [ServerRpc]
        //客户端数据上传到服务端
        public void sendProps_ServerRPC(bool fromServer, uint transportId, ContainerKVPair[] encodeProps)
        {
            SyncPlayerDataRpc(fromServer, transportId, DecodePlayerProperties(encodeProps));
        }
        [ClientRpc]
        //服务端数据下放到每个客户端(包括Host)
        public void sendProps_ClientRPC(bool fromServer, uint transportId, ContainerKVPair[] encodeProps)
        {
            //奇怪的localPlayer-null报错
            if (localPlayer == null)
            {
                Debug.Log("SyncPlayerProperties_ClientRPC出错: localPlayer不存在.");
                return;
            }
            //if (transportId == localPlayer.TransportId) return;

            SyncPlayerDataRpc(fromServer, transportId, DecodePlayerProperties(encodeProps));
        }


        private void SyncPlayerDataRpc(bool fromServer, uint transportId, Dictionary<string, string> props)
        {
            if (!relayRoom.Players.ContainsKey(transportId))
            {
                Debug.Log("HandleSyncPlayerData出错: 玩家不存在(可能离线).");
            }
            else
            {
                syncTransportId.Value = transportId;  //初始化playerRole-TransportId关联
                var player = relayRoom.GetPlayer(transportId);
                player.Properties = props;

                Debug.Log($"{(IsServer ? "主机" : "客户端")} 接收到来自{(fromServer ? "主机" : "客户端")}玩家[{syncTransportId.Value}]-{player.Name}数据, GameStatu: {props[GameStatu.Key]}.");
            }
        }


        private ContainerKVPair[] EncodeProps(Dictionary<string, string> properties)
        {
            var kvPairs = new ContainerKVPair[properties.Count];
            var i = 0;
            foreach (var (k, v) in properties) kvPairs[i++] = new ContainerKVPair() { key = k, val = v };

            return kvPairs;
        }
        private Dictionary<string, string> DecodePlayerProperties(ContainerKVPair[] kvPairs)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var kvPair in kvPairs)
            {
                dictionary[kvPair.key] = kvPair.val;
            }
            return dictionary;
        }


        private void HandlePropEvents(uint transportId, Dictionary<string, string> props)
        {
            var gameStatu = props[GameStatu.Key];
            if (gameStatu == GameStatu.ToInitial)
            {
                props[GameStatu.Key] = GameStatu.Gaming;

                LobbyRoomManager.Instance.CGGameWindow();

                //Debug.Log($"{(IsServer ? "主机" : "客户端")} 接收到来自{(fromServer ? "主机" : "客户端")}玩家{player.PlayerName}数据, GameStatu: {props[GameStatu.Key]}.");

                nwManager.InitPlayer(transportId);
                //if (IsServer) nwManager.InitPlayer(transportId);
                //else InitPlayer_ClientRpc(transportId);
            }
        }


        [ClientRpc]
        private void InitPlayer_ClientRpc(uint transportId)
        {
            nwManager.InitPlayer(transportId);
        }

        [ServerRpc]
        private void InitPlayer_ServerRPC(uint transportId)
        {
            nwManager.InitPlayer(transportId);
        }


        public class ContainerKVPair : INetworkSerializable
        {
            public string key;
            public string val;
            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                if (serializer.IsWriter)
                {
                    serializer.GetFastBufferWriter().WriteValueSafe(key);
                    serializer.GetFastBufferWriter().WriteValueSafe(val);
                }
                else
                {
                    serializer.GetFastBufferReader().ReadValueSafe(out key);
                    serializer.GetFastBufferReader().ReadValueSafe(out val);
                }
            }
        }
    }

}