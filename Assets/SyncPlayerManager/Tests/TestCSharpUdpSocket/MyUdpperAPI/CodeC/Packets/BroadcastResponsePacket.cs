

namespace GoldSprite.MyUdpperAPI
{
    public class BroadcastResponsePacket : ResponsePacket
    {
        public string Message;
        public override byte Command => ICommand.BROADCAST_RESPONSE;
    }
}
