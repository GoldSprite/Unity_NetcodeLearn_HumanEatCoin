using System;

namespace GoldSprite {
    public class DateTools
    {
        public static string currentDateTime()
        {
            return DateTime.Now.ToString("[HH:mm:ss] ");
        }
    }
}