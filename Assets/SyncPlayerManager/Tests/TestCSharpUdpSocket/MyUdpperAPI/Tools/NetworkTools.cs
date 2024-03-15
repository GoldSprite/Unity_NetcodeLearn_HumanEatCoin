using System;
using System.Net;
using System.Text.RegularExpressions;

namespace GoldSprite.MyUdpperAPI
{
    public class NetworkTools
    {
        public static string RegexGetIPAddress(string input)
        {
            string pattern = @"\b(?:\d{1,3}\.){3}\d{1,3}\b";
            Regex regex = new Regex(pattern);
            Match match = regex.Match(input);
            if (match.Success)
            {
                return match.Value;
            }
            else
            {
                return "未找到IP地址";
            }
        }


        public static string GetPublicIPHtml()
        {
            string publicIPAddress = string.Empty;
            try
            {
                using (WebClient client = new WebClient())
                {
                    publicIPAddress = client.DownloadString("https://api.ipify.org");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error retrieving public IP address: " + e.Message);
            }

            return publicIPAddress;
        }


        public static string GetNetworkIPV4()
        {
            return RegexGetIPAddress(GetPublicIPHtml());
        }
    }
}
