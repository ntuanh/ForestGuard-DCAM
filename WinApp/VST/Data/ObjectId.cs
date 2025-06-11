using System;
using System.Collections.Generic;
using System.Text;

namespace BsonData
{
    public class ObjectId
    {
        static int _unique;
        static int _inc;
        static DateTime? _startTime;
        static char ToHex(int value)
        {
            return (char)(value > 9 ? value + 87 : (value | 0x30));
        }

        public byte[] Bytes { get; } = new byte[12];
        void PushValue(int v, int index)
        {
            for (int i = 0; i < 4; i++)
            {
                Bytes[index + i] = (byte)v;
                v >>= 4;
            }
        }

        public ObjectId()
        {
            if (_startTime == null)
            {
                _unique = System.Diagnostics.Process.GetCurrentProcess().Id;
                _inc = new Random(0).Next();
                _startTime = new DateTime(1970, 1, 1);
            }

            PushValue((int)(DateTime.Now - _startTime.Value).TotalSeconds, 0);
            PushValue(_unique, 4);
            PushValue(_inc++, 8);
        }
        public static implicit operator string(ObjectId obj)
        {
            return obj.ToString();
        }
        public override string ToString()
        {
            var s = new char[Bytes.Length << 1];
            for (int i = 0, k = 0; i < Bytes.Length; i++)
            {
                s[k++] = ToHex(Bytes[i] >> 4);
                s[k++] = ToHex(Bytes[i] & 15);
            }
            return new string(s);
        }
        
        public int CompareTo(ObjectId id)
        {
            for (int i = 0; i < Bytes.Length; i++)
            {
                int c = Bytes[i];
                c -= id.Bytes[i];

                if (c != 0) { return c; }
            }
            return 0;
        }
        public override bool Equals(object obj)
        {
            return CompareTo((ObjectId)obj) == 0;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
