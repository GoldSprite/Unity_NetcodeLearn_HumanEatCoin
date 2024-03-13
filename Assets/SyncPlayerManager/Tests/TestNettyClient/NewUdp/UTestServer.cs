using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DotNetty;
using GoldSprite.TestDotNetty_API;
using UnityEngine;

namespace TestDotNetty_Server
{
    public class UTestServer:MonoBehaviour
    {
        private NetworkManager networkManager => NetworkManager.Singleton;
        public Thread clientThread;


        private void OnDestroy()
        {
            StopClientChannel();
        }


        private void Start()
        {
            LogTools.LogAction = log => Debug.Log(log);
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
            //TestDotNetty_Server.TestServer.Main(null);
            //clientThread = new Thread(() =>
            //{
            //    Console.WindowWidth = 120;
            //    Run();
            //});
            //clientThread.Start();
        }


        private void Run()
        {
            var data = new ConnectData()
            {
                //localAddress = GlobalConfiguation.LocalServerAddress
                localAddress = GlobalConfiguation.LocalAddress2
            };
            networkManager.SetConnectData(data);
            networkManager.StartServer();
        }
    }
}