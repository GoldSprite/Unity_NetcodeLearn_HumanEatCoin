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

namespace GoldSprite.TestDotNetty
{
    public class TestNettyUdpClient : MonoBehaviour
    {
        public Thread clientThread;
        private MultithreadEventLoopGroup group;
        private IChannel clientChannel;
        private IChannel clientChannel2;


        public const String addr1 = "10.0.0.2";
        public const String addr1_w = "162.14.68.248";
        public const String addr2 = "192.168.1.105";
        public const String addr2_w = "112.195.244.151";
        public static IPEndPoint remoteAddress = new IPEndPoint(IPAddress.Parse(addr2), 34001);  //localhost, 8888
        public static IPEndPoint remoteAddress2 = new IPEndPoint(IPAddress.Parse(addr2), 34001);  //localhost, 8888
        public static IPEndPoint localAddress = new IPEndPoint(IPAddress.Parse(addr2), 30001);  //localhost, 8888
        public static IPEndPoint localAddress2 = new IPEndPoint(IPAddress.Parse(addr2_w), 30001);  //localhost, 8888

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
                    .Handler(new ActionChannelInitializer<IChannel>((ch =>
                    {
                        IChannelPipeline pipeline = ch.Pipeline;
                        pipeline.AddLast(new NettyUdpChannelInboundHandler(false));
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

                clientChannel2 = (IChannel) await bootstrap.BindAsync(localAddress);
                if(clientChannel2.RemoteAddress != null)
                    await clientChannel2.ConnectAsync(remoteAddress2);

                //while (serverChannel.Active)
                //{
                //    Task.Delay(1000).Wait();
                //}


                //var str = "我是天选, 也是抉择!";
                //NLog(localAddress2, "发送消息: " + str);
                //IByteBuffer byteBuf = new UnpooledByteBufferAllocator(true).Buffer();
                //byteBuf.WriteString(str, Encoding.UTF8);

                //var packet = new DatagramPacket(byteBuf, remoteAddress2);
                //await serverChannel.WriteAndFlushAsync(packet);
                //NLog(localAddress2, "已完成发送.");


                for (int i = 0; i < 1; i++)
                {
                    var str = "你好你好.";
                    NLog(localAddress2, "发送消息: " + str);
                    IByteBuffer byteBuf = clientChannel2.Allocator.Buffer();
                    var bytes = Encoding.UTF8.GetBytes(str);
                    byteBuf.WriteBytes(bytes);

                    var packet = new DatagramPacket(byteBuf, remoteAddress2);
                    await clientChannel2.WriteAndFlushAsync(packet);
                    NLog(localAddress2, "已完成发送.");
                }

                NLog(localAddress2, "线程任务已结束.");

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
            NLog(localAddress2, "关闭本地频道...");
            //await clientChannel.CloseAsync();
            await clientChannel2.CloseAsync();
            await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
            NLog(localAddress2, "关闭结束.");
        }

        [ContextMenu("StopClientChannel")]
        public void StopClientChannel()
        {
            Task.Run(() => CloseChannelSafe());
        }
    }
}
