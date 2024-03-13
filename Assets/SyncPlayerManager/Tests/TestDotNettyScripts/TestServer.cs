using GoldSprite.TestDotNetty_API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TestDotNetty_Server {
    public class TestServer : MonoBehaviour {
        private NetworkManager networkManager => NetworkManager.Singleton;
        public Thread clientThread;


        private void OnDestroy()
        {
            StopClientChannel();
        }


        [ContextMenu("StopClientChannel")]
        public void StopClientChannel() {
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
                //localAddress = GlobalConfiguation.LocalServerAddress
                localAddress = GlobalConfiguation.LocalAddress2
            };
            networkManager.SetConnectData(data);
            networkManager.StartServer();
        }
    }
}
