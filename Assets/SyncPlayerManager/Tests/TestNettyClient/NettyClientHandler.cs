using DotNetty.Buffers;
using DotNetty.Common.Concurrency;
using DotNetty.Transport.Channels;
using System;
using System.Text;
using System.Threading;
using UnityEngine;

namespace GoldSprite.TestDotNetty
{
    public class NettyClientHandler : SimpleChannelInboundHandler<object>
    {
        private TestNettyClient3 main;


        public NettyClientHandler(TestNettyClient3 main)
        {
            this.main = main;
        }


        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            Debug.Log("频道已激活.");

            var str = "你好你好你好你好你好你好你好你好你好你好你好你好你好你好你好你好.";
            Debug.Log("发送消息: "+str);
            IByteBuffer byteBuf = ctx.Allocator.Buffer();
            byteBuf.WriteString(str, Encoding.UTF8);

            ctx.Channel.WriteAndFlushAsync(byteBuf).Wait();
            Debug.Log("已完成发送.");
        }


        public override void ChannelInactive(IChannelHandlerContext context)
        {
            Debug.Log("频道已关闭.");
            //main.CloseChannelSafe();
        }


        protected override void ChannelRead0(IChannelHandlerContext ctx, object msg)
        {
            var buf = (IByteBuffer)msg;
            String strMsg = buf.ToString(Encoding.UTF8);
            Debug.Log("读到消息..."+ strMsg);
        }
    }
}