using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoldSprite.TestDotNetty_API
{
    public class MathTools
    {
        public static Random rand = new Random();


        public static string NewCustomGuid(int maxHexDigits = 16)
        {
            string alphabet = "abcdefABCDEF";
            char[] guidChars = new char[maxHexDigits];
            for (int i = 0, j = rand.Next(3, 6); i < maxHexDigits; i++)
            {
                guidChars[i] = alphabet[rand.Next(alphabet.Length)];
                if (i == j && j < maxHexDigits - 1)
                {
                    guidChars[j] = '-';
                    j += rand.Next(3, 6);
                }
            }
            string guidString = new string(guidChars);
            return guidString;
        }
    }
}
