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

namespace Assets.SyncPlayerManager.Tests.TestCSharpUdpSocket {
    public class Server : MonoBehaviour {
        static Socket server;
        private Thread t2;
        private Thread t;


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

        [ContextMenu("Start")]
        public void Main()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(new IPEndPoint(IPAddress.Parse("192.168.1.105"), 8007));//绑定端口号和IP
            Debug.Log("服务端已经开启");
            t = new Thread(ReciveMsg);//开启接收消息线程
            t.Start();
            //t2 = new Thread(sendMsg);//开启发送消息线程
            //t2.Start();


        }


        static EndPoint point = new IPEndPoint(IPAddress.Parse("192.168.1.105"), 9007);
        [ContextMenu("SendMsg2")]
        public void sendMsg2()
        {
            Debug.Log("发送");
            string msg = "dgasdgasd";
            server.SendTo(Encoding.UTF8.GetBytes(msg), point);
        }


        /// <summary>
        /// 向特定ip的主机的端口发送数据报
        /// </summary>
        static void sendMsg()
        {
            try {
                while (true) {
                    Debug.Log("发送");
                    string msg = "asedfaeswt";
                    server.SendTo(Encoding.UTF8.GetBytes(msg), point);

                    Debug.Log("等两秒");
                    Thread.Sleep(2000);
                }
            }
            catch (Exception e) {
                Debug.LogWarning("线程已被终止.");
            }


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
                    int length = server.ReceiveFrom(buffer, ref point);//接收数据报
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
