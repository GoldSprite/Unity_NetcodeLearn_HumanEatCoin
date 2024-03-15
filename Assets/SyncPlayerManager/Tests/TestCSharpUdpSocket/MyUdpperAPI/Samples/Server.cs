using System.Net;
using System.Threading;

namespace GoldSprite.MyUdpperAPI {
    public class Server {
        public string localIp = "0.0.0.0";
#if APPLICATION_CONSOLE
        public int localPort = 8007;
#elif APPLICATION_CLOUD || UNITY_2017_1_OR_NEWER
        public int localPort = 34001;
#endif
        public IPEndPoint localAddress;

        public string networkIp = "192.168.1.105";
        public int networkPort = 9007;
        public IPEndPoint networkAddress;

        public Udpper udpper;
        public Thread cmdThread;


        public Server()
        {
            localAddress = new IPEndPoint(IPAddress.Parse(localIp), localPort);
            networkAddress = new IPEndPoint(IPAddress.Parse(networkIp), networkPort);

            udpper = new Udpper();
            udpper.SetConnectData(new ConnectData { LocalAddress = localAddress });
            udpper.StartServer();

            if (GlobalConfiguration.CLICMD_Enabled) {
                cmdThread = new Thread(() => new CmdSender());
                cmdThread.Start();
            }
        }
    }
}
