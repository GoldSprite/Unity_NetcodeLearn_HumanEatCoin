using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Drawing;
using UnityEditor.PackageManager;
using GoldSprite.MyUdpperAPI;

namespace Assets.SyncPlayerManager.Tests.TestCSharpUdpSocket {
    public class Server2 : MonoBehaviour {
        public string localIp = "192.168.1.105";
        public int localPort = 8007;
        public IPEndPoint localAddress;

        public string networkIp = "192.168.1.105";
        public int networkPort = 9007;
        public IPEndPoint networkAddress;

        public Udpper udpper;

        private Thread t;


        private void OnDestroy()
        {
            try {
                t.Interrupt();
            }
            catch (Exception e) {
                Debug.LogWarning("1线程已终止.");
            }
        }

        [ContextMenu("Start")]
        public void Server()
        {
            localAddress = new IPEndPoint(IPAddress.Parse(localIp), localPort);
            networkAddress = new IPEndPoint(IPAddress.Parse(networkIp), networkPort);

            udpper = new Udpper();
            udpper.SetConnectData(new ConnectData { LocalAddress = localAddress });
            udpper.StartServer();
        }


        //static EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.1.105"), 9007);
        //[ContextMenu("SendMsg2")]
        //public void sendMsg2()
        //{
        //    Debug.Log("发送");
        //    string msg = "dgasdgasd";

        //}


        ///// <summary>
        ///// 向特定ip的主机的端口发送数据报
        ///// </summary>
        //static void sendMsg()
        //{
        //    try {
        //        while (true) {
        //            Debug.Log("发送");
        //            string msg = "asedfaeswt";
        //            server.SendTo(Encoding.UTF8.GetBytes(msg), point);

        //            Debug.Log("等两秒");
        //            Thread.Sleep(2000);
        //        }
        //    }
        //    catch (Exception e) {
        //        Debug.LogWarning("线程已被终止.");
        //    }
        //}

        ///// <summary>
        ///// 接收发送给本机ip对应端口号的数据报
        ///// </summary>
        //static void ReciveMsg()
        //{
        //    try {
        //        while (true) {
        //            EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
        //            byte[] buffer = new byte[1024];
        //            int length = server.ReceiveFrom(buffer, ref point);//接收数据报
        //            string message = Encoding.UTF8.GetString(buffer, 0, length);
        //            Debug.Log(point.ToString() + message);

        //        }
        //    }
        //    catch (Exception e) {
        //        Debug.LogWarning("线程已被终止.");
        //    }
        //}


    }
}
