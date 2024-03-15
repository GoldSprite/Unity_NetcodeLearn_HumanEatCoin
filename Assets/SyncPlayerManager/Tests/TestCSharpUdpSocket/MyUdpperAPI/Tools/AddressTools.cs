using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoldSprite.MyUdpperAPI {
    public class AddressTools {
        // 将 EndPoint 转换为字节数组
        public static byte[] EndPointToBytes(IPEndPoint endPoint)
        {
            byte[] addressBytes = endPoint.Address.GetAddressBytes();
            byte[] portBytes = BitConverter.GetBytes((ushort)endPoint.Port);

            // 组合 IP 地址和端口信息到一个字节数组中
            byte[] data = new byte[addressBytes.Length + portBytes.Length];
            Array.Copy(addressBytes, 0, data, 0, addressBytes.Length);
            Array.Copy(portBytes, 0, data, addressBytes.Length, portBytes.Length);

            return data;
        }

        // 将字节数组转换回 EndPoint
        public static IPEndPoint BytesToEndPoint(byte[] data)
        {
            // 解析 IP 地址
            IPAddress ipAddress = new IPAddress(new byte[] { data[0], data[1], data[2], data[3] });

            // 解析端口
            int port = BitConverter.ToUInt16(new byte[] { data[4], data[5] }, 0);

            return new IPEndPoint(ipAddress, port);
        }
    }
}
