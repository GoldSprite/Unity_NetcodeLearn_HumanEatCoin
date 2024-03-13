using DotNetty.Buffers;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace GoldSprite.TestDotNetty_API {
    internal class PacketHandler : SimpleChannelInboundHandler<DatagramPacket> {
        private bool isServer => NetworkManager.Singleton.NetTrans.IsServer;
        private Dictionary<Type, List<Action<ResponsePacket>>> callbacks = new Dictionary<Type, List<Action<ResponsePacket>>>();


        public override void ExceptionCaught(IChannelHandlerContext context, Exception e)
        {
            LogTools.NLogErr("处理器异常: \n" + e.StackTrace);
        }


        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            LogTools.NLogInfo("频道已激活.");


            IByteBuffer buf = ctx.Allocator.Buffer();
            buf.WriteBytes(Encoding.UTF8.GetBytes("你好"));
            DatagramPacket dpk = new DatagramPacket(buf, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8007));
            LogTools.NLogDebug("已打包1.");

            ctx.WriteAndFlushAsync(dpk).Wait();

            buf = ctx.Allocator.Buffer();
            buf.WriteBytes(Encoding.UTF8.GetBytes("你好"));
            dpk = new DatagramPacket(buf, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9007));
            LogTools.NLogDebug("已打包2.");

            ctx.WriteAndFlushAsync(dpk).Wait();
        }


        public override void ChannelInactive(IChannelHandlerContext context)
        {
            LogTools.NLogInfo("频道已关闭.");
        }


        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            ctx.Flush();
        }


        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            LogTools.NLogInfo("读数据: "+msg.ToString());
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket msg)
        {
            LogTools.NLogInfo("读数据: " + msg.ToString());
        }
        /*
        protected override void ChannelRead0(IChannelHandlerContext ctx, DatagramPacket dpk)
        {
            var sender = dpk.Sender;
            var packet = PacketCodeC.INSTANCE.Decode(dpk.Content);
            LogTools.NLogDebug("收到包类型: "
                + packet.GetType().Name + ": "
                + packet.ToString()
                );


            if (isServer) {
                //识别客户端(给访问ip进行编号)
                //识别重登挤号
                if (NetworkManager.Singleton.NetTrans.IsRepeatAddress(sender))
                    NetworkManager.Singleton.NetTrans.RemoveClient(sender);
                //新客户端(Stranger)录入
                if (!NetworkManager.Singleton.NetTrans.IsOnline(packet.Guid))
                    NetworkManager.Singleton.NetTrans.NewClient(packet, sender);
            }
            //if (Server.Instance.strangerIntercept)
            //    if (isServer)
            //    {
            //        var intercept = strangerInterceptor(packet);
            //        if (intercept)
            //        {
            //            LogTools.NLogInfo("已拦截未登录者消息.");
            //            var rep = new LoginResponsePacket(packet.getOwnerGuid(), IStatus.RETURN_DEFEAT_LOGIN_NOTLOGIN);
            //            Server.Instance.sendPacket(rep);
            //            return;
            //        }
            //    }

            switch (packet.Command) {
                case ICommand.MESSAGE_REQUEST: {
                    MessageRequestPacket pk = (MessageRequestPacket)packet;
                    HandleMessageRequestPacket(pk);
                    break;
                }
                case ICommand.MESSAGE_RESPONSE: {
                    MessageResponsePacket pk = (MessageResponsePacket)packet;
                    HandleMessageResponsePacket(pk);
                    break;
                }

                default:
                    break;
            }

            if (packet is ResponsePacket repPk)
                Callback(repPk);
        }
        */

        private void HandleMessageResponsePacket(MessageResponsePacket pk)
        {
        }


        private void HandleMessageRequestPacket(MessageRequestPacket pk)
        {
            var repPk = new MessageResponsePacket() { Guid = pk.Guid, RepCode = IStatus.RETURN_SUCCESS/*, Reson = "收到客户端消息"*/ };
            NetworkManager.Singleton.NetTrans.SendPacket(repPk);
        }


        public void AddCallbackListener<T>(Action<T> callback) where T : ResponsePacket
        {
            var ppid = typeof(T);
            if (!callbacks.ContainsKey(ppid))
                callbacks.Add(ppid, new List<Action<ResponsePacket>>());
            var pkCallbacks = callbacks[ppid];
            pkCallbacks.Add(pk => {
                LogTools.NLogDebug(pk.ResonMsg);  //响应信息
                callback?.Invoke((T)pk);
            });
        }


        private void Callback(ResponsePacket pk)
        {
            var pkCallbacks = callbacks[pk.GetType()];
            if (pkCallbacks != null && pkCallbacks.Count > 0) {
                pkCallbacks.ForEach(p => p?.Invoke(pk));
                pkCallbacks.Clear();
            }
        }
    }
}