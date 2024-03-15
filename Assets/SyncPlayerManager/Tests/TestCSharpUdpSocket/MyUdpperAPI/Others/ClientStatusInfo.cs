using System.Net;
using System.Xml.Linq;

namespace GoldSprite.MyUdpperAPI
{
    public class ClientStatusInfo
    {
        public string Name { get; set; }
        public IdentityType Identity {  get; set; }
        public EndPoint Address { get; set; }

        public long VisitTimeMillis;

        public bool IsLogined => Identity != IdentityType.Stranger;
    }
}