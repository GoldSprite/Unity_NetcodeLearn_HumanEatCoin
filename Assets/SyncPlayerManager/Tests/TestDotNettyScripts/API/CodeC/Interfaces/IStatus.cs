namespace GoldSprite.TestDotNetty_API
{
    public class IStatus
    {
        private IStatus() { }


        public static byte SEND_SUCCESS => 101;
        public static byte SEND_DEFEAT => 102;
        public static byte RETURN_SUCCESS => 201;
        public static byte RETURN_DEFEAT => 202;


        public static bool IsSend(byte code)
        {
            return code / 100 == 1;
        }


        public static bool IsSuccess(byte code)
        {
            return code % 100 == 1;
        }
    }
}