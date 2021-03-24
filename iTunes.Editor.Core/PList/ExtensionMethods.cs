// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Extension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> <see cref="int"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Nullable{T}"/> <see cref="int"/>.</returns>
        public static int? GetNullableInt32(this IDictionary<string, object?> dictionary, string key) => (int?)dictionary.GetNullableInt64(key);

        /// <summary>
        /// Gets the <see cref="int"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="int"/>.</returns>
        public static int GetInt32(this IDictionary<string, object?> dictionary, string key) => (int)dictionary.GetInt64(key);

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> <see cref="long"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Nullable{T}"/> <see cref="long"/>.</returns>
        public static long? GetNullableInt64(this IDictionary<string, object?> dictionary, string key) =>
            dictionary.ContainsKey(key) ? (long?)dictionary[key] : default;

        /// <summary>
        /// Gets the <see cref="long"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="long"/>.</returns>
        public static long GetInt64(this IDictionary<string, object?> dictionary, string key) => (long)dictionary[key]!;

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> <see cref="bool"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Nullable{T}"/> <see cref="bool"/>.</returns>
        public static bool? GetNullableBoolean(this IDictionary<string, object?> dictionary, string key) =>
            dictionary.ContainsKey(key) ? (bool?)dictionary[key] : default;

        /// <summary>
        /// Gets the <see cref="bool"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public static bool GetBoolean(this IDictionary<string, object?> dictionary, string key) => (bool)dictionary[key]!;

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Nullable{T}"/> <see cref="DateTime"/>.</returns>
        public static DateTime? GetNullableDateTime(this IDictionary<string, object?> dictionary, string key) =>
            dictionary.ContainsKey(key) ? (DateTime?)dictionary[key] : default;

        /// <summary>
        /// Gets the <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="DateTime"/>.</returns>
        public static DateTime GetDateTime(this IDictionary<string, object?> dictionary, string key) => (DateTime)dictionary[key]!;

        /// <summary>
        /// Gets the <see cref="string"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="string"/> value.</returns>
        public static string? GetNullableString(this IDictionary<string, object?> dictionary, string key) =>
            dictionary.ContainsKey(key) ? (string?)dictionary[key] : default;

        /// <summary>
        /// Gets the <see cref="string"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="string"/> value.</returns>
        public static string GetString(this IDictionary<string, object?> dictionary, string key) => (string)dictionary[key]!;

        /// <summary>
        /// Gets a range of <see cref="byte" /> from the specified start and length.
        /// </summary>
        /// <param name="value">The byte array.</param>
        /// <param name="start">The start index.</param>
        /// <param name="length">The number of bytes to get.</param>
        /// <returns>The range of <see cref="byte"/>.</returns>
        public static byte[] GetRange(this byte[] value, int start, int length)
        {
            var bytes = new byte[length];
            Array.Copy(value, start, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// Reads a byte from the stream at the specified offset and advances the position within the stream by one byte, or returns -1 if at the end of the stream.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="origin">A value of type <see cref="System.IO.SeekOrigin"/>  indicating the reference point used to obtain the new position.</param>
        /// <returns>The unsigned byte.</returns>
        public static byte ReadByte(this System.IO.Stream stream, long offset, System.IO.SeekOrigin origin = System.IO.SeekOrigin.Begin)
        {
            stream.Seek(offset, origin);
            return (byte)stream.ReadByte();
        }

        /// <summary>
        /// Reads a sequence of bytes from the stream at the specified offset and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="length">A number of bytes to read.</param>
        /// <returns>The unsigned byte array.</returns>
        public static byte[] Read(this System.IO.Stream stream, int length) => stream.Read(0L, length, System.IO.SeekOrigin.Current);

        /// <summary>
        /// Reads a sequence of bytes from the stream at the specified offset and advances the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="stream">The stream to read from.</param>
        /// <param name="offset">A byte offset relative to the <paramref name="origin"/> parameter.</param>
        /// <param name="length">A number of bytes to read.</param>
        /// <param name="origin">A value of type <see cref="System.IO.SeekOrigin"/>  indicating the reference point used to obtain the new position.</param>
        /// <returns>The unsigned byte array.</returns>
        public static byte[] Read(this System.IO.Stream stream, long offset, int length, System.IO.SeekOrigin origin = System.IO.SeekOrigin.Begin)
        {
            var bytes = new byte[length];
            stream.Seek(offset, origin);
            _ = stream.Read(bytes, 0, length);
            return bytes;
        }

        /// <summary>
        /// Writes the specfied bytes to the stream.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="buffer">The bytes to write.</param>
        public static void Write(this System.IO.Stream stream, byte[] buffer) => stream.Write(buffer, 0, buffer.Length);

        /// <summary>
        /// Reverses the array.
        /// </summary>
        /// <param name="input">The input array.</param>
        /// <returns>The reversed array.</returns>
        public static byte[] Reverse(this byte[] input)
        {
            System.Array.Reverse(input);
            return input;
        }
    }
}
