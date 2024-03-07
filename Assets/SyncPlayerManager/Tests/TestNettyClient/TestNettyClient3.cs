using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

namespace GoldSprite.TestDotNetty
{
    public class TestNettyClient3 : MonoBehaviour
    {
        public Thread clientThread;
        private MultithreadEventLoopGroup group;
        private IChannel clientChannel;

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
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true)
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        //if (cert != null)
                        //{
                        //    pipeline.AddLast("tls", new TlsHandler(stream => new SslStream(stream, true, (sender, certificate, chain, errors) => true), new ClientTlsSettings(targetHost)));
                        //}
                        //pipeline.AddLast(new LoggingHandler());
                        //pipeline.AddLast("framing-enc", new LengthFieldPrepender(2));
                        //pipeline.AddLast("framing-dec", new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 2, 0, 2));

                        pipeline.AddLast("echo", new NettyClientHandler(this));
                    }));

                clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("192.168.1.105"), 8001));

                //while (clientChannel.Active)
                //{
                //    Task.Delay(1000).Wait();
                //}
                Debug.Log("线程任务已结束.");

                //await clientChannel.CloseAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
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
            Debug.Log("关闭本地频道...");
            await clientChannel.CloseAsync();
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
        }

        [ContextMenu("StopClientChannel")]
        public void StopClientChannel()
        {
            Task.Run(() => CloseChannelSafe());
        }
    }
}
