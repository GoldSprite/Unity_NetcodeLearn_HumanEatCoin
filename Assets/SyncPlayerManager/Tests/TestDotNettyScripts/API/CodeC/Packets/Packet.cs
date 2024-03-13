using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace GoldSprite.TestDotNetty_API
{
    public abstract class Packet
    {
        public readonly byte Version = 1;
        public string Guid { get; set; }


        public abstract byte Command { get; }


        public override string ToString()
        {
            var jsonStr = JsonConvert.SerializeObject(this);
            string pattern = "(\"[A-Za-z|_]+\")|([}+/\\s+])";
            //string replacement = "\n$1$2";
            string result = Regex.Replace(jsonStr, pattern, m =>
            {
                if (m.Groups[1].Success) return "\n  " + m.Groups[1].Value;
                else return "\n"+m.Groups[2].Value;
            });
            return result;
        }
    }
}