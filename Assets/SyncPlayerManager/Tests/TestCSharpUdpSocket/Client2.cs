using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using GoldSprite.MyUdpperAPI;
using UnityEditor.PackageManager;
using UnityEngine.UI;

namespace Assets.SyncPlayerManager.Tests.TestCSharpUdpSocket {
    public class Client2 : MonoBehaviour {
        public string localIp = "192.168.1.105";
        public int localPort = 9007;
        public IPEndPoint localAddress;

        public string networkIp = "192.168.1.105";
        public int networkPort = 8007;
        public IPEndPoint networkAddress;

        public Udpper udpper;
        public Thread cmdThread;
        private Thread t2;
        public string CmdString;
        public InputField CmdInput;

        private void OnDestroy()
        {
            udpper.Shutdown();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Q)) {
                Client();
            }
            if (Input.GetKeyDown(KeyCode.E)) {
                sendMsg2();
            }
            if (Input.GetKeyDown(KeyCode.R)) {
                SendCmd();
            }
        }


        [ContextMenu("Start")]
        public void Client()
        {
            localAddress = new IPEndPoint(IPAddress.Parse(localIp), localPort);
            networkAddress = new IPEndPoint(IPAddress.Parse(networkIp), networkPort);

            udpper = new Udpper();
            udpper.SetConnectData(new ConnectData { LocalAddress = localAddress, RemoteAddress = networkAddress });
            udpper.StartClient();

        }


        [ContextMenu("SendMsg2")]
        public void sendMsg2()
        {
            Debug.Log("发送");
            string msg = "和雕塑高富帅";
            CmdSender.Instance.Cmd_SendMsg(msg);
        }

        [ContextMenu("SendCmd")]
        public void SendCmd()
        {
            
            var str = this.CmdString = CmdInput.text;
            var cmd = str.Split(' ');
            CmdSender.Instance.CmdHandler(cmd);
        }


    }
}
