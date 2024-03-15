using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace GoldSprite.MyUdpperAPI
{
    public abstract class Packet
    {
        public readonly byte Version = 1;
        public string Guid { get; set; }
        public abstract byte Command { get; }
        //public AddressPoint Sender { get; set; }


        public override string ToString()
        {
            return RegexTools.ToFormatJsonStr(this);
        }
    }

    //public class AddressPoint
    //{
    //    public string Ip;
    //    public int Port;
    //}
}