using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GoldSprite.MyUdpperAPI {
    public class UdpNetworkTransport {
        private PacketDispatchHandler packetHandler;
        public PacketDispatchHandler PacketHandler => packetHandler;
        private HandlerChannel channel;
        public HandlerChannel Channel => channel;
        private Dictionary<string, ClientStatusInfo> clients = new Dictionary<string, ClientStatusInfo>();
        public Dictionary<string, ClientStatusInfo> Clients => clients;

        private string guid;
        public string Guid { get => guid; set => guid = value; }


        public void InitHandlers(HandlerChannel channel)
        {
            this.channel = channel;
            channel.AddLast(new PacketDecodeHandler());
            channel.AddLast(packetHandler = new PacketDispatchHandler());
            channel.AddLast(new PacketEncodeHandler());
        }


        public void SendPacket<T>(Packet pk, Action<T> callback) where T : ResponsePacket
        {
            packetHandler.AddCallbackListener(callback);
            SendPacket(pk);
        }


        public void SendPacket(Packet pk)
        {
            //客户端自动填自身Guid
            if (!IsServer) pk.Guid = Guid;
            channel.WriteAndFlushAsync(pk);
        }


        public ClientStatusInfo GetClient(string guid)
        {
            if (guid == null || !clients.ContainsKey(guid)) return null;
            return clients[guid];
        }

        public ClientStatusInfo GetClient(EndPoint point)
        {
            if (clients.Count == 0) return null;
            var client = clients.Values.First(p => p.Address.Equals(point));
            if (client == null) return null;
            return client;
        }

        public string GetGuid(ClientStatusInfo client)
        {
            if (!clients.ContainsValue(client)) return null;
            return clients.First(p => p.Value == client).Key;
        }

        public bool IsServer => Udpper.Singleton.StartType == StartType.Server;

        public bool IsServerPoint(EndPoint point)
        {
            if (IsServer) return Udpper.Singleton.ConnectData.LocalAddress.Equals(point);
            return Udpper.Singleton.ConnectData.RemoteAddress.Equals(point);
        }

        public bool IsOnline(string guid)
        {
            if (String.IsNullOrEmpty(guid)) return false;
            return clients.ContainsKey(guid);
        }

        public bool IsLogined(string guid)
        {
            return IsOnline(guid) && clients[guid].IsLogined;
        }


        public bool IsRepeatAddress(EndPoint address)
        {
            return clients.Values.Any(p => p.IsLogined && p.Address.Equals(address));
        }


        public void RemoveClient(EndPoint address)
        {
            foreach (var item in clients) {
                if (item.Value.Address.Equals(address)) {
                    clients.Remove(item.Key);
                    return;
                }
            }
        }

        public string IdentityName(EndPoint sender)
        {
            var senderClient = GetClient(sender);
            var identityName = IsServerPoint(sender) ? "服务器" : "玩家<" + (GetGuid(senderClient) ?? "无") + ":" + (senderClient == null ? "未知玩家" : senderClient.Name) + ">";
            return identityName;
        }


        public void NewClient(Packet pk, EndPoint address)
        {
            var identity = IdentityType.Stranger;

            //识别重登挤号
            var oldClient = GetClient(address);
            if (oldClient != null) {
                RemoveClient(address);

                if(oldClient.IsLogined) {
                    LogTools.NLogDebug("重登挤号.");
                    identity = IdentityType.User;
                    LogTools.NLogDebug("自动重登.");
                }
            }

            var newGuid = MathTools.NewCustomGuid();
            pk.Guid = newGuid.ToString();

            var client = new ClientStatusInfo() {
                Name = "无名者", 
                Identity = identity,
                Address = address,
                VisitTimeMillis = DateTools.CurrentTimeMillis(),
            };
            clients.Add(newGuid, client);
        }
    }
}