using DotNetty.Buffers;
using DotNetty.Common.Concurrency;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Local;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Text;
using System.Threading;
using UnityEngine;

using static GoldSprite.LogTools;
using static GoldSprite.TestDotNetty.TestNettyUdpClient;

namespace GoldSprite.TestDotNetty
{
    public class NettyUdpChannelInboundHandler : SimpleChannelInboundHandler<DatagramPacket>
    {
        bool isResponse;


        public NettyUdpChannelInboundHandler(bool isResponse)
        {
            this.isResponse = isResponse;
        }


        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket msg)
        {
            var buf = msg.Content;
            String strMsg = buf.ToString(Encoding.UTF8);
            NLog(remoteAddress2, "recv: "+ strMsg);

            if (isResponse)
            {
                IByteBuffer buf1 = new UnpooledByteBufferAllocator(true).Buffer();
                buf1.WriteBytes(Encoding.UTF8.GetBytes("ok"));

                var packet = new DatagramPacket(buf1, msg.Sender);

                ctx.WriteAndFlushAsync(packet).Wait();
            }
        }
    }
}