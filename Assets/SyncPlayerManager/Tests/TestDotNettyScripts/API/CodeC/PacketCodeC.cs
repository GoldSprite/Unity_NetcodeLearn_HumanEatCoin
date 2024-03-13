using DotNetty.Buffers;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels.Sockets;
using System;
using System.Collections.Generic;
using System.Net;

namespace GoldSprite.TestDotNetty_API
{
    public class PacketCodeC
    {
        public const int MAGIC_NUMBER = 0x31699784;  //0x开头接8位数: 编码识别id
        private static readonly Dictionary<byte, ISerializer> serializerMap = new Dictionary<byte, ISerializer>();  //编码器id
        private static readonly Dictionary<byte, Type> packetTypeMap = new Dictionary<byte, Type>();  //包类型id
        public static readonly PacketCodeC INSTANCE = new PacketCodeC();


        public PacketCodeC()
        {
            ISerializer serializer = new JSONSerializer();
            serializerMap.Add(serializer.SerializerAlgorithm, serializer);

            packetTypeMap.Add(ICommand.MESSAGE_REQUEST, typeof(MessageRequestPacket));
            packetTypeMap.Add(ICommand.MESSAGE_RESPONSE, typeof(MessageResponsePacket));
        }


        public IByteBuffer Encode(IByteBufferAllocator alloc, Packet packet)
        {
            // 1. 创建 ByteBuf 对象
            IByteBuffer buf = alloc.Buffer();
            // 2. 序列化 Java 对象
            byte[] bytes = ISerializer.DEFAULT.Serialize(packet);
            // 3. 实际编码过程，把通信协议几个部分，一一编码
            buf.WriteInt(MAGIC_NUMBER);
            buf.WriteByte(packet.Version);
            buf.WriteByte(ISerializer.DEFAULT.SerializerAlgorithm);
            buf.WriteByte(packet.Command);
            buf.WriteInt(bytes.Length);
            buf.WriteBytes(bytes);
            return buf;
        }


        //解码
        public Packet Decode(IByteBuffer buf)
        {
            // 跳过魔数
            buf.SkipBytes(4);
            // 跳过版本号
            buf.SkipBytes(1);
            // 序列化算法标识
            byte serializeAlgorithm = buf.ReadByte();
            // 指令
            byte command = buf.ReadByte();
            // 数据包长度
            int length = buf.ReadInt();
            byte[] bytes = new byte[length];
            buf.ReadBytes(bytes);
            Type requestType = GetRequestType(command);
            ISerializer serializer = GetSerializer(serializeAlgorithm);
            if (requestType != null && serializer != null)
            {
                return (Packet)serializer.Deserialize(bytes, requestType);
            }
            return null;
        }


        private ISerializer GetSerializer(byte serializerAlgorithm)
        {
            if (!serializerMap.ContainsKey(serializerAlgorithm)) return null;
            return serializerMap[serializerAlgorithm];
        }


        private Type GetRequestType(byte command)
        {
            if (!packetTypeMap.ContainsKey(command)) return null;
            return packetTypeMap[command];
        }


        public DatagramPacket EncodeDpk(IByteBufferAllocator alloc, Packet pk, EndPoint target)
        {
            var buf = Encode(alloc, pk);
            var dpk = new DatagramPacket(buf, target);
            return dpk;
        }
    }
}