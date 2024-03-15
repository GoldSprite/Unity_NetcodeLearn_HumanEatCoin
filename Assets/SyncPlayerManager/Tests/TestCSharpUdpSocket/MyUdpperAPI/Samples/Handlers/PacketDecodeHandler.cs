


using DotNetty.Transport.Channels.Sockets;
using System;
using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public class PacketDecodeHandler : InboundHandlerAdapter {
        private bool isServer => Udpper.Singleton.NetTrans.IsServer;


        public override void ExceptionCaught(IHandlerContext ctx, Exception e)
        {
            LogTools.NLogErr("处理器异常: \n" + e.StackTrace);
        }

        public override void Read(IHandlerContext ctx, object msg)
        {
            if (msg == null || !(msg is DatagramPacket))
                throw new Exception("数据包格式异常.");
            DatagramPacket dpk = (DatagramPacket)msg;
            ctx.FireChannelRead(dpk);
        }
    }
}