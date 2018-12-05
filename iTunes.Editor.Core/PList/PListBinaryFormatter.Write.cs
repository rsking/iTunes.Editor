﻿// -----------------------------------------------------------------------
// <copyright file="PListBinaryFormatter.Write.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// The write methods for <see cref="PListBinaryFormatter"/>.
    /// </summary>
    public partial class PListBinaryFormatter
    {
        private static int CountReferences(object value)
        {
            switch (value)
            {
                case IDictionary<string, object> dict:
                    return dict.Values.Sum(CountReferences) + dict.Keys.Count + 1;
                case IList<object> list:
                    return list.Sum(CountReferences) + 1;
                default:
                    return 1;
            }
        }

        private static void Write(Stream stream, IList<int> offsetTable, int objectReferenceSize, object value)
        {
            switch (value)
            {
                case IDictionary<string, object> dict:
                    Write(stream, offsetTable, objectReferenceSize, dict);
                    break;
                case IList<object> list:
                    Write(stream, offsetTable, objectReferenceSize, list);
                    break;
                case byte[] bytes:
                    Write(stream, bytes);
                    break;
                case float floatValue:
                    Write(stream, floatValue);
                    break;
                case double doubleValue:
                    Write(stream, doubleValue);
                    break;
                case short shortValue:
                    Write(stream, shortValue);
                    break;
                case int intValue:
                    Write(stream, intValue);
                    break;
                case long longValue:
                    Write(stream, longValue);
                    break;
                case string stringValue:
                    Write(stream, stringValue, true);
                    break;
                case DateTime dateTime:
                    Write(stream, dateTime);
                    break;
                case bool boolValue:
                    Write(stream, boolValue);
                    break;
            }
        }

        private static void Write(Stream stream, IList<int> offsetTable, int referenceSize, IDictionary<string, object> dictionary)
        {
            if (dictionary.Count < 15)
            {
                stream.WriteByte(Convert.ToByte(0xD0 | Convert.ToByte(dictionary.Count)));
            }
            else
            {
                stream.WriteByte(0xD0 | 0x0F);
                stream.Write(GetBinaryInt(dictionary.Count));
            }

            // get the refs first
            var referencePosition = stream.Position;
            stream.Position += dictionary.Count * referenceSize * 2;

            var references = new List<int>();
            var keys = new string[dictionary.Count];
            dictionary.Keys.CopyTo(keys, 0);
            for (int i = 0; i < dictionary.Count; i++)
            {
                references.Add(offsetTable.Count);
                offsetTable.Add((int)stream.Position);
                Write(stream, offsetTable, referenceSize, keys[i]);
            }

            var values = new object[dictionary.Count];
            dictionary.Values.CopyTo(values, 0);
            for (int i = 0; i < dictionary.Count; i++)
            {
                references.Add(offsetTable.Count);
                offsetTable.Add((int)stream.Position);
                Write(stream, offsetTable, referenceSize, values[i]);
            }

            var endPosition = stream.Position;
            stream.Position = referencePosition;

            foreach (var reference in references)
            {
                var refBuffer = RegulateNullBytes(BitConverter.GetBytes(reference), referenceSize);
                stream.Write(refBuffer.Reverse());
            }

            stream.Position = endPosition;
        }

        private static void Write(Stream stream, IList<int> offsetTable, int referenceSize, IList<object> values)
        {
            // write the header
            if (values.Count < 15)
            {
                stream.WriteByte(Convert.ToByte(0xA0 | Convert.ToByte(values.Count)));
            }
            else
            {
                stream.WriteByte(0xA0 | 0x0F);
                stream.Write(GetBinaryInt(values.Count));
            }

            // get the refs first
            var referencePosition = stream.Position;
            stream.Position += values.Count * referenceSize;

            var references = new List<int>();
            foreach (var value in values)
            {
                references.Add(offsetTable.Count);
                offsetTable.Add((int)stream.Position);
                Write(stream, offsetTable, referenceSize, value);
            }

            var endPosition = stream.Position;
            stream.Position = referencePosition;

            foreach (var reference in references)
            {
                stream.Write(RegulateNullBytes(BitConverter.GetBytes(reference), referenceSize).Reverse());
            }

            stream.Position = endPosition;
        }

        private static void Write(Stream stream, DateTime value)
        {
            var buffer = RegulateNullBytes(BitConverter.GetBytes(PlistDateConverter.ConvertToAppleTimeStamp(value)), 8).Reverse();
            stream.WriteByte(0x33);
            stream.Write(buffer, 0, buffer.Length);
        }

        private static void Write(Stream stream, bool value) => stream.WriteByte(value ? (byte)0x09 : (byte)0x08);

        private static void Write(Stream stream, long value) => stream.Write(GetBinaryInt(value));

        private static void Write(Stream stream, double value)
        {
            var buffer = new List<byte>(RegulateNullBytes(BitConverter.GetBytes(value), 4));
            while (buffer.Count != Math.Pow(2, Math.Log(buffer.Count) / Math.Log(2)))
            {
                buffer.Add(0x00);
            }

            buffer.Add(Convert.ToByte(0x20 | (int)(Math.Log(buffer.Count) / Math.Log(2))));
            buffer.Reverse();
            stream.Write(buffer.ToArray(), 0, buffer.Count);
        }

        private static void Write(Stream stream, byte[] value)
        {
            if (value.Length < 15)
            {
                stream.WriteByte(Convert.ToByte(0x40 | Convert.ToByte(value.Length)));
            }
            else
            {
                stream.WriteByte(0x40 | 0xf);
                stream.Write(GetBinaryInt((long)value.Length));
            }

            stream.Write(value);
        }

        private static void Write(Stream stream, string value, bool head)
        {
            const int MaxAnsiCode = 255;

            if (head)
            {
                if (value.Length < 15)
                {
                    stream.WriteByte(Convert.ToByte(0x50 | Convert.ToByte(value.Length)));
                }
                else
                {
                    stream.WriteByte(0x50 | 0xf);
                    stream.Write(GetBinaryInt(value.Length));
                }
            }

            // see if this contains any unicode characters
            if (value.Any(c => c > MaxAnsiCode))
            {
                stream.Write(System.Text.Encoding.BigEndianUnicode.GetBytes(value));
            }
            else
            {
                stream.Write(System.Text.Encoding.UTF8.GetBytes(value));
            }
        }

        private static byte[] GetBinaryInt(long value) => GetBinaryInt(BitConverter.GetBytes(value));

        private static byte[] GetBinaryInt(byte[] bytes)
        {
            var buffer = new List<byte>(RegulateNullBytes(bytes));
            while (buffer.Count != Math.Pow(2, Math.Log(buffer.Count) / Math.Log(2)))
            {
                buffer.Add(0);
            }

            buffer.Add(Convert.ToByte(0x10 | (int)(Math.Log(buffer.Count) / Math.Log(2))));
            buffer.Reverse();
            return buffer.ToArray();
        }
    }
}
