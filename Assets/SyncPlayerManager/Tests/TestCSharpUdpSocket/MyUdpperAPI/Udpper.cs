using System;
using System.Net;
using System.Net.Sockets;



namespace GoldSprite.MyUdpperAPI {
    public class Udpper {
        public static bool LoginAuth = true;

        public static Udpper Singleton { get; private set; }
        public Socket udpSocket;
        public HandlerChannel channel;
        private ConnectData connectData = new ConnectData();
        public ConnectData ConnectData => connectData;
        public StartType StartType { get; private set; }
        private UdpNetworkTransport netTrans = new UdpNetworkTransport();
        public UdpNetworkTransport NetTrans => netTrans;


        public Udpper() { Singleton = this; }

        public void SetConnectData(ConnectData connectData)
        {
            this.connectData = connectData;
        }

        public void StartServer()
        {
            StartType = StartType.Server;
            if (CanStart(StartType))
                LogTools.NLogInfo("开启服务器.");
            if (StartUdpper()) {
                LogTools.NLogInfo("开启服务器成功.");
            } else
                LogTools.NLogInfo("开启服务器失败.");
        }


        public void StartClient()
        {
            StartType = StartType.Client;
            if (CanStart(StartType))
                LogTools.NLogInfo("开启客户端.");
            if (StartUdpper()) {
                LogTools.NLogInfo("开启客户端成功.");
            } else
                LogTools.NLogInfo("开启客户端失败.");
        }

        public bool StartUdpper()
        {
            try {
                udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                udpSocket.Bind(ConnectData.LocalAddress);

                channel = new HandlerChannel(udpSocket, this);
                channel.Handlers((channel) => {
                    netTrans.InitHandlers(channel);
                });
                channel.Start();
                LogTools.SubMsg = "[" + (StartType == StartType.Server ? connectData.LocalAddress.Port + "" : NetworkTools.GetNetworkIPV4()) + "] ";
                return true;
            }
            catch (Exception e) {
                LogTools.NLogErr(e.StackTrace);
                return false;
            }
        }

        public void Shutdown()
        {
            LogTools.NLogInfo("关闭本地频道...");
            channel.CloseChannel();
            udpSocket.Close(1);
        }


        private bool CanStart(StartType type)
        {
            string errPrefix = "开始配置: ";
            var errInfo = "";
            switch (type) {
                case StartType.Server:
                    errInfo += connectData.LocalAddress == null ? "本地地址未设置" : "";
                    if (!string.IsNullOrEmpty(errInfo)) {
                        LogTools.NLogWarn(errPrefix + errInfo);
                        return false;
                    }
                    break;
                case StartType.Client:
                    errInfo += connectData.LocalAddress == null ? "本地地址未设置" : "";
                    errInfo += connectData.RemoteAddress == null ? "远程地址未设置" : "";
                    if (!string.IsNullOrEmpty(errInfo)) {
                        LogTools.NLogWarn(errPrefix + errInfo);
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}
