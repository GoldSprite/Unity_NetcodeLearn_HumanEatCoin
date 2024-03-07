using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

using static GoldSprite.LogTools;
using DotNetty.Buffers;
using System.Text;
using System.Net.Sockets;

namespace GoldSprite.TestDotNetty
{
    public class TestNettyUdpServer : MonoBehaviour
    {
        public Thread clientThread;
        private MultithreadEventLoopGroup group;
        private SocketDatagramChannel serverChannel;

        async Task RunClientAsync()
        {
            //ExampleHelper.SetConsoleLogger();

            group = new MultithreadEventLoopGroup();

            X509Certificate2 cert = null;
            string targetHost = null;
            //if (ClientSettings.IsSsl)
            //{
            //    cert = new X509Certificate2(Path.Combine(ExampleHelper.ProcessDirectory, "shared\\dotnetty.com.pfx"), "password");
            //    targetHost = cert.GetNameInfo(X509NameType.DnsName, false);
            //}
            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<SocketDatagramChannel>()
                    .Handler(new ActionChannelInitializer<IDatagramChannel>((ch =>
                    {
                        IChannelPipeline pipeline = ch.Pipeline;
                        pipeline.AddLast(new NettyUdpChannelInboundHandler(true));
                    })));
                //.Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                //{
                //    IChannelPipeline pipeline = channel.Pipeline;

                //    //if (cert != null)
                //    //{
                //    //    pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                //    //}
                //    //pipeline.AddLast(new LoggingHandler());
                //    //pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                //    //pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                //    pipeline.AddLast("echo", new NettyClientHandler(this));
                //}));

                serverChannel = (SocketDatagramChannel) await bootstrap.BindAsync(TestNettyUdpClient.remoteAddress);

                NLog(TestNettyUdpClient.remoteAddress2, "$ni.name : $ni.displayName");

                //            ch.joinGroup(groupAddress, ni).sync();

                NLog(TestNettyUdpClient.remoteAddress2, "udp server($groupAddress.hostName:$groupAddress.port) is running...");

                //await serverChannel.CloseAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
                StopClientChannel();
            }
            finally
            {
                //await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            }
        }


        void Start()
        {
            StartLink();
        }


        [ContextMenu("StartLink")]
        private void StartLink()
        {
            clientThread = new Thread(new ThreadStart(() =>
            {
                RunClientAsync().Wait();
            }));
            clientThread.Start();
        }

        public async Task CloseChannelSafe()
        {
            NLog(TestNettyUdpClient.remoteAddress2, "关闭本地频道...");
            await serverChannel.CloseAsync();
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }

        [ContextMenu("StopClientChannel")]
        public void StopClientChannel()
        {
            Task.Run(() => CloseChannelSafe());
        }
    }
}
