using GoldSprite.AutoListUI;
using GoldSprite.BasicUIs;
using GoldSprite.MySyncPlayerManager;
using GoldSprite.TestSyncTemp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Sync.Relay;
using Unity.Sync.Relay.Lobby;
using Unity.Sync.Relay.Model;
using Unity.Sync.Relay.Transport.Netcode;
using UnityEngine;
using UnityEngine.UI;
using static GoldSprite.MySyncPlayerManager.NetworkGameManager;

namespace GoldSprite.LobbyRoomUI
{
    public class LobbyRoomManager : MonoBehaviour
    {
        //public const string RoomNameSpace = "AAA";
        public string RoomNameSpace = "HumanEatCoinRoom";

        public static LobbyRoomManager Instance { get; internal set; }
        public NetworkGameManager nwManager => NetworkGameManager.Instance;

        private NetworkManager networkManager;
        private UnityTransport netTrans;
        private RelayCallbacks callbacks;

        public static Action EnterRoomEvent;

        [SerializeField]
        private GameObject PlayerPrefab;
        [SerializeField]
        private Transform MapsParent;

        public GameObject LobbyWindow, RoomWindow;
        private GameObject currentWindow;
        public Text RoomWindow_PlayerName_Txt;
        public InputField NameInput;
        public InputField CreateRoomNameInput;
        public SimpleToggleClick CreateRoomToggle;
        public Text CreateRoomTip_Txt;

        public AutoListView LobbyListView;
        public AutoListView RoomListView;
        public List<LobbyRoom> lobbyRooms;
        [SerializeField]
        private int queryRoomCount = 10;
        [SerializeField]
        private float queryRoomInterval = 3;
        [SerializeField]
        private float refreshRoomPlayerInterval = 0.5f;
        private Coroutine Query_LobbyRoomListTask;
        private Coroutine Refresh_RoomPlayerListTask;

        public Dictionary<uint, RelayPlayer> roomPlayers;
        private RelayPlayer localPlayer;
        public RelayPlayer LocalPlayer => localPlayer;
        private string playerGuid => nwManager.PlayerGuid;

        private void Start()
        {
            Instance = this;
            networkManager = NetworkManager.Singleton;
            netTrans = networkManager.GetComponent<UnityTransport>();

            CGWindow(LobbyWindow);

            StartQueryLobbyRoomListTask();

            NameInput.text = nwManager.PlayerInitName;
            SetPlayerNameDataBtn();
        }

        private void Update()
        {
            //Debug.Log($"networkManager.IsConnectedClient: {networkManager.IsConnectedClient}, IsRooming: {IsRooming()}");
            //if (currentWindow == RoomWindow && !IsRooming())
            //{
            //    ExitRoomUI();
            //}
        }

        //public bool IsRooming()
        //{
        //    return !(relayRoom.Players == null || relayRoom.Players.Count <= 0 || !relayRoom.Players.ContainsKey(localPlayer.TransportId));
        //}

        //[ContextMenu("QueryRoomList")]
        //public void QueryRoomList()
        //{
        //    StartQueryLobbyRoomListTask();
        //}

        public void QueryLobbyRoomList()
        {
            //var request = new ListRoomRequest()
            //{
            //    Namespace = RoomNameSpace,
            //    Start = 0,
            //    Count = queryRoomCount
            //};

            //StartCoroutine(LobbyService.AsyncListRoom(request, (resp) =>
            //{
            //    if (resp.Code != (uint)RelayCode.OK)
            //    {
            //        //Debug.Log("查询房间失败.");
            //    }
            //    else
            //    {
            //        lobbyRooms = resp.Items;
            //        //移除解散的房间
            //        var removeList = LobbyListView.GetItems().Keys.Where(roomUuid => !lobbyRooms.Any(p => p.RoomUuid == (string)roomUuid)).ToList();
            //        foreach (var roomUuid in removeList) LobbyListView.RemoveItem(roomUuid);

            //        if (resp.Items.Count <= 0)
            //        {
            //            //Debug.Log("查询房间数为空.");
            //        }
            //        else
            //        {
            //            //Debug.Log($"查询房间数成功: {resp.Items.Count}个房间.");

            //            lobbyRooms = resp.Items;

            //            //刷新增加的房间
            //            foreach (var room in lobbyRooms)
            //            {
            //                LobbyListView.AddItem(room.RoomUuid);  //增加新房间信息

            //                //刷新房间信息
            //                var roomItem = LobbyListView.GetItem<string, LobbyAutoListItem>(room.RoomUuid);
            //                roomItem.Name = room.Name;
            //                roomItem.RoomPlayerCount = room.PlayerCount;
            //                roomItem.MaxRoomPlayers = room.MaxPlayers;
            //                roomItem.roomInfo = room;
            //                roomItem.UpdateContent();
            //            }
            //        }
            //    }
            //}));
        }

