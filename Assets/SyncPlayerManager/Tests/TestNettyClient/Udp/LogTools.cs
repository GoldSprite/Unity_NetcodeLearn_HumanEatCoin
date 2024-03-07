using UnityEngine;
using System.Net;
using GoldSprite.TestDotNetty;

namespace GoldSprite
{
    public class LogTools
    {
        public static void NLog(IPEndPoint sender, string msg)
        {
            var isServer = sender.Address.ToString() == TestNettyUdpClient.addr1_w;
            Debug.Log(
                    //DateTools.currentDateTime() +
                            "[" + (isServer ? "SERVER" : "CLIENT") + "] " +
                            "[" + sender.Address.ToString() + ":" + sender.Port + "] " +
                            msg
            );
        }
    }
}
