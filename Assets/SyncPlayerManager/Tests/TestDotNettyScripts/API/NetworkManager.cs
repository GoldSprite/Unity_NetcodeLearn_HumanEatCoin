using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldSprite.TestDotNetty_API
{
    public class NetworkManager
    {
        public static Random rand = new Random();
        public static readonly NetworkManager Singleton = new NetworkManager();


        private ConnectData connectData = new ConnectData();
        public ConnectData ConnectData => connectData;
        public StartType StartType { get; private set; }
        private UdpNetworkTransport netTrans = new UdpNetworkTransport();
        public UdpNetworkTransport NetTrans => netTrans;


        private Bootstrap bootstrap;
        private MultithreadEventLoopGroup group;


        public void StartServer()
        {
            StartType = StartType.Server;
            if (CanStart(StartType))
                LogTools.NLogInfo("开启服务器.");
            if (StartLocalBootstrap())
            {
                LogTools.NLogInfo("开启服务器成功.");
            }
            else LogTools.NLogInfo("开启服务器失败.");
        }


        public void StartClient()
        {
            StartType = StartType.Client;
            if (CanStart(StartType))
                LogTools.NLogInfo("开启客户端.");
            if (StartLocalBootstrap())
            {
                LogTools.NLogInfo("开启客户端成功.");
            }
            else LogTools.NLogInfo("开启客户端失败.");
        }


        private bool StartLocalBootstrap()
        {
            try
            {
                group = new MultithreadEventLoopGroup();
                bootstrap = new Bootstrap();
                bootstrap.Group(group);
                bootstrap.Channel<SocketDatagramChannel>();
                bootstrap.Handler(new ActionChannelInitializer<IChannel>(ch =>
                {
                    netTrans.InitHandlers(ch);
                }));
                bootstrap.LocalAddress(connectData.localAddress);
                bootstrap.BindAsync().Wait();
                LogTools.SubMsg = "["+(StartType == StartType.Server ? connectData.localAddress.Port + "" : NetworkTools.GetNetworkIPV4())+"] ";
                return true;
            }
            catch (Exception e)
            {
                group.ShutdownGracefullyAsync().Wait();
                LogTools.NLogErr(e.StackTrace);
                return false;
            }
        }


        public void Shutdown()
        {
            LogTools.NLogInfo("关闭本地频道...");

            netTrans.Channel.CloseAsync().Wait();
            group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }


        public void SetConnectData(ConnectData connectData)
        {
            this.connectData = connectData;
        }


        private bool CanStart(StartType type)
        {
            string errPrefix = "开始配置: ";
            var errInfo = "";
            switch (type)
            {
                case StartType.Server:
                    errInfo += connectData.localAddress == null ? "本地地址未设置" : "";
                    if (!String.IsNullOrEmpty(errInfo))
                    {
                        LogTools.NLogWarn(errPrefix + errInfo);
                        return false;
                    }
                    break;
                case StartType.Client:
                    errInfo += connectData.localAddress == null ? "本地地址未设置" : "";
                    errInfo += connectData.remoteAddress == null ? "远程地址未设置" : "";
                    if (!String.IsNullOrEmpty(errInfo))
                    {
                        LogTools.NLogWarn(errPrefix + errInfo);
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}
