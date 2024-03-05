using GoldSprite.LobbyRoomUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace GoldSprite.TestSyncTemp
{
    public class TestManager2 : MonoBehaviour
    {
        public static TestManager2 Instance { get; private set; }
        public string RoomNamespace = "BBB";
        public NetworkManager networkManager;
        public UnityTransport netTrans;

        public Text tip_Txt;
        public static Text Tip_Txt => Instance.tip_Txt;
        public string playerGuid;
        public string playerInitName;
        public string roomUuid;

        public string localIp = "0.0.0.0";
        public string networkIp = "0.0.0.0";
        public ushort port = 34001;

        public Text rttMs_Txt;
        public Text HostIp_Txt;
        public string HostIP;
        public Text Info_Txt;

        public bool IsConnected { get; set; }
        public uint LocalPlayerTransportId { get; set; }
        public int RoomPlayerCount { get; set; }

        public ulong LocalNId = 0;


        private void Start()
        {
            Instance = this;
            networkManager = NetworkManager.Singleton;
            netTrans = networkManager.GetComponent<UnityTransport>();

            HostIP = "HostIP: " + NetworkTools.GetNetworkIPV4();

        }

        private void Update()
        {
            //Debug.Log("IsConnected: "+ IsConnected);

            Info_Txt.text =
                HostIP +
                "\nServer: "+networkIp+":"+port+
                "\nPing: " + (IsConnected ? netTrans.GetCurrentRtt(netTrans.ServerClientId) : -1) + "ms" +
                "\nFPS: " + (int)(1 / Time.deltaTime)+
                "\nIsConnected: "+ IsConnected + 
                "\nLocalClientId: " + LocalNId +
                "\nRoomPlayerCount: " + RoomPlayerCount
                ;
        }

        public void StartHost()
        {
            netTrans.SetConnectionData(localIp, port);
            networkManager.StartHost();
        }

        public void StartClient()
        {
            netTrans.SetConnectionData(networkIp, port);
            networkManager.StartClient();
        }

        public void Shutdown()
        {
            networkManager.Shutdown();
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


    public class NetworkTools
    {
        public static string RegexGetIPAddress(string input)
        {
            string pattern = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(input);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "未找到IP地址";
            }
        }

        public static string GetPublicIPHtml()
        {
            string publicIPAddress = string.Empty;
            try
            {
                using (WebClient client = new WebClient())
                {
                    publicIPAddress = client.DownloadString("https://api.ipify.org");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error retrieving public IP address: " + e.Message);
            }

            return publicIPAddress;
        }

        public static string GetNetworkIPV4()
        {
            return RegexGetIPAddress(GetPublicIPHtml());
        }
    }
}
