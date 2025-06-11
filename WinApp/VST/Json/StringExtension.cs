using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringExtension
    {
        public static byte[] ASCII(this string s)
        {
            return Encoding.ASCII.GetBytes(s);
        }
        public static string Words(this string s)
        {
            var words = "";
            foreach (var c in s)
            {
                if (char.IsUpper(c) && words.Length > 0)
                {
                    words += ' ';
                }
                words += c;
            }
            return words;
        }
        public static byte[] UTF8(this string s)
        {
            return Encoding.UTF8.GetBytes(s);
        }
        public static string ASCII(this byte[] bytes)
        {
            return Encoding.ASCII.GetString(bytes);
        }
        public static string UTF8(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        public static string ToBase64(this byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
        public static string ToBase64(this string s) => s.UTF8().ToBase64();
        public static string FromBase64(this string s) => Convert.FromBase64String(s).UTF8();

        public static long ToLong(this string s)
        {
            long a = 0;
            foreach (char c in s)
            {
                if (c >= '0' && '9' >= c)
                {
                    a = (a << 1) + (a << 3) + (c & 15);
                }
            }
            return a;
        }
        public static string ToEmail(this string s)
        {
            var e = s.ToCharArray();
            int j = 0;
            for (int i = 0; i < e.Length; i++)
            {
                char c = e[i];
                switch (c)
                {
                    case ' ':
                    case ',':
                    case ';': continue;
                }

                e[j++] = char.ToLower(c);
            }
            return new string(e, 0, j);
        }
    }

    public static class NumberExtention
    {
        static string[] don_vi = new string[] {
            "không",
            "một",
            "hai",
            "ba",
            "bốn",
            "năm",
            "sáu",
            "bảy",
            "tám",
            "chín",
            "mười",
            "linh",
            "mốt",
            null,
            "tư",
            "lăm",
        };
        static string[] chuc = new string[] { "mươi", "trăm" };
        static string[] hang = new string[] {
            null,
            "nghìn",
            "triệu",
            "tỷ"
        };
        static string one_thousand(long v, bool first = false)
        {
            long d, c, t;
            v = Math.DivRem(v, 10, out d);
            t = Math.DivRem(v, 10, out c);

            List<string> s = new List<string>();

            if (t != 0 || first == false)
            {
                s.Add(don_vi[t] + ' ' + chuc[1]);
                if (c == 0 && d == 0)
                {
                    return s[0];
                }
            }

            switch (c)
            {
                case 0:
                    if (!first || s.Count > 0)
                    {
                        s.Add(don_vi[11]);
                    }
                    break;

                case 1:
                    s.Add(don_vi[10]);
                    break;

                default:
                    s.Add(don_vi[c]);
                    s.Add(chuc[0]);
                    break;
            }

            if (d != 0)
            {
                switch (d)
                {
                    case 1:
                        if (c > 1) d += 11;
                        break;

                    case 4:
                        if (c != 1) d += 10;
                        break;

                    case 5:
                        if (c != 0) d += 10;
                        break;
                }
                s.Add(don_vi[d]);
            }
            return string.Join(" ", s);
        }
        public static string BangChu(this long v)
        {
            var stk = new Stack<long>();
            long r;
            while (v >= 1000)
            {
                v = Math.DivRem(v, 1000, out r);
                stk.Push(r);
            }

            List<string> s = new List<string>();
            s.Add(one_thousand(v, true));

            while (stk.Count != 0)
            {
                s.Add(hang[stk.Count]);
                var one = stk.Pop();
                if (one != 0)
                {
                    s.Add(one_thousand(one));
                }
            }

            return string.Join(" ", s);
        }
    }
}
