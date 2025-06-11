using System;
using System.Collections.Generic;

namespace System.Text
{
    public static class VietExtension
    {
        public static string VnText(this string source)
        {
            var res = new StringBuilder();
            VietUnicode code;
            byte viet = 0;
            foreach (var c in source)
            {
                if (c >= 'A' && VietUnicode.Map.TryGetValue(c, out code))
                {
                    res.Append(code.Original)
                        .Append((char)((code.Viet >> 4) | '0'));

                    if (viet == 0) viet = code.Viet;
                }
                else if (c == ' ' && viet != 0)
                {
                    res.Append((char)((viet & 15) | '0')).Append(' ');
                    viet = 0;
                }
                else
                {
                    res.Append(c);
                }
            }
            if (viet != 0) res.Append((char)((viet & 15) | '0'));

            return res.ToString();
        }
        public static string VnCharacter(this string source)
        {
            string res = string.Empty;
            foreach (var c in source)
            {
                VietUnicode code;
                if (c >= 'A' && VietUnicode.Map.TryGetValue(c, out code))
                {
                    res += code.Original;
                }
                else
                {
                    res += c;
                }
            }
            return res;
        }
        public static string VnPersonName(this string source)
        {
            var i = source.LastIndexOf(' ');
            if (i < 0) return source;

            return source.Substring(i + 1) + ' ' + source.Substring(0, i);
        }
        public static DateTime ToDate(this string source)
        {
            var v = new int[3];
            int i = 0;
            bool slash = true;

            int a = 0;


            foreach (var c in source)
            {
                if (char.IsDigit(c))
                {
                    a = (a << 1) + (a << 3) + (c & 15);
                    continue;
                }

                if (c == '-')
                {
                    slash = false;
                }
                v[i++] = a;
                a = 0;

                if (i == v.Length) break;
            }
            if (i < v.Length) { v[i] = a; }
            if (slash) { a = v[0]; v[0] = v[2]; v[2] = a; };

            return new DateTime(v[0], v[1], v[2]);
        }
    }

    public class VietUnicode
    {
        public char Original { get; set; } // ký tự latinh
        public byte Viet { get; set; } // 4 bit cao - mũ, 4 bit thấp - dấu

