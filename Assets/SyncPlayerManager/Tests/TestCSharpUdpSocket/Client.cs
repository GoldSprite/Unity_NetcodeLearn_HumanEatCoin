using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.SyncPlayerManager.Tests.TestCSharpUdpSocket {
    public class Client : MonoBehaviour {
        static Socket client;
        private Thread t2;
        private Thread t;

        public string ip = "192.168.1.105";
        public int port = 34002;
        public IPEndPoint localPoint;
        [Space]
        public string remoteIp = "162.14.68.248";
        public int remotePort = 34002;
        public IPEndPoint remotePoint;


        private void OnDestroy()
        {
            try {
                t.Interrupt();
            }
            catch (Exception e) {
                Debug.LogWarning("1线程已终止.");
            }
            try {
                t2.Interrupt();
            }
            catch (Exception e) {
                Debug.LogWarning("2线程已终止.");
            }
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.E)) {
                sendMsg2();
            }
        }


        [ContextMenu("Start")]
        public void Main()
        {
            localPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            remotePoint = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);

            client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            client.Bind(localPoint);
            //t = new Thread(sendMsg);
            //t.Start();
            t2 = new Thread(ReciveMsg);
            t2.Start();
            Debug.Log("客户端已经开启");
        }


        [ContextMenu("SendMsg2")]
        public void sendMsg2()
        {
            Debug.Log("发送");
            string msg = "和雕塑高富帅";
            client.SendTo(Encoding.UTF8.GetBytes(msg), remotePoint);
        }


        /// <summary>
        /// 接收发送给本机ip对应端口号的数据报
        /// </summary>
        static void ReciveMsg()
        {
            try {
                while (true) {
                    EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                    byte[] buffer = new byte[1024];
                    int length = client.ReceiveFrom(buffer, ref point);//接收数据报
                    string message = Encoding.UTF8.GetString(buffer, 0, length);
                    Debug.Log(point.ToString() + message);

                }
            }
            catch (Exception e) {
                Debug.LogWarning("线程已被终止.");
            }
        }


    }
}
