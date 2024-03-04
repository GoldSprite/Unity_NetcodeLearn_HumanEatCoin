using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Netcode;
using Unity.Sync.Relay.Model;
using Unity.Sync.Relay.Transport.Netcode;
using UnityEngine;
using static GoldSprite.MySyncPlayerManager.NetworkGameManager;
using Random = UnityEngine.Random;

namespace GoldSprite.TestSyncTemp
{
    public class TestSyncPropsHandler : NetworkBehaviour
    {
        private NetworkManager networkManager;
        private RelayTransportNetcode netTrans;

        private RelayRoom relayRoom;
        private RelayPlayer localPlayer;
        private RelayPlayer selfPlayer;

        public bool isInit;

        public LocalPlayerProps selfProps = new LocalPlayerProps();
        private SyncPlayerPropsVar selfProps2 = new SyncPlayerPropsVar();

        private SyncPlayerPropsVar EncodeSelfProps(LocalPlayerProps selfProps)
        {
            selfProps2.TransportId = selfProps.TransportId;
            selfProps2.PlayerName = selfProps.PlayerName;
            selfProps2.PlayerPos = selfProps.PlayerPos;
            return selfProps2;
        }

        private LocalPlayerProps DecodeSyncProps(SyncPlayerPropsVar selfProps2)
        {
            selfProps.TransportId = selfProps2.TransportId;
            selfProps.PlayerName = selfProps2.PlayerName;
            selfProps.PlayerPos = selfProps2.PlayerPos;
            return selfProps;
        }

        private NetworkVariable<SyncPlayerPropsVar> syncProps = new();
        public NetworkVariable<SyncPlayerPropsVar> SyncProps => syncProps;


        private void Start()
        {
            networkManager = NetworkManager.Singleton;
            netTrans = networkManager.GetComponent<RelayTransportNetcode>();
            relayRoom = netTrans.GetRoomInfo();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            transform.position = new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
        }

        private void Update()
        {
            try
            {
                if (!isInit) TestManager.Tip_Txt.text = $"{(IsLocalPlayer ? "本机" : "其他玩家")}初始化房间中...";
                localPlayer = netTrans.GetCurrentPlayer();  //获取玩家在房间实例, 表示已加入房间
                if (syncProps.Value == null || syncProps.Value.TransportId == 0)  //表示还未关联transportId
                {
                    if (!isInit) TestManager.Tip_Txt.text = $"{(IsLocalPlayer ? "本机" : "其他玩家")}已加入房间, 初始化玩家Identifier中...";
                    if (IsLocalPlayer)
                    {
                        selfProps.TransportId = localPlayer.TransportId;
                        selfProps.PlayerName = localPlayer.Name;
                        selfProps.PlayerPos = transform.position;
                        UploadData();
                    }
                }
                else  //已完成transportId初始化, 开始数据更新
                {
                    if (!isInit)
                    {
                        isInit = true;
                        TestManager.Tip_Txt.text = $"{(IsLocalPlayer ? "本机" : "其他")}玩家Identifier初始化完成.";
                        if (IsLocalPlayer) TestManager.Instance.LocalPlayer = this;
                    }

                    //从服务端下载玩家数据
                    selfProps = DecodeSyncProps(syncProps.Value);
                    //应用玩家数据
                    transform.position = selfProps.PlayerPos;

                    //本地玩家自身数据上载服务端
                    UploadData();
                }

                //Debug.Log($"{(IsServer ? "主机" : "客户端")}-{(IsLocalPlayer ? "本地" : "非本地")}玩家数据: [SyncTID-{syncProps.Value.TransportId}, selfTID-{selfProps.TransportId}, ], [SyncName-{syncProps.Value.PlayerName}, selfName-{selfProps.PlayerName}, ], .");
                Debug.Log($"{(IsServer ? "主机" : "客户端")}-{(IsLocalPlayer ? "本地" : "非本地")}玩家数据: [SyncTID-{syncProps.Value.TransportId}], [SyncPos-{syncProps.Value.PlayerPos}].");

            }
            catch (Exception)
            {
                //Debug.Log("未加入房间, 无CurrentPlayer");
            }
        }

        public void UploadData()
        {
            if (IsLocalPlayer)
            {
                selfProps.PlayerName = TestManager.Instance.playerInitName;
                if (IsServer)
                {
                    SyncPlayerProps_ClientRpc(EncodeSelfProps(selfProps));
                }
                else SyncPlayerProps_ServerRpc(EncodeSelfProps(selfProps));
            }
        }

        [ServerRpc]
        private void SyncPlayerProps_ServerRpc(SyncPlayerPropsVar props)
        {
            syncProps.Value = props;
        }

        [ClientRpc]
        private void SyncPlayerProps_ClientRpc(SyncPlayerPropsVar props)
        {
            if (IsLocalPlayer)
            {
                syncProps.Value = props;
            }
        }


        public void UploadMove(Vector3 vec, Rigidbody rb)
        {
            if (IsLocalPlayer)
            {
                rb.MovePosition(vec);
                selfProps.PlayerPos = transform.position;
                UploadData();
            }
        }

    }
}
