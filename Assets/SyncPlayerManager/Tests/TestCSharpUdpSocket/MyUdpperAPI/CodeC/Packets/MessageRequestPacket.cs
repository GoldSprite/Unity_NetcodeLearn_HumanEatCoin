

namespace GoldSprite.MyUdpperAPI
{
    public class MessageRequestPacket : Packet
    {
        public string Message { get; set; }
        public override byte Command => ICommand.MESSAGE_REQUEST;
    }
}