        public IEnumerator QueryLobbyRoomListTask()
        {
            while (true)
            {
                QueryLobbyRoomList();
                yield return new WaitForSeconds(queryRoomInterval);
            }
        }

        public void StartQueryLobbyRoomListTask() { Query_LobbyRoomListTask = StartCoroutine(QueryLobbyRoomListTask()); }
        public void StopQueryLobbyRoomListTask() { if (Query_LobbyRoomListTask != null) StopCoroutine(Query_LobbyRoomListTask); }


        public void RefreshRoomPlayerList()
        {
            //移除退出的玩家
            var removeList = RoomListView.GetItems().Keys.Where(playerTransportID => !roomPlayers.ContainsKey((uint)playerTransportID)).ToList();
            foreach (var roomUuid in removeList) RoomListView.RemoveItem(roomUuid);

            foreach (var (transportID, player) in roomPlayers)
            {
                RoomListView.AddItem(transportID); //增加新玩家信息

                //刷新信息
                var playerItem = RoomListView.GetItem<uint, RoomAutoListItem>(transportID);
                playerItem.Name = player.Name;
                playerItem.Ready = player.Properties.ContainsKey(GameStatu.Key) ? player.Properties[GameStatu.Key] == GameStatu.Ready : false;
                playerItem.UpdateContent();
            }
        }
        public IEnumerator RefreshRoomPlayerListTask()
        {
            while (true)
            {
                RefreshRoomPlayerList();
                yield return new WaitForSeconds(refreshRoomPlayerInterval);
            }
        }

        public void StartRefreshRoomPlayerListTask() { Refresh_RoomPlayerListTask = StartCoroutine(RefreshRoomPlayerListTask()); }
        public void StopRefreshRoomPlayerListTask() { if (Refresh_RoomPlayerListTask != null) StopCoroutine(Refresh_RoomPlayerListTask); }

        public void GetRandomNameBtn()
        {
            NameInput.text = nwManager.GetRandomName();
            SetPlayerNameDataBtn();
        }

        public void SetPlayerNameDataBtn()
        {
            nwManager.SetPlayerNameData(NameInput.text);
        }

        public void JoinRoom(LobbyRoom room)
        {
            //StartCoroutine(LobbyService.AsyncQueryRoom(room.RoomUuid, (resp) =>
            //{
            //    if (resp.Code != (uint)RelayCode.OK)
            //    {
            //        Debug.Log("加入房间失败.");
            //    }
            //    else
            //    {
            //        netTrans.SetRoomData(resp);
            //        if (!netTrans.CheckRequirement())
            //        {
            //            Debug.Log("加入房间失败: 配置不全.");
            //        }
            //        else
            //        {
            //            networkManager.StartClient();
            //            Debug.Log("加入房间成功.");
            //            EnterRoomEvent?.Invoke();
            //        }
            //    }
            //}));
        }

        public void JoinRoomBtn()
        {
            TestManager2.Instance.StartClient();
            StartCoroutine(JoinRoomTask());
        }

        private IEnumerator JoinRoomTask()
        {
            var exit = 0;
            var startTime = DateTime.Now.Ticks;
            var timeOut = 3000;
            while (exit==0)
            {
                if(startTime + timeOut < DateTime.Now.Ticks)
                {
                    exit = 1;
                    CreateRoomTip_Txt.text = "连接超时";
                }else if (GameManager.Instance.IsConnected)
                {
                    exit = 2;
                    CreateRoomTip_Txt.text = "连接成功";
                }

                yield return new WaitForSeconds(0.2f);
            }
            if(exit==2) CGGameWindow();
        }

        public void InitRoom(uint playerTransportID=default(uint), RelayRoom room=null)
        {
            CGWindow(RoomWindow);
            StopQueryLobbyRoomListTask();

            localPlayer = room.GetPlayer(playerTransportID);
            RoomWindow_PlayerName_Txt.text = localPlayer.Name;

            roomPlayers = room.Players;
            //StartRefreshRoomPlayerListTask();
        }

        public void ExitRoomUI()
        {
            CGWindow(LobbyWindow);
            StopRefreshRoomPlayerListTask();
            StartQueryLobbyRoomListTask();
        }

        public void CGWindow(GameObject window)
        {
            LobbyWindow.SetActive(false);
            RoomWindow.SetActive(false);

            if (window != null)
            {
                window.SetActive(true);
                currentWindow = window;
            }
        }

        public void ReadyBtn()
        {
            if (!localPlayer.Properties.ContainsKey(GameStatu.Key)) return;

            var isReady = localPlayer.Properties[GameStatu.Key] == GameStatu.Ready;
            Ready(localPlayer.TransportId, !isReady);
        }

