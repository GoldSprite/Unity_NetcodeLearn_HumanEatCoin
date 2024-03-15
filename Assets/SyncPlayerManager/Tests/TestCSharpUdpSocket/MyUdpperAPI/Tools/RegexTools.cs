using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GoldSprite.MyUdpperAPI {
    public class RegexTools {
        public static string ToFormatJsonStr(object obj)
        {
            try {
                var jsonStr = JsonConvert.SerializeObject(obj);
                string pattern = "(\"[A-Za-z|_]+\":)|([}+/\\s+])";
                //string replacement = "\n$1$2";
                string result = Regex.Replace(jsonStr, pattern, m => {
                    if (m.Groups[1].Success) return "\n  " + m.Groups[1].Value;
                    else return "\n" + m.Groups[2].Value;
                });
                return result;
            }catch(Exception ex) {
                throw new Exception("序列化异常, 异常的格式: "+ex.Message);
            }
        }
    }
}
