

namespace GoldSprite.MyUdpperAPI
{
    public class LoginRequestPacket : Packet
    {
        public string UserName;
        public string Password;
        public override byte Command => ICommand.LOGIN_REQUEST;
    }
}
