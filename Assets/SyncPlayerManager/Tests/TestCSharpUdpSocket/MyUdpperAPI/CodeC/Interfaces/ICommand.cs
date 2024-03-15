namespace GoldSprite.MyUdpperAPI
{
    public class ICommand
    {
        private ICommand() { }


        public const byte MESSAGE_REQUEST = 1;
        public const byte MESSAGE_RESPONSE = 2;

        public const byte BROADCAST_REQUEST = 3;
        public const byte BROADCAST_RESPONSE = 4;

        public const byte LOGIN_REQUEST = 5;
        public const byte LOGIN_RESPONSE = 6;
    }
}