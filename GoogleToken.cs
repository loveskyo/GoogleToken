using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uTransHubEngine.TransHost
{

    public class GoogleTokenGenerator
    {
        public string generateToken(string text)
        {
            return TL(text);
        }

        private string TL(string a)
        {
            int b = generateB();
            List<int> e = new List<int>();
            for (int g = 0; g < a.Length; g++)
            {
                string chars = a.Substring(g, 1);
                int m = charCodeAt(chars);
                if (128 > m)
                {
                    e.Add(m);
                }
                else
                {
                    if (2048 > m)
                    {
                        e.Add(m >> 6 | 192);
                    }
                    else
                    {
                        if (55296 == (m & 64512) && g + 1 < a.Length && 56320 == (charCodeAt(a.Substring(g + 1, 1)) & 64512))
                        {
                            m = 65536 + ((m & 1023) << 10) + (charCodeAt(a.Substring(++g, 1)) & 1023);
                            e.Add(m >> 18 | 240);
                            e.Add(m >> 12 & 63 | 128);
                        }
                        else
                        {
                            e.Add(m >> 12 | 224);
                            e.Add(m >> 6 & 63 | 128);
                        }
                    }
                    e.Add(m & 63 | 128);
                }
            }

            int z = b;
            foreach (int l in e)
            {
                z += l;
                z = RL(z, "+-a^+6");
            }
            z = RL(z, "+-3^+b+-f");

            long y = z;
            if (0 > z)
            {
                y = (z & 2147483647) + 2147483648;
            }
            y = y % 1000000;
            return (y.ToString() + "." + (y ^ b));
        }


        private int RL(int a, string _b)
        {
            for (var c = 0; c < _b.Length - 2; c += 3)
            {
                int d = 0;
                int.TryParse(_b.Substring(c + 2, 1), out d);
                if (d <= 0)
                {
                    d = charCodeAt(_b.Substring(c + 2, 1));
                }
                d = d >= charCodeAt("a") ? d - 87 : d;
                d = charCodeAt(_b.Substring(c + 1, 1)) == charCodeAt("+") ? MoveByte(a, d) : a << d;
                a = charCodeAt(_b.Substring(c, 1)) == charCodeAt("+") ? (int)(a + d & 4294967295) : a ^ d;
            }
            return a;
        }

        /// <summary>
        /// 特殊的右移位操作，补0右移，如果是负数，需要进行特殊的转换处理（右移位）
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        private int MoveByte(int value, int pos)
        {
            if (value < 0)
            {
                string s = Convert.ToString(value, 2);    // 转换为二进制
                for (int i = 0; i < pos; i++)
                {
                    s = "0" + s.Substring(0, 31);
                }
                return Convert.ToInt32(s, 2);            // 将二进制数字转换为数字
            }
            else
            {
                return value >> pos;
            }
        }

        private int charCodeAt(string str)
        {
            string achar = get_uft32(get_uft8(str));  //mb_substr($str, $index, 1, 'UTF-8');
            char ret = Convert.ToChar(achar);
            int result = Convert.ToInt32(ret);
            return result;
        }

        private int generateB()
        {
            DateTime start = DateTime.Parse("1970-01-01");
            DateTime nowdate = DateTime.UtcNow;
            TimeSpan diff = DateDiff(nowdate, start);
            return diff.Hours + (diff.Days * 24);
        }

        //计算时间的差值
        private static TimeSpan DateDiff(DateTime DateTime1, DateTime DateTime2)
        {

            TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
            TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            return ts;
        }


        public static string get_uft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }

        public static string get_uft32(string unicodeString)
        {
            UTF32Encoding utf32 = new UTF32Encoding();
            Byte[] encodedBytes = utf32.GetBytes(unicodeString);
            String decodedString = utf32.GetString(encodedBytes);
            return decodedString;
        }
    }
}
