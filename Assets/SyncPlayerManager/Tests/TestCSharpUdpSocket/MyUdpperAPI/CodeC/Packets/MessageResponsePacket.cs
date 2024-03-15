

namespace GoldSprite.MyUdpperAPI
{
    public class MessageResponsePacket : ResponsePacket
    {
        public override byte Command => ICommand.MESSAGE_RESPONSE;
    }
}