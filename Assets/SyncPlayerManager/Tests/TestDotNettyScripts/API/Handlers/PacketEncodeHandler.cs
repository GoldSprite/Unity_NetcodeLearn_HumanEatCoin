using DotNetty.Common.Concurrency;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Net;
using System.Threading.Tasks;

namespace GoldSprite.TestDotNetty_API
{
    public class PacketEncodeHandler : ChannelHandlerAdapter
    {
        private bool isServer => NetworkManager.Singleton.NetTrans.IsServer;


        public override void ExceptionCaught(IChannelHandlerContext ctx, Exception e)
        {
            LogTools.NLogErr("处理器异常: \n" + e.StackTrace);
        }


        public override void Flush(IChannelHandlerContext ctx)
        {
            ctx.Flush();
        }


        public override Task WriteAsync(IChannelHandlerContext ctx, object msg)
        {
            LogTools.NLogDebug("检测到待发送数据...");
            var packet = (Packet)msg;
            LogTools.NLogDebug("打包待发送数据...");
            DatagramPacket dpk = EncodeAuthentication(packet, ctx);
            if (dpk != null) {
                LogTools.NLogDebug("已打包.");
                return ctx.WriteAsync(dpk);
            }
            throw new Exception("发包验证不通过");
        }


        //将Packet包裹为DatagramPacket
        private DatagramPacket EncodeAuthentication(Packet pk, IChannelHandlerContext ctx)
        {
            var info = "发包验证异常: ";
            EndPoint networkAddress = NetworkManager.Singleton.ConnectData.remoteAddress;
            if (isServer)
            {
                if (!NetworkManager.Singleton.NetTrans.IsOnline(pk.Guid))
                {
                    LogTools.NLogInfo(info + "该玩家不在线上.");
                    return null;
                }
                ClientStatusInfo client = NetworkManager.Singleton.NetTrans.Clients[pk.Guid];
                networkAddress = client.Address;
                //LogTools.NLogDebug("目标玩家地址: " + networkAddress);
            }
            if (networkAddress == null)
            {
                LogTools.NLogInfo(info + "发送地址为空.");
                return null;
            }
            DatagramPacket dpk = PacketCodeC.INSTANCE.EncodeDpk(ctx.Allocator, pk, networkAddress);
            return dpk;
        }
    }
}