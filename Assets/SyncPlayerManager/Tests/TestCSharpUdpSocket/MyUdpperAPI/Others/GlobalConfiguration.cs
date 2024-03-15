using System.Net;

namespace GoldSprite.MyUdpperAPI
{
    public class GlobalConfiguration {
        public static IPEndPoint LocalAddress => new IPEndPoint(IPAddress.Parse("192.168.1.105"), 9007);
        public static IPEndPoint LocalAddress2 => new IPEndPoint(IPAddress.Parse("192.168.1.105"), 34002);
        public static IPEndPoint LocalServerAddress => new IPEndPoint(IPAddress.Parse("192.168.1.105"), 34002);
        public static IPEndPoint CloudServerAddress => new IPEndPoint(IPAddress.Parse("162.14.68.248"), 34002);  //正式34001, 测试34002

#if UNITY_2017_1_OR_NEWER
        public static bool CLICMD_Enabled = false;
#elif APPLICATION_CONSOLE || APPLICATION_CLOUD
        public static bool CLICMD_Enabled = true;
#endif
    }
}