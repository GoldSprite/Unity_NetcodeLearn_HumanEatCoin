using System;
using System.Net;
using System.Threading;

namespace GoldSprite.MyUdpperAPI {
    public class Client {
        public string localIp = "192.168.1.105";
        public int localPort = 9007;
        public IPEndPoint localAddress;

        public string networkIp = "192.168.1.105";
        public int networkPort = 8007;
        public IPEndPoint networkAddress;

        public Udpper udpper;
        public Thread cmdThread;


        public Client()
        {
            localAddress = new IPEndPoint(IPAddress.Parse(localIp), localPort);
            networkAddress = new IPEndPoint(IPAddress.Parse(networkIp), networkPort);

            udpper = new Udpper();
            udpper.SetConnectData(new ConnectData { LocalAddress = localAddress, RemoteAddress = networkAddress });
            udpper.StartClient();

            if (GlobalConfiguration.CLICMD_Enabled) {
                cmdThread = new Thread(() => new CmdSender());
                cmdThread.Start();
            }
        }
    }
}
