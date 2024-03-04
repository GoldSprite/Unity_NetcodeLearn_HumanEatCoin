using System;
using Unity.Netcode;
using Unity.Sync.Relay.Lobby;
using Unity.Sync.Relay.Model;
using Unity.Sync.Relay.Transport.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace GoldSprite.TestSyncTemp
{
    public class TestManager : MonoBehaviour
    {
        public static TestManager Instance;
        public NetworkManager networkManager;
        public RelayTransportNetcode netTrans;

        public Text tip_Txt;
        public static Text Tip_Txt => Instance.tip_Txt;
        public string playerGuid;
        public string playerInitName;
        public string roomUuid;

        public TestSyncPropsHandler LocalPlayer;


        private void Start()
        {
            Instance = this;
            networkManager = NetworkManager.Singleton;
            netTrans = networkManager.GetComponent<RelayTransportNetcode>();

            //playerGuid = Guid.NewGuid().ToString();

            //playerInitName = GetRandomName();
            //netTrans.SetPlayerData(playerGuid, playerInitName);
        }

        public void StartHost()
        {

            Tip_Txt.text = "连接远程房间中..";
            StartCoroutine(LobbyService.AsyncCreateRoom(new CreateRoomRequest()
            {
                Name = "AAA",
                Namespace = "AAA",
                MaxPlayers = 4,
                OwnerId = playerGuid,
                Visibility = LobbyRoomVisibility.Public
            }, (resp) =>
            {
                if (resp.Code == (uint)RelayCode.OK)
                {
                    netTrans.SetRoomData(resp);
                    Tip_Txt.text = "连接房间成功.";
                    NetworkManager.Singleton.StartHost();
                    Tip_Txt.text = "开启房间中..";
                }
            }));
        }

        public void StartClient()
        {
            // 异步查询房间列表
            StartCoroutine(LobbyService.AsyncListRoom(new ListRoomRequest()
            {
                Namespace = "AAA",
                Start = 0,
                Count = 10,
                //PlayerName = "U", // 选填项，可用于房间名的模糊搜索
            }, (resp) =>
            {
                if (resp.Code == (uint)RelayCode.OK)
                {
                    Debug.Log("List Room succeed.");
                    if (resp.Items.Count > 0)
                    {
                        foreach (var item in resp.Items)
                        {
                            if (item.Status == LobbyRoomStatus.Ready)
                            {
                                // 异步查询房间信息
                                StartCoroutine(LobbyService.AsyncQueryRoom(item.RoomUuid,
                                    (_resp) =>
                                    {
                                        if (_resp.Code == (uint)RelayCode.OK)
                                        {
                                            // 需要在连接到Relay服务器之前，设置好房间信息
                                            NetworkManager.Singleton.GetComponent<RelayTransportNetcode>()
                                                .SetRoomData(_resp);
                                            // 如果是Private类型的房间，需要开发者自行获取JoinCode，并调用以下方法设置好
                                            // NetworkManager.Singleton.GetComponent<RelayTransportNetcode>().SetJoinCode(JoinCode);

                                            NetworkManager.Singleton.StartClient();

                                            roomUuid = _resp.RoomUuid;
                                            Tip_Txt.text = $"成功加入房间[{_resp.Namespace}-{_resp.Name}].";
                                        }
                                        else
                                        {
                                            Tip_Txt.text = "Query Room Fail By Lobby Service";
                                        }
                                    }));
                                break;
                            }
                        }
                    }
                    else
                    {
                        Tip_Txt.text = "没有运行房间可加入.";
                    }
                }
                else
                {
                    Tip_Txt.text = "List Room Fail By Lobby Service";
                }
            }));
        }


        [ContextMenu("RandomName")]
        public void SetRandomName()
        {

            playerInitName = GetRandomName();
        }

        public string GetRandomName()
        {
            var nList = new string[] { "猪老二", "神仙队友", "大神", "小菜鸟", "老六", "美女", "精神病患者", "嗜血狂魔" };
            var randomName = nList[UnityEngine.Random.Range(0, nList.Length - 1)];
            return randomName + "-" + UnityEngine.Random.Range(0, 500);
        }

    }
}
