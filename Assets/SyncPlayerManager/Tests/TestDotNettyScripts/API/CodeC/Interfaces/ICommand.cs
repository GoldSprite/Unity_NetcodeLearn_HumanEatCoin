namespace GoldSprite.TestDotNetty_API
{
    public class ICommand
    {
        private ICommand() { }


        public const byte MESSAGE_REQUEST = 1;
        public const byte MESSAGE_RESPONSE = 2;
    }
}