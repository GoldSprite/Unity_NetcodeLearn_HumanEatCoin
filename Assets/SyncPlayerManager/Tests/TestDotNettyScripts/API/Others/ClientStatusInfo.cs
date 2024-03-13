using System.Net;

namespace GoldSprite.TestDotNetty_API
{
    public class ClientStatusInfo
    {
        public IdentityType Identity {  get; set; }
        public EndPoint Address { get; set; }
    }
}