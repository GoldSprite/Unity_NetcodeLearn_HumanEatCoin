using System;
using System.Collections.Generic;
using System.Net;

namespace GoldSprite.TestDotNetty_API
{
    public class DateTools
    {
        public static string simpleFormat = "[yy/MM/dd-HH:mm:ss:fff] ";
        public static DateTime dateTime;
        public static string CurrentFormatTime()
        {
            return DateTime.Now.ToString(simpleFormat);
        }

        public static string FormatTimeByMillis(long millis)
        {
            return new DateTime(millis * TimeSpan.TicksPerMillisecond).ToString(simpleFormat); 
        }
    }
}