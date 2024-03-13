using GoldSprite.TestDotNetty_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TestDotNetty_Client {
    public class TestClient: MonoBehaviour {
        private NetworkManager networkManager => NetworkManager.Singleton;
        public Thread clientThread;


        private void OnDestroy()
        {
            StopClientChannel();
        }


        [ContextMenu("StopClientChannel")]
        public void StopClientChannel()
        {
            Task.Run(() => CloseChannelSafe()).Wait();
        }


        public async Task CloseChannelSafe() {
            NetworkManager.Singleton.Shutdown();
            clientThread.Join();
        }


        [ContextMenu("Main")]
        public void Main() {
            clientThread = new Thread(() => {
                //Console.WindowWidth = 120;
                Run();
                //Console.ReadLine();
            });
            clientThread.Start();
        }


        private void Run() {
            var data = new ConnectData() {
                localAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9007)
                ,
                remoteAddress = GlobalConfiguration.LocalServerAddress
                //remoteAddress = GlobalConfiguration.CloudServerAddress
            };
            networkManager.SetConnectData(data);
            networkManager.StartClient();


            //var msg = "Hello!";
            //LogTools.NLogMsg("发送消息: " + msg);
            //var pk = new MessageRequestPacket() { Message = msg };
            //networkManager.NetTrans.SendPacket<MessageResponsePacket>(pk, (rep) => {
            //    LogTools.NLogMsg("已收到回信!: " + rep.ResonMsg);
            //});
        }
    }
}
