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
        public static int? GetNullableInt32(this IDictionary<string, object> dictionary, string key) =>
            (int?)dictionary.GetNullableInt64(key);

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> <see cref="long"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Nullable{T}"/> <see cref="long"/>.</returns>
        public static long? GetNullableInt64(this IDictionary<string, object> dictionary, string key) =>
            dictionary.ContainsKey(key) ? (long)dictionary[key] : default(long?);

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> <see cref="bool"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Nullable{T}"/> <see cref="bool"/>.</returns>
        public static bool? GetNullableBoolean(this IDictionary<string, object> dictionary, string key) =>
            dictionary.ContainsKey(key) ? (bool)dictionary[key] : default(bool?);

        /// <summary>
        /// Gets the <see cref="Nullable{T}"/> <see cref="DateTime"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="Nullable{T}"/> <see cref="DateTime"/>.</returns>
        public static DateTime? GetNullableDateTime(this IDictionary<string, object> dictionary, string key) =>
            dictionary.ContainsKey(key) ? (DateTime)dictionary[key] : default(DateTime?);

        /// <summary>
        /// Gets the <see cref="string"/>.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The <see cref="string"/> value.</returns>
        public static string GetString(this IDictionary<string, object> dictionary, string key) =>
            dictionary.ContainsKey(key) ? (string)dictionary[key] : default;

        public static byte[] GetRange(this byte[] value, int start, int length)
        {
            var bytes = new byte[length];
            Array.Copy(value, start, bytes, 0, bytes.Length);
            return bytes;
        }

        public static byte ReadByte(this System.IO.Stream stream, long offset, System.IO.SeekOrigin origin = System.IO.SeekOrigin.Begin)
        {
            stream.Seek(offset, origin);
            return (byte)stream.ReadByte();
        }

        public static byte[] Read(this System.IO.Stream stream, int length) => stream.Read(0L, length, System.IO.SeekOrigin.Current);

        public static byte[] Read(this System.IO.Stream stream, long offset, int length, System.IO.SeekOrigin origin = System.IO.SeekOrigin.Begin)
        {
            var bytes = new byte[length];
            stream.Seek(offset, origin);
            stream.Read(bytes, 0, length);
            return bytes;
        }

        public static void Write(this System.IO.Stream stream, byte[] buffer) => stream.Write(buffer, 0, buffer.Length);

        public static byte[] Reverse(this byte[] input)
        {
            System.Array.Reverse(input);
            return input;
        }
    }
}
