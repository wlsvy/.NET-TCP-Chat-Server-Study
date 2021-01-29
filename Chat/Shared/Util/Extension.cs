﻿using Shared.Network;
using Shared.Protocol;
using System;
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
        public static int SizeForWrite(this PacketProtocol value) => sizeof(byte);
        public static int SizeForWrite(this string value)
        {
            return sizeof(int) + Encoding.UTF8.GetByteCount(value);
        }
        public static int SizeForWrite(this List<int> list)
        {
            return sizeof(int) + list.Count * sizeof(int); // length + elements
        }

        public static void Write(this BinaryEncoder encoder, in PacketProtocol value)
        {
            byte underlyingValue = (byte)value;
            encoder.Write(in underlyingValue);
        }
        public static int Read(this BinaryDecoder decoder, out PacketProtocol value)
        {
            var consumedBytes = decoder.Read(out byte underlyingValue);
            value = (PacketProtocol)underlyingValue;
            return consumedBytes;
        }

        public static void Write(this BinaryEncoder encoder, in CSPacketProtocol value)
        {
            byte underlyingValue = (byte)value;
            encoder.Write(in underlyingValue);
        }
        public static int Read(this BinaryDecoder decoder, out CSPacketProtocol value)
        {
            var consumedBytes = decoder.Read(out byte underlyingValue);
            value = (CSPacketProtocol)underlyingValue;
            return consumedBytes;
        }
        public static void Write(this BinaryEncoder encoder, in SCPacketProtocol value)
        {
            byte underlyingValue = (byte)value;
            encoder.Write(in underlyingValue);
        }
        public static int Read(this BinaryDecoder decoder, out SCPacketProtocol value)
        {
            var consumedBytes = decoder.Read(out byte underlyingValue);
            value = (SCPacketProtocol)underlyingValue;
            return consumedBytes;
        }
    }
}
