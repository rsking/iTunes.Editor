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
            dictionary.ContainsKey(key) ? (string)dictionary[key] : default(string);
    }
}
