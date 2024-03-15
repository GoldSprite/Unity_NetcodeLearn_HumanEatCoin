

namespace GoldSprite.MyUdpperAPI
{
    public class BroadcastRequestPacket : Packet
    {
        public string Message { get; set; }
        public override byte Command => ICommand.BROADCAST_REQUEST;
    }
}
