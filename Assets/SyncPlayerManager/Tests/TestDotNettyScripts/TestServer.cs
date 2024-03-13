using GoldSprite.TestDotNetty_API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace TestDotNetty_Server {
    public class TestServer : MonoBehaviour {
        private NetworkManager networkManager => NetworkManager.Singleton;
        public Thread clientThread;


        //private void OnDestroy()
        //{
        //    StopClientChannel();
        //}


        //[ContextMenu("StopClientChannel")]
        //public void StopClientChannel() {
        //    Task.Run(() => CloseChannelSafe()).Wait();
        //}


        //public async Task CloseChannelSafe() {
        //    NetworkManager.Singleton.Shutdown();
        //    clientThread.Join();
        //}


        [ContextMenu("Main")]
        public void Main() {
            StartCoroutine(Run());
        }


        private IEnumerator Run() {
            var data = new ConnectData() {
                //localAddress = GlobalConfiguration.LocalServerAddress
                localAddress = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8007)
            };
            networkManager.SetConnectData(data);
            networkManager.StartServer();
            yield return null;
        }
    }
}
