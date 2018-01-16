using System;
using System.Collections.Generic;
using System.Windows;

namespace VideoSearch.Utils
{
    public class StringUtils
    {
        public static byte String2Byte(String value)
        {
            return String.IsNullOrEmpty(value) ? (byte)0 : Decimal.ToByte(Decimal.Parse(value));
        }

        public static short String2Short(String value)
        {
            return String.IsNullOrEmpty(value) ? (short)0 : Decimal.ToInt16(Decimal.Parse(value));
        }

        public static int String2Int(String value)
        {
            return String.IsNullOrEmpty(value) ? 0 : Decimal.ToInt32(Decimal.Parse(value));
        }

        public static Int64 String2Int64(String value)
        {
            return String.IsNullOrEmpty(value) ? 0 : Decimal.ToInt64(Decimal.Parse(value));
        }

        public static UInt64 String2UInt64(String value)
        {
            return String.IsNullOrEmpty(value) ? 0 : Decimal.ToUInt64(Decimal.Parse(value));
        }

        public static double String2Double(String value)
        {
            return String.IsNullOrEmpty(value) ? 0 : Decimal.ToDouble(Decimal.Parse(value));
        }

        public static bool String2Bool(String value)
        {
            int intVal = String.IsNullOrEmpty(value) ? 0 : Decimal.ToInt32(Decimal.Parse(value));

            return (intVal == 0) ? false : true;
        }

        public static String String2String(String value)
        {
            return String.IsNullOrEmpty(value) ? "" : (String)value.Clone();
        }

        public static Rect String2Rect(String value)
        {
            Rect rt = new Rect();

            if (String.IsNullOrEmpty(value))
                return rt;

            string[] separators = { "," };
            string[] words = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            List<double> list = new List<double>();
            foreach (var word in words)
                list.Add(String2Double(word));

            if (list.Count == 4)
                return new Rect(list[0], list[1], list[2] - list[0], list[3] - list[1]);

            return rt;
        }
    }
}
