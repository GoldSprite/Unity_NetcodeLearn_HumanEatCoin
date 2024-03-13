using DotNetty.Common.Concurrency;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;

namespace GoldSprite.TestDotNetty_API
{
    public class PacketDecodeHandler : ChannelHandlerAdapter
    {
        private bool isServer => NetworkManager.Singleton.NetTrans.IsServer;


        public override void ExceptionCaught(IChannelHandlerContext context, Exception e)
        {
            LogTools.NLogErr("处理器异常: \n" + e.StackTrace);
        }


        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
        }


        //这个步骤是为了Java-C#通信而准备的(JC互通无法自动识别DatagramPacket格式会被筛掉而收不到包)
        public override void ChannelRead(IChannelHandlerContext ctx, Object msg)
        {
            LogTools.NLogDebug("接收到数据..."+msg.ToString());
            if (msg == null || !(msg is DatagramPacket)) throw new Exception("数据包格式异常.");
            DatagramPacket dpk = (DatagramPacket)msg;
            ctx.FireChannelRead(dpk);
        }
    }
}