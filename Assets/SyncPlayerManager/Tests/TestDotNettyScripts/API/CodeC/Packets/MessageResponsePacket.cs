namespace GoldSprite.TestDotNetty_API
{
    public class MessageResponsePacket : ResponsePacket
    {
        public override byte Command => ICommand.MESSAGE_RESPONSE;
    }
}