        private void Ready(uint transportId, bool ready)
        {
            if (!localPlayer.Properties.ContainsKey(GameStatu.Key)) return;

            localPlayer.Properties[GameStatu.Key] = ready ? GameStatu.Ready : GameStatu.UnReady;
            var playerItem = RoomListView.GetItem<uint, RoomAutoListItem>(transportId);
            playerItem.Ready = ready;
            playerItem.UpdateContent();
        }

        public void ExitRoomBtn()
        {
            networkManager.Shutdown();
            CGWindow(LobbyWindow);
            //var playerNWObj = networkManager.LocalClient.PlayerObject;
            //var relayPlayer = netTrans.GetRoomInfo().Players.Values.First(p => p.ID == NetworkGameManager.Instance.PlayerGuid);
            ////netTrans.KickPlayer(relayPlayer.TransportId);
            ////networkManager.DisconnectClient(networkManager.LocalClientId);
            ////networkManager.Shutdown();
            //var randomOtherClientId = networkManager.ConnectedClientsIds.First(p => p != networkManager.LocalClientId);
            //netTrans.GetRoomInfo().MasterClientID = randomOtherClientId;
            //ExitRoomUI();
        }

        public void CloseRoomBtn()
        {
            //if (nwManager.IsRoomMaster())
            //{
            //    StartCoroutine(LobbyService.AsyncCloseRoom(relayRoom.ID, (resp) =>
            //    {
            //        if (resp.Code == (uint)RelayCode.OK)
            //        {
            //            Debug.Log("房主关闭房间.");
            //            networkManager.Shutdown();
            //        }
            //        else
            //        {
            //            Debug.Log($"关闭房间异常: {resp.Code}.");
            //        }
            //    }));
            //}
            //ExitRoomUI();
        }

        public void CreateAndJoinRoomBtn()
        {
            var lowerRoomNameLength = 3;
            if (CreateRoomNameInput.text == "" || CreateRoomNameInput.text.Length < lowerRoomNameLength)
            {
                CreateRoomTip_Txt.text = "房间名最短3位.";
            }
            else
            {
                CreateRoomTip_Txt.text = "开始连接远程房间..";

                //var request = new CreateRoomRequest()
                //{
                //    Namespace = RoomNameSpace,
                //    Name = CreateRoomNameInput.text,
                //    MaxPlayers = 4,
                //    OwnerId = playerGuid,
                //    Visibility = LobbyRoomVisibility.Public
                //};
                //StartCoroutine(LobbyService.AsyncCreateRoom(request, (resp) =>
                //{
                //    if (resp.Code != (uint)RelayCode.OK)
                //    {
                //        CreateRoomTip_Txt.text = "远程房间创建失败.";
                //    }
                //    else
                //    {
                //        netTrans.SetRoomData(resp);
                //        if (!netTrans.CheckRequirement())
                //        {
                //            CreateRoomTip_Txt.text = "客户端配置不全.";
                //        }
                //        else
                //        {
                //            networkManager.StartHost();
                //            Debug.Log("房间创建成功.");
                //            CreateRoomTip_Txt.text = "";
                //            CreateRoomToggle.CGDroping?.Invoke();
                //            EnterRoomEvent?.Invoke();
                //            CGGameWindow();
                //        }
                //    }
                //}));
            }
        }

        public void StartGameBtn()
        {
            if (!nwManager.IsRoomMaster())
            {
                Debug.Log("非房主无权限.");
            }
            else
            {
                var isAllReady = !roomPlayers.Values.Any(p => p.Properties[GameStatu.Key] == GameStatu.UnReady);
                if (!isAllReady)
                {
                    Debug.Log("还有玩家未准备.");
                }
                else
                {
                    Debug.Log("所有人已准备, 游戏开始");

                    nwManager.StartGame();
                }
            }
        }

        public void CGGameWindow()
        {
            CGWindow(null);
            StopRefreshRoomPlayerListTask();
        }
    }

    public class ChineseTool
    {
        static System.Random rand = new System.Random();

        public static string GetChineseStr(int numCode)
        {
            var code16 = numCode.ToString("X4");
            return Regex.Unescape("\\u" + code16);
        }

        public static string GetRandomChineseStr()
        {
            var minIndex = Convert.ToInt32("00004e00", 16);
            var maxCount = Convert.ToInt32("00009fa5", 16) - minIndex;
            var randomNum = minIndex + rand.Next(maxCount);
            return GetChineseStr(randomNum);
        }

        public static string GetRandomChineseStrs(int length, string split)
        {
            var str = "";
            for (int i = 0; i < length; i++)
            {
                str += GetRandomChineseStr() + split;
            }
            return str;
        }
    }
}
