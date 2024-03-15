

namespace GoldSprite.MyUdpperAPI
{
    public class LoginResponsePacket : ResponsePacket
    {
        public override byte Command => ICommand.LOGIN_RESPONSE;
    }
}
