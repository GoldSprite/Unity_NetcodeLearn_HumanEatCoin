using System.Net;

namespace GoldSprite.TestDotNetty_API
{
    public class GlobalConfiguation
    {
        public static IPEndPoint LocalAddress => new IPEndPoint(IPAddress.Parse("192.168.1.105"), 9007);
        public static IPEndPoint LocalAddress2 => new IPEndPoint(IPAddress.Parse("192.168.1.105"), 34002);
        public static IPEndPoint LocalServerAddress => new IPEndPoint(IPAddress.Parse("192.168.1.105"), 34002);
        public static IPEndPoint CloudServerAddress => new IPEndPoint(IPAddress.Parse("162.14.68.248"), 34002);  //正式34001, 测试34002
    }
}