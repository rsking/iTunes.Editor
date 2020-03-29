// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The extension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Sets the properties on the subject.
        /// </summary>
        /// <typeparam name="T">The type of subject.</typeparam>
        /// <param name="subject">The subject.</param>
        /// <param name="properties">The properties.</param>
        /// <returns>The return subject.</returns>
        public static T SetProperties<T>(this T subject, params string[] properties)
        {
            if (subject is null || properties is null)
            {
                return subject;
            }

            foreach (var prop in string.Join(';', properties).Split(';'))
            {
                var split = prop.Split('=');
                var name = split[0];
                var value = split[1];

                var propertyInfo = subject.GetType().GetProperty(name);
                if (propertyInfo is null)
                {
                    continue;
                }

                if (propertyInfo.PropertyType == typeof(bool) && bool.TryParse(value, out var boolValue))
                {
                    propertyInfo.SetValue(subject, boolValue, null);
                }
                else if (propertyInfo.PropertyType == typeof(int) && int.TryParse(value, out var intValue))
                {
                    propertyInfo.SetValue(subject, intValue, null);
                }
                else if (propertyInfo.PropertyType == typeof(string))
                {
                    propertyInfo.SetValue(subject, value, null);
                }
            }

            return subject;
        }

        /// <summary>
        /// Concatenates the members of the <see cref="System.Collections.Generic.IEnumerable{T}"/> collection of type <see cref="string"/>, using the specified separator between each member.
        /// </summary>
        /// <param name="source">A collection that contains the strings to concatenate.</param>
        /// <param name="separator">The string to use as a separator. <paramref name="separator"/> is included in the returned string only if <paramref name="source"/> has more than one element.</param>
        /// <returns>A string that consists of the members of <paramref name="source"/> delimited by the separator string. -or- <see cref="string.Empty"/> if values has zero elements or all the elements of values are null. -or- <see langword="null" /> if <paramref name="source"/> is <see langword="null"/>.</returns>
        public static string? Join(this System.Collections.Generic.IEnumerable<string?> source, string? separator) => source is null ? null : string.Join(separator, source);

        /// <summary>
        /// Concatenates the members of the <see cref="System.Collections.Generic.IEnumerable{T}"/> collection, using the specified separator between each member.
        /// </summary>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <param name="source">A collection that contains the objects to concatenate.</param>
        /// <param name="separator">The string to use as a separator. <paramref name="separator"/> is included in the returned string only if <paramref name="source"/> has more than one element.</param>
        /// <returns>A string that consists of the members of <paramref name="source"/> delimited by the separator string. -or- <see cref="string.Empty"/> if values has zero elements or all the elements of values are null. -or- <see langword="null" /> if <paramref name="source"/> is <see langword="null"/>.</returns>
        public static string? Join<T>(this System.Collections.Generic.IEnumerable<T> source, string? separator) => source is null ? null : string.Join(separator, source);
    }
}