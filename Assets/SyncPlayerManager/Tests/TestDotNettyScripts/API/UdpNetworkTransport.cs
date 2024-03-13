using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GoldSprite.TestDotNetty_API
{
    public class UdpNetworkTransport
    {
        private PacketHandler packetHandler;
        private IChannel channel;
        public IChannel Channel => channel;


        private Dictionary<string, ClientStatusInfo> clients = new Dictionary<string, ClientStatusInfo>();
        public Dictionary<string, ClientStatusInfo> Clients => clients;


        public bool IsServer => NetworkManager.Singleton.StartType == StartType.Server;


        public void InitHandlers(IChannel channel)
        {
            this.channel = channel;
            var pipeline = channel.Pipeline;
            //pipeline.AddLast(new PacketDecodeHandler());
            pipeline.AddLast(packetHandler = new PacketHandler());
            //pipeline.AddLast(new PacketEncodeHandler());
        }


        public void SendPacket<T>(Packet pk, Action<T> callback) where T : ResponsePacket
        {
            packetHandler.AddCallbackListener(callback);
            SendPacket(pk);
        }


        public void SendPacket(Packet pk)
        {
            channel.WriteAndFlushAsync(pk);
        }


        public bool IsOnline(string guid)
        {
            if (String.IsNullOrEmpty(guid)) return false;
            return clients.ContainsKey(guid);
        }


        public bool IsRepeatAddress(EndPoint address)
        {
            return clients.Values.Any(p => p.Address.Equals(address));
        }


        public void RemoveClient(EndPoint address)
        {
            foreach (var item in clients)
            {
                if (item.Value.Address.Equals(address))
                {
                    clients.Remove(item.Key);
                    return;
                }
            }
        }


        public void NewClient(Packet pk, EndPoint address)
        {
            var newGuid = MathTools.NewCustomGuid();
            pk.Guid = newGuid.ToString();

            var client = new ClientStatusInfo()
            {
                Identity = IdentityType.Stranger,
                Address = address
            };
            clients.Add(newGuid, client);
        }
    }
}