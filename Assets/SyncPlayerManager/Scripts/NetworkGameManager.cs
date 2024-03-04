using GoldSprite.LobbyRoomUI;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Sync.Relay.Model;
using Unity.Sync.Relay.Transport.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoldSprite.MySyncPlayerManager
{
    public class NetworkGameManager : MonoBehaviour
    {
        public static NetworkGameManager Instance { get; internal set; }

        public static Action StartGameEvent;

        private NetworkManager networkManager;
        private RelayTransportNetcode netTrans;

        [SerializeField]
        private GameObject SyncPlayerManagerPrefab;
        [SerializeField]
        private GameObject PlayerPrefab;
        [SerializeField]
        public Transform GameElementsParent;

        private string playerGuid;
        public string PlayerGuid => playerGuid;
        private string playerInitName;
        public string PlayerInitName => playerInitName;
        private Dictionary<string, string> playerInitProps;

        private RelayRoom relayRoom => netTrans.GetRoomInfo();
        private RelayPlayer localPlayer => netTrans.GetCurrentPlayer();
        public Dictionary<uint, RelayPlayer> roomPlayers => relayRoom.Players;
        public Dictionary<uint, GameObject> playerRoles = new();


        private void Awake()
        {
            Instance = this;
            networkManager = NetworkManager.Singleton;
            netTrans = networkManager.GetComponent<RelayTransportNetcode>();

            SetInitializedPlayerData();

            LobbyRoomManager.EnterRoomEvent += () =>
            {
                InitSyncPlayerManager();
            };

        }

        [ContextMenu("SetInitializedPlayerData")]
        public void SetInitializedPlayerData()
        {
            playerGuid = Guid.NewGuid().ToString();

            var nList = new string[] { "猪老二", "神仙队友", "大神", "小菜鸟", "老六", "美女", "精神病患者", "嗜血狂魔" };
            var randomName = nList[UnityEngine.Random.Range(0, nList.Length - 1)];
            playerInitName = randomName + "-" + UnityEngine.Random.Range(0, 500);
            //playerInitName = GetRandomName();
            playerInitProps = new Dictionary<string, string>
        {
            { "PlayerName", playerInitName },
            { "GameStatu", GameStatu.UnReady },
        };
            SetPlayerData(playerGuid, playerInitName, playerInitProps);
        }

        public string GetRandomName()
        {
            return $"{ChineseTool.GetRandomChineseStrs(3, "")}-{UnityEngine.Random.Range(0, 999)}";
        }

        public void SetPlayerData(string guid, string name, Dictionary<string, string> props = null)
        {
            if (props == null) props = playerInitProps;
            netTrans.SetPlayerData(guid, name, props);
        }

        internal void SetPlayerNameData(string name)
        {
            SetPlayerData(playerGuid, name, playerInitProps);
        }


        public void StartGame()
        {
            playerRoles.Clear();
        }

        public void InitPlayer(uint transportId)
        {
            if (!roomPlayers.ContainsKey(transportId))
            {
                Debug.Log($"初始化玩家-{transportId}角色失败, 玩家id不存在.");
            }
            //else if (playerRoles.ContainsKey(transportId))
            //{
            //    Debug.Log($"初始化玩家-{transportId}角色失败, 玩家角色已存在.");
            //}
            else
            {
                //if (!IsRoomMaster())
                //{
                //    Debug.Log($"初始化玩家-{transportId}角色失败, 非服务端操作.");
                //}
                SpawnPlayerRole(transportId);
            }

        }

        private void SpawnPlayerRole(uint transportId)
        {

            //var player = roomPlayers[transportId];
            //Debug.Log($"生成玩家-{player.PlayerName}角色: ");

            //var playerObj = Instantiate(PlayerPrefab, GameElementsParent);
            //var nwObj = playerObj.GetComponent<NetworkObject>();
            //var playerName_Txt = playerObj.GetComponentInChildren<Text>();
            //playerObj.name = "Player-" + (playerName_Txt.text = player.PlayerName);

            //nwObj.Spawn();  //需要服务端Server才可生成
            //playerObj.transform.SetParent(GameElementsParent);
            //playerRoles.Add(transportId, playerObj);
        }


        private void InitSyncPlayerManager()
        {
        }


        public bool IsRoomMaster()
        {
            return relayRoom.MasterClientID == localPlayer.TransportId;
        }


        public struct GameStatu
        {
            public const string Key = "GameStatu";
            public const string UnReady = "0";
            public const string Ready = "1";
            public const string ToInitial = "2";
            public const string Gaming = "3";
            public const string GameFinish = "4";
        }
    }
}
