using System;
using System.Collections.Generic;

namespace GoldSprite.MyUdpperAPI
{
    public class ILogLevel
    {
        public const int FORCE = -2;
        public const int ERROR = 1;
        public const int WARNING = 2;
        public const int DEBUG = 3;
        public const int INFO = 4;
        public const int MSG = 5;

        private ILogLevel() { }

        public static Dictionary<int, string> msgMap = new Dictionary<int, string>() 
        {
            {ILogLevel.MSG,     "[ MSG ] "},
            {ILogLevel.INFO,    "[ INFO] "},
            {ILogLevel.DEBUG,   "[DEBUG] "},
            {ILogLevel.WARNING, "[ WARN] "},
            {ILogLevel.ERROR,   "[ ERR ] "},
            {ILogLevel.FORCE,   "[FORCE] "},
        };

        public static String getLogMsg(int logLevel)
        {
            if (!msgMap.ContainsKey(logLevel)) return "[UNKNOWN] ";
            return msgMap[logLevel];
        }
    }
}