        static VietUnicodeCollection _map;
        static public VietUnicodeCollection Map
        {
            get
            {
                if (_map == null)
                {
                    _map = new VietUnicodeCollection {
                        { 'a', new VietUnicode { Original = 'a', Viet = 0x00 } },
                        { 'á', new VietUnicode { Original = 'a', Viet = 0x04 } },
                        { 'à', new VietUnicode { Original = 'a', Viet = 0x01 } },
                        { 'ã', new VietUnicode { Original = 'a', Viet = 0x03 } },
                        { 'ạ', new VietUnicode { Original = 'a', Viet = 0x05 } },
                        { 'ả', new VietUnicode { Original = 'a', Viet = 0x02 } },
                        { 'ă', new VietUnicode { Original = 'a', Viet = 0x10 } },
                        { 'ắ', new VietUnicode { Original = 'a', Viet = 0x14 } },
                        { 'ằ', new VietUnicode { Original = 'a', Viet = 0x11 } },
                        { 'ẵ', new VietUnicode { Original = 'a', Viet = 0x13 } },
                        { 'ặ', new VietUnicode { Original = 'a', Viet = 0x15 } },
                        { 'ẳ', new VietUnicode { Original = 'a', Viet = 0x12 } },
                        { 'â', new VietUnicode { Original = 'a', Viet = 0x20 } },
                        { 'ấ', new VietUnicode { Original = 'a', Viet = 0x24 } },
                        { 'ầ', new VietUnicode { Original = 'a', Viet = 0x21 } },
                        { 'ẫ', new VietUnicode { Original = 'a', Viet = 0x23 } },
                        { 'ậ', new VietUnicode { Original = 'a', Viet = 0x25 } },
                        { 'ẩ', new VietUnicode { Original = 'a', Viet = 0x22 } },
                        { 'd', new VietUnicode { Original = 'd', Viet = 0x00 } },
                        { 'đ', new VietUnicode { Original = 'd', Viet = 0x20 } },
                        { 'e', new VietUnicode { Original = 'e', Viet = 0x00 } },
                        { 'é', new VietUnicode { Original = 'e', Viet = 0x04 } },
                        { 'è', new VietUnicode { Original = 'e', Viet = 0x01 } },
                        { 'ẹ', new VietUnicode { Original = 'e', Viet = 0x05 } },
                        { 'ẽ', new VietUnicode { Original = 'e', Viet = 0x03 } },
                        { 'ẻ', new VietUnicode { Original = 'e', Viet = 0x02 } },
                        { 'ê', new VietUnicode { Original = 'e', Viet = 0x20 } },
                        { 'ế', new VietUnicode { Original = 'e', Viet = 0x24 } },
                        { 'ề', new VietUnicode { Original = 'e', Viet = 0x21 } },
                        { 'ệ', new VietUnicode { Original = 'e', Viet = 0x25 } },
                        { 'ể', new VietUnicode { Original = 'e', Viet = 0x22 } },
                        { 'ễ', new VietUnicode { Original = 'e', Viet = 0x23 } },
                        { 'i', new VietUnicode { Original = 'i', Viet = 0x00 } },
                        { 'í', new VietUnicode { Original = 'i', Viet = 0x04 } },
                        { 'ì', new VietUnicode { Original = 'i', Viet = 0x01 } },
                        { 'ị', new VietUnicode { Original = 'i', Viet = 0x05 } },
                        { 'ĩ', new VietUnicode { Original = 'i', Viet = 0x03 } },
                        { 'ỉ', new VietUnicode { Original = 'i', Viet = 0x02 } },
                        { 'o', new VietUnicode { Original = 'o', Viet = 0x00 } },
                        { 'ó', new VietUnicode { Original = 'o', Viet = 0x04 } },
                        { 'ò', new VietUnicode { Original = 'o', Viet = 0x01 } },
                        { 'ọ', new VietUnicode { Original = 'o', Viet = 0x05 } },
                        { 'ỏ', new VietUnicode { Original = 'o', Viet = 0x02 } },
                        { 'õ', new VietUnicode { Original = 'o', Viet = 0x03 } },
                        { 'ô', new VietUnicode { Original = 'o', Viet = 0x20 } },
                        { 'ố', new VietUnicode { Original = 'o', Viet = 0x24 } },
                        { 'ồ', new VietUnicode { Original = 'o', Viet = 0x21 } },
                        { 'ộ', new VietUnicode { Original = 'o', Viet = 0x25 } },
                        { 'ổ', new VietUnicode { Original = 'o', Viet = 0x22 } },
                        { 'ỗ', new VietUnicode { Original = 'o', Viet = 0x23 } },
                        { 'ơ', new VietUnicode { Original = 'o', Viet = 0x10 } },
                        { 'ớ', new VietUnicode { Original = 'o', Viet = 0x14 } },
                        { 'ờ', new VietUnicode { Original = 'o', Viet = 0x11 } },
                        { 'ợ', new VietUnicode { Original = 'o', Viet = 0x15 } },
                        { 'ở', new VietUnicode { Original = 'o', Viet = 0x12 } },
                        { 'ỡ', new VietUnicode { Original = 'o', Viet = 0x13 } },
                        { 'u', new VietUnicode { Original = 'u', Viet = 0x00 } },
                        { 'ú', new VietUnicode { Original = 'u', Viet = 0x04 } },
                        { 'ù', new VietUnicode { Original = 'u', Viet = 0x01 } },
                        { 'ụ', new VietUnicode { Original = 'u', Viet = 0x05 } },
                        { 'ủ', new VietUnicode { Original = 'u', Viet = 0x02 } },
                        { 'ũ', new VietUnicode { Original = 'u', Viet = 0x03 } },
                        { 'ư', new VietUnicode { Original = 'u', Viet = 0x10 } },
                        { 'ứ', new VietUnicode { Original = 'u', Viet = 0x14 } },
                        { 'ừ', new VietUnicode { Original = 'u', Viet = 0x11 } },
                        { 'ự', new VietUnicode { Original = 'u', Viet = 0x15 } },
                        { 'ử', new VietUnicode { Original = 'u', Viet = 0x12 } },
                        { 'ữ', new VietUnicode { Original = 'u', Viet = 0x13 } },
                        { 'y', new VietUnicode { Original = 'y', Viet = 0x00 } },
                        { 'ý', new VietUnicode { Original = 'y', Viet = 0x04 } },
                        { 'ỳ', new VietUnicode { Original = 'y', Viet = 0x01 } },
                        { 'ỵ', new VietUnicode { Original = 'y', Viet = 0x05 } },
                        { 'ỷ', new VietUnicode { Original = 'y', Viet = 0x02 } },
                        { 'ỹ', new VietUnicode { Original = 'y', Viet = 0x03 } },

                        { 'A', new VietUnicode { Original = 'A', Viet = 0x00 } },
                        { 'Á', new VietUnicode { Original = 'A', Viet = 0x04 } },
                        { 'À', new VietUnicode { Original = 'A', Viet = 0x01 } },
                        { 'Ã', new VietUnicode { Original = 'A', Viet = 0x03 } },
                        { 'Ạ', new VietUnicode { Original = 'A', Viet = 0x05 } },
                        { 'Ả', new VietUnicode { Original = 'A', Viet = 0x02 } },
                        { 'Ă', new VietUnicode { Original = 'A', Viet = 0x10 } },
                        { 'Ắ', new VietUnicode { Original = 'A', Viet = 0x14 } },
                        { 'Ằ', new VietUnicode { Original = 'A', Viet = 0x11 } },
                        { 'Ẵ', new VietUnicode { Original = 'A', Viet = 0x13 } },
                        { 'Ặ', new VietUnicode { Original = 'A', Viet = 0x15 } },
                        { 'Ẳ', new VietUnicode { Original = 'A', Viet = 0x12 } },
                        { 'Â', new VietUnicode { Original = 'A', Viet = 0x20 } },
                        { 'Ấ', new VietUnicode { Original = 'A', Viet = 0x24 } },
                        { 'Ầ', new VietUnicode { Original = 'A', Viet = 0x21 } },
                        { 'Ẫ', new VietUnicode { Original = 'A', Viet = 0x23 } },
                        { 'Ậ', new VietUnicode { Original = 'A', Viet = 0x25 } },
                        { 'Ẩ', new VietUnicode { Original = 'A', Viet = 0x22 } },
                        { 'D', new VietUnicode { Original = 'D', Viet = 0x00 } },
                        { 'Đ', new VietUnicode { Original = 'D', Viet = 0x20 } },
                        { 'E', new VietUnicode { Original = 'E', Viet = 0x00 } },
                        { 'É', new VietUnicode { Original = 'E', Viet = 0x04 } },
                        { 'È', new VietUnicode { Original = 'E', Viet = 0x01 } },
                        { 'Ẹ', new VietUnicode { Original = 'E', Viet = 0x05 } },
                        { 'Ẽ', new VietUnicode { Original = 'E', Viet = 0x03 } },
                        { 'Ẻ', new VietUnicode { Original = 'E', Viet = 0x02 } },
                        { 'Ê', new VietUnicode { Original = 'E', Viet = 0x20 } },
                        { 'Ế', new VietUnicode { Original = 'E', Viet = 0x24 } },
                        { 'Ề', new VietUnicode { Original = 'E', Viet = 0x21 } },
                        { 'Ệ', new VietUnicode { Original = 'E', Viet = 0x25 } },
                        { 'Ể', new VietUnicode { Original = 'E', Viet = 0x22 } },
                        { 'Ễ', new VietUnicode { Original = 'E', Viet = 0x23 } },
                        { 'I', new VietUnicode { Original = 'I', Viet = 0x00 } },
                        { 'Í', new VietUnicode { Original = 'I', Viet = 0x04 } },
                        { 'Ì', new VietUnicode { Original = 'I', Viet = 0x01 } },
                        { 'Ị', new VietUnicode { Original = 'I', Viet = 0x05 } },
                        { 'Ĩ', new VietUnicode { Original = 'I', Viet = 0x03 } },
                        { 'Ỉ', new VietUnicode { Original = 'I', Viet = 0x02 } },
                        { 'O', new VietUnicode { Original = 'O', Viet = 0x00 } },
                        { 'Ó', new VietUnicode { Original = 'O', Viet = 0x04 } },
                        { 'Ò', new VietUnicode { Original = 'O', Viet = 0x01 } },
                        { 'Ọ', new VietUnicode { Original = 'O', Viet = 0x05 } },
                        { 'Ỏ', new VietUnicode { Original = 'O', Viet = 0x02 } },
                        { 'Õ', new VietUnicode { Original = 'O', Viet = 0x03 } },
                        { 'Ô', new VietUnicode { Original = 'O', Viet = 0x20 } },
                        { 'Ố', new VietUnicode { Original = 'O', Viet = 0x24 } },
                        { 'Ồ', new VietUnicode { Original = 'O', Viet = 0x21 } },
                        { 'Ộ', new VietUnicode { Original = 'O', Viet = 0x25 } },
                        { 'Ổ', new VietUnicode { Original = 'O', Viet = 0x22 } },
                        { 'Ỗ', new VietUnicode { Original = 'O', Viet = 0x23 } },
                        { 'Ơ', new VietUnicode { Original = 'O', Viet = 0x10 } },
                        { 'Ớ', new VietUnicode { Original = 'O', Viet = 0x14 } },
                        { 'Ờ', new VietUnicode { Original = 'O', Viet = 0x11 } },
                        { 'Ợ', new VietUnicode { Original = 'O', Viet = 0x15 } },
                        { 'Ở', new VietUnicode { Original = 'O', Viet = 0x12 } },
                        { 'Ỡ', new VietUnicode { Original = 'O', Viet = 0x13 } },
                        { 'U', new VietUnicode { Original = 'U', Viet = 0x00 } },
                        { 'Ú', new VietUnicode { Original = 'U', Viet = 0x04 } },
                        { 'Ù', new VietUnicode { Original = 'U', Viet = 0x01 } },
                        { 'Ụ', new VietUnicode { Original = 'U', Viet = 0x05 } },
                        { 'Ủ', new VietUnicode { Original = 'U', Viet = 0x02 } },
                        { 'Ũ', new VietUnicode { Original = 'U', Viet = 0x03 } },
                        { 'Ư', new VietUnicode { Original = 'U', Viet = 0x10 } },
                        { 'Ứ', new VietUnicode { Original = 'U', Viet = 0x14 } },
                        { 'Ừ', new VietUnicode { Original = 'U', Viet = 0x11 } },
                        { 'Ự', new VietUnicode { Original = 'U', Viet = 0x15 } },
                        { 'Ử', new VietUnicode { Original = 'U', Viet = 0x12 } },
                        { 'Ữ', new VietUnicode { Original = 'U', Viet = 0x13 } },
                        { 'Y', new VietUnicode { Original = 'Y', Viet = 0x00 } },
                        { 'Ý', new VietUnicode { Original = 'Y', Viet = 0x04 } },
                        { 'Ỳ', new VietUnicode { Original = 'Y', Viet = 0x01 } },
                        { 'Ỵ', new VietUnicode { Original = 'Y', Viet = 0x05 } },
                        { 'Ỷ', new VietUnicode { Original = 'Y', Viet = 0x02 } },
                        { 'Ỹ', new VietUnicode { Original = 'Y', Viet = 0x03 } },
                    };
                }
                return _map;
            }
        }
    }
    public class VietUnicodeCollection : Dictionary<char, VietUnicode>
    {

    }
}
