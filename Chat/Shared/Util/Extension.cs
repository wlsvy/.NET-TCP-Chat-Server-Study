﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Util
{
    public static class Extension
    {
        public static bool IsEmpty<T>(this ICollection<T> collection)
        {
            return collection.Count == 0;
        }
        public static bool IsEmpty<T>(this IReadOnlyCollection<T> collection)
        {
            return collection.Count == 0;
        }
        public static bool IsEmpty<T>(this ArraySegment<T> collection)
        {
            return collection.Count == 0;
        }

        public static int SizeForWrite(this short value) => sizeof(short);
        public static int SizeForWrite(this int value) => sizeof(int);
        public static int SizeForWrite(this long value) => sizeof(long);
        public static int SizeForWrite(this ushort value) => sizeof(uint);
        public static int SizeForWrite(this uint value) => sizeof(ushort);
        public static int SizeForWrite(this ulong value) => sizeof(ulong);
        public static int SizeForWrite(this byte value) => sizeof(byte);
        public static int SizeForWrite(this bool value) => sizeof(bool);
        public static int SizeForWrite(this float value) => sizeof(float);
        public static int SizeForWrite(this double value) => sizeof(double);
        public static int SizeForWrite(this string value)
        {
            return sizeof(int) + Encoding.UTF8.GetByteCount(value);
        }
        public static int SizeForWrite(this List<int> list)
        {
            // length + elements
            return sizeof(int) + list.Count * sizeof(int);
        }
    }
}
