using System.Collections.Generic;
using System;
using DotNetty.Transport.Channels.Sockets;
using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public class PacketDispatchHandler : SimpleInboundHandler<DatagramPacket> {
        private bool isServer => Udpper.Singleton.NetTrans.IsServer;
        private readonly Dictionary<Type, List<Action<ResponsePacket>>> callbacks = new Dictionary<Type, List<Action<ResponsePacket>>>();


        public override void ExceptionCaught(IHandlerContext ctx, Exception e)
        {
            LogTools.NLogErr("处理器异常: \n" + e.StackTrace);
        }

        public override void Read0(IHandlerContext ctx, DatagramPacket dpk)
        {
            var sender = dpk.Sender;
            var packet = PacketCodeC.INSTANCE.Decode(dpk.Content);
            LogTools.NLogDebug("收到包类型: "
                + packet.GetType().Name + ": "
                + packet.ToString()
                );

            if (isServer) {
                //新客户端(Stranger)录入
                if (!Udpper.Singleton.NetTrans.IsOnline(packet.Guid)) {
                    Udpper.Singleton.NetTrans.NewClient(packet, sender);
                    LogTools.NLogDebug("新的客户端访问.");
                }

                if (!(packet is LoginRequestPacket)) {
                    //陌生者拦截
                    var client = Udpper.Singleton.NetTrans.GetClient(packet.Guid);
                    if (Udpper.LoginAuth && !client.IsLogined) {
                        LogTools.NLogDebug("陌生者信息拦截.");
                        var rep = new LoginResponsePacket() { Guid = packet.Guid, RepCode = IStatus.RETURN_DEFEAT, Reason = "服务器开启了登录验证, 请先登录." };
                        Udpper.Singleton.NetTrans.SendPacket(rep);
                        return;
                    }
                }
            }

            switch (packet.Command) {
                case ICommand.LOGIN_REQUEST: {
                    LoginRequestPacket pk = (LoginRequestPacket)packet;
                    HandleLoginRequestPacket(pk, sender);
                    break;
                }
                case ICommand.LOGIN_RESPONSE: {
                    LoginResponsePacket pk = (LoginResponsePacket)packet;
                    HandleLoginResponsePacket(pk);
                    break;
                }
                case ICommand.BROADCAST_REQUEST: {
                    BroadcastRequestPacket pk = (BroadcastRequestPacket)packet;
                    HandleBroadcastRequestPacket(pk, sender);
                    break;
                }
                case ICommand.BROADCAST_RESPONSE: {
                    BroadcastResponsePacket pk = (BroadcastResponsePacket)packet;
                    HandleBroadcastResponsePacket(pk);
                    break;
                }
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


        private void HandleLoginResponsePacket(LoginResponsePacket pk)
        {
            LogTools.NLogInfo($"登录响应: {pk.ReasonMsg}.");
            Udpper.Singleton.NetTrans.Guid = pk.Guid;
        }

        private void HandleLoginRequestPacket(LoginRequestPacket pk, EndPoint sender)
        {
            var repMsg = LoginAuthentication(pk, sender);
            var pass = repMsg == "";
            var rep = new LoginResponsePacket { Guid = pk.Guid, RepCode = IStatus.RETURN_DEFEAT, Reason = repMsg };
            if (pass) {
                rep.RepCode = IStatus.RETURN_SUCCESS;
                var client = Udpper.Singleton.NetTrans.GetClient(pk.Guid);
                client.Identity = IdentityType.User;
                client.Name = pk.UserName;
            }
            Udpper.Singleton.NetTrans.SendPacket(rep);
        }

        private void HandleBroadcastResponsePacket(BroadcastResponsePacket pk)
        {
            LogTools.NLogMsg("收到广播消息: " + pk.Message);
        }

        public void HandleBroadcastRequestPacket(BroadcastRequestPacket pk, EndPoint sender)
        {
            var clients = Udpper.Singleton.NetTrans.Clients;
            if (clients.Count > 0) {
                var isServer = sender.Equals(Udpper.Singleton.ConnectData.LocalAddress);
                var senderClient = Udpper.Singleton.NetTrans.GetClient(pk.Guid);
                var identityMsg = isServer ? "服务器" : "玩家-" + senderClient?.Name;
                foreach (var pair in clients) {
                    var playerGuid = pair.Key;
                    var msg = $"[{identityMsg}]发送了广播: " + pk.Message;
                    var rep = new BroadcastResponsePacket() { Message = msg, Guid = playerGuid };
                    Udpper.Singleton.NetTrans.SendPacket(rep);
                }
                LogTools.NLogInfo($"广播成功到[{clients.Count}]名玩家.");
            } else {
                LogTools.NLogInfo($"广播失败, 无玩家在线.");
            }
        }

        private void HandleMessageResponsePacket(MessageResponsePacket pk)
        {
        }

        private void HandleMessageRequestPacket(MessageRequestPacket pk)
        {
            var repPk = new MessageResponsePacket() { Guid = pk.Guid, RepCode = IStatus.RETURN_SUCCESS/*, Reason = "收到客户端消息"*/ };
            Udpper.Singleton.NetTrans.SendPacket(repPk);
        }


        private string LoginAuthentication(LoginRequestPacket pk, EndPoint sender)
        {
            var statusMsg = "";
            var isRepeatGuid = Udpper.Singleton.NetTrans.IsLogined(pk.Guid);
            //var ipRepeatAddress = Udpper.Singleton.NetTrans.GetClient(sender)!=null;  //已经在前置阶段处理
            if (isRepeatGuid) {
                statusMsg = "重复登录";
            }
            if (statusMsg != "") LogTools.NLogInfo(statusMsg);
            return statusMsg;
        }

        public void AddCallbackListener<T>(Action<T> callback) where T : ResponsePacket
        {
            var ppid = typeof(T);
            if (!callbacks.ContainsKey(ppid)) callbacks.Add(ppid, new List<Action<ResponsePacket>>());
            var pkCallbacks = callbacks[ppid];
            pkCallbacks.Add(pk => {
                //LogTools.NLogDebug(pk.ReasonMsg);  //响应信息
                callback?.Invoke((T)pk);
            });
        }


        private void Callback(ResponsePacket pk)
        {
            var ppid = pk.GetType();
            if (!callbacks.ContainsKey(ppid)) return;
            var pkCallbacks = callbacks[ppid];
            if (pkCallbacks != null && pkCallbacks.Count > 0) {
                pkCallbacks.ForEach(p => p?.Invoke(pk));
                pkCallbacks.Clear();
            }
        }
    }
}