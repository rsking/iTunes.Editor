// -----------------------------------------------------------------------
// <copyright file="PListBinaryFormatter.Read.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// The read methods for <see cref="PListBinaryFormatter"/>.
    /// </summary>
    public partial class PListBinaryFormatter
    {
        private static object Read(Stream stream, IList<int> offsetTable, int index, int objectReferenceSize)
        {
            var header = stream.ReadByte(offsetTable[index]);
            switch (header & 0xF0)
            {
                case 0: // boolean
                    // If the byte is
                    // 0 return null
                    // 9 return true
                    // 8 return false
                    return (header == 0) ? (object)null : header == 9;
                case 0x10: // int64
                    return ReadInt64(stream, offsetTable[index]);
                case 0x20: // double
                    return ReadDouble(stream, offsetTable[index]);
                case 0x30: // date time
                    return ReadDateTime(stream, offsetTable[index]);
                case 0x40: // bytes
                    return ReadBytes(stream, offsetTable[index]);
                case 0x50: // ASCII
                    return ReadAsciiString(stream, offsetTable[index]);
                case 0x60: // Unicode
                    return ReadUnicodeString(stream, offsetTable[index]);
                case 0xa0: // Array
                    return ReadArray(stream, offsetTable, index, objectReferenceSize);
                case 0xd0: // Dictionary
                    return ReadDictionary(stream, offsetTable, index, objectReferenceSize);
            }

            throw new Exception("This type is not supported");
        }

        private static object ReadDictionary(Stream stream, IList<int> offsetTable, int index, int referenceSize)
        {
            var buffer = new Dictionary<string, object>();
            var referenceCount = GetCount(stream, offsetTable[index], out var referenceStartPosition);

            if (referenceCount < 15)
            {
                referenceStartPosition = offsetTable[index] + 1;
            }
            else
            {
                // The following integer has a header aswell so we increase the referenceStartPosition by two to account for that.
                referenceStartPosition = offsetTable[index] + 2 + RegulateNullBytes(BitConverter.GetBytes(referenceCount), 1).Length;
            }

            var references = new int[referenceCount * 2];
            var current = referenceStartPosition;
            for (int i = 0; i < references.Length; i++)
            {
                byte[] referenceBuffer = stream.Read(current, referenceSize).Reverse();
                references[i] = BitConverter.ToInt32(RegulateNullBytes(referenceBuffer, 4), 0);
                current += referenceSize;
            }

            for (int i = 0; i < referenceCount; i++)
            {
                buffer.Add(
                    (string)Read(stream, offsetTable, references[i], referenceSize),
                    Read(stream, offsetTable, references[i + referenceCount], referenceSize));
            }

            return buffer;
        }

        private static IList<object> ReadArray(Stream stream, IList<int> offsetTable, int index, int referenceSize)
        {
            var buffer = new List<object>();
            var referenceCount = GetCount(stream, offsetTable[index], out var referenceStartPosition);

            if (referenceCount < 15)
            {
                referenceStartPosition = offsetTable[index] + 1;
            }
            else
            {
                // The following integer has a header aswell so we increase the referenceStartPosition by two to account for that.
                referenceStartPosition = offsetTable[index] + 2 + RegulateNullBytes(BitConverter.GetBytes(referenceCount), 1).Length;
            }

            var references = new List<int>();
            for (int i = referenceStartPosition; i < referenceStartPosition + (referenceCount * referenceSize); i += referenceSize)
            {
                var referenceBuffer = stream.Read(i, referenceSize).Reverse();
                references.Add(BitConverter.ToInt32(RegulateNullBytes(referenceBuffer, 4), 0));
            }

            for (int i = 0; i < referenceCount; i++)
            {
                buffer.Add(Read(stream, offsetTable, references[i], referenceSize));
            }

            return buffer;
        }

        private static long ReadInt64(Stream stream, int headerPosition) => ReadInt64(stream, headerPosition, out var output);

        private static long ReadInt64(Stream stream, int headerPosition, out int newHeaderPosition)
        {
            var header = stream.ReadByte(headerPosition);
            var byteCount = (int)Math.Pow(2, header & 0xf);

            var buffer = stream.Read(headerPosition + 1, byteCount).Reverse();

            // Add one to account for the header byte
            newHeaderPosition = headerPosition + byteCount + 1;
            return BitConverter.ToInt64(RegulateNullBytes(buffer, sizeof(long)), 0);
        }

        private static DateTime ReadDateTime(Stream stream, int headerPosition)
        {
            var buffer = stream.Read(headerPosition + 1, 8).Reverse();
            var appleTime = BitConverter.ToDouble(buffer, 0);
            return PlistDateConverter.ConvertFromAppleTimeStamp(appleTime);
        }

        private static double ReadDouble(Stream stream, int headerPosition)
        {
            var header = stream.ReadByte(headerPosition);
            var byteCount = (int)Math.Pow(2, header & 0xf);
            var buffer = stream.Read(headerPosition + 1, byteCount).Reverse();
            return BitConverter.ToDouble(RegulateNullBytes(buffer, 8), 0);
        }

        private static string ReadAsciiString(Stream stream, int headerPosition)
        {
            var charCount = GetCount(stream, headerPosition, out var charStartPosition);
            var buffer = stream.Read(charStartPosition, charCount);
            return buffer.Length > 0 ? Encoding.UTF8.GetString(buffer) : string.Empty;
        }

        private static string ReadUnicodeString(Stream stream, int headerPosition)
        {
            var charCount = GetCount(stream, headerPosition, out var charStartPosition) * 2;
            var buffer = stream.Read(charStartPosition, charCount);
            return Encoding.BigEndianUnicode.GetString(buffer);
            //var buffer = new byte[charCount];
            //byte one, two;

            //for (int i = 0; i < charCount; i += 2)
            //{
            //    one = stream.ReadByte(charStartPosition + i);
            //    two = stream.ReadByte(charStartPosition + i + 1);

            //    if (BitConverter.IsLittleEndian)
            //    {
            //        buffer[i] = two;
            //        buffer[i + 1] = one;
            //    }
            //    else
            //    {
            //        buffer[i] = one;
            //        buffer[i + 1] = two;
            //    }
            //}

            //return Encoding.Unicode.GetString(buffer);
        }

        private static byte[] ReadBytes(Stream stream, int headerPosition)
        {
            var byteCount = GetCount(stream, headerPosition, out var byteStartPosition);
            return stream.Read(byteStartPosition, byteCount);
        }

        private static int GetCount(Stream stream, int bytePosition, out int newBytePosition)
        {
            var headerByte = stream.ReadByte(bytePosition);
            var headerByteTrail = Convert.ToByte(headerByte & 0xf);
            int count;
            if (headerByteTrail < 15)
            {
                count = headerByteTrail;
                newBytePosition = bytePosition + 1;
            }
            else
            {
                count = (int)ReadInt64(stream, bytePosition + 1, out newBytePosition);
            }

            return count;
        }
    }
}
