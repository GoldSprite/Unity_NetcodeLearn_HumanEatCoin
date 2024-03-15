using DotNetty.Transport.Channels.Sockets;
using DotNetty.Transport.Channels;
using System.Net;
using System.Threading.Tasks;
using System;

namespace GoldSprite.MyUdpperAPI {
    public class PacketEncodeHandler : SimpleOutboundHandler<Packet> {
        private bool isServer => Udpper.Singleton.NetTrans.IsServer;
        private EndPoint recipient;


        public override void ExceptionCaught(IHandlerContext ctx, Exception e)
        {
            LogTools.NLogErr("处理器异常: \n" + e.StackTrace);
        }

        public override void Flush(IHandlerContext ctx)
        {
            ctx.Flush();
        }

        public override void Write0(IHandlerContext ctx, Packet pk)
        {
            DatagramPacket dpk = EncodeAuthentication(pk, ctx);
            if (dpk == null) throw new Exception("发包验证不通过");
            //var buf = PacketCodeC.INSTANCE.Encode(ctx.Alloc(), pk);
            ctx.Write(dpk);
        }


        //将Packet包裹为DatagramPacket
        private DatagramPacket EncodeAuthentication(Packet pk, IHandlerContext ctx)
        {
            var info = "发包验证异常: ";
            EndPoint networkAddress = Udpper.Singleton.ConnectData.RemoteAddress;
            if (isServer) {
                if(pk.Guid == null) {
                    LogTools.NLogInfo(info + "目标玩家Guid为空(是否忘记手动填写?).");
                    return null;
                }
                if (!Udpper.Singleton.NetTrans.IsOnline(pk.Guid)) {
                    LogTools.NLogInfo(info + "该客户端没有录入.");
                    return null;
                }
                ClientStatusInfo client = Udpper.Singleton.NetTrans.Clients[pk.Guid];
                networkAddress = client.Address;
                //LogTools.NLogDebug("目标玩家地址: " + networkAddress);
            }
            if (networkAddress == null) {
                LogTools.NLogInfo(info + "发送地址为空.");
                return null;
            }
            DatagramPacket dpk = PacketCodeC.INSTANCE.EncodeDpk(ctx.Alloc(), pk, networkAddress);
            return dpk;
        }
    }
}