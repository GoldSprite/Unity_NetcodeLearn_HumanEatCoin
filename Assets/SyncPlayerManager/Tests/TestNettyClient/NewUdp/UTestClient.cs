using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty;
using DotNetty.Transport.Channels;
using GoldSprite.TestDotNetty;
using GoldSprite.TestDotNetty_API;
using UnityEngine;

namespace TestDotNetty_Client
{
    public class UTestClient: MonoBehaviour
    {
        private NetworkManager networkManager => NetworkManager.Singleton;
        public Thread clientThread;


        private void OnDestroy()
        {
            StopClientChannel();
        }


        [ContextMenu("StopClientChannel")]
        public void StopClientChannel()
        {
            Task.Run(() => CloseChannelSafe());
        }


        public async Task CloseChannelSafe()
        {
            NetworkManager.Singleton.Shutdown();
            clientThread.Join();
        }


        [ContextMenu("StartLink")]
        private void StartLink()
        {
            //TestDotNetty_Client.TestClient.Main(null);
            //clientThread = new Thread(() =>
            //{
            //    LogTools.NLogDebug("Run start...");
            //    Run();
            //    LogTools.NLogDebug("Run end.");
            //});
            //clientThread.Start();
        }


        private async Task Run()
        {
            var data = new ConnectData()
            {
                localAddress = GlobalConfiguation.LocalAddress,
                remoteAddress = GlobalConfiguation.LocalServerAddress
                //remoteAddress = GlobalConfiguation.CloudServerAddress
            };
            networkManager.SetConnectData(data);
            LogTools.NLogDebug("StartClient...");
            networkManager.StartClient();
            LogTools.NLogDebug("Started Client.");


            var msg = "Hello!";
            LogTools.NLogMsg("发送消息: " + msg);
            var pk = new MessageRequestPacket() { Message = msg };
            networkManager.NetTrans.SendPacket<MessageResponsePacket>(pk, (rep) =>
            {
                LogTools.NLogMsg("已收到回信!: " + rep.ResonMsg);
            });
        }
    }
}