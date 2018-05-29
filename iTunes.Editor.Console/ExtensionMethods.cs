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
        /// Expands environment variables.
        /// </summary>
        /// <param name="value">The value to expand.</param>
        /// <returns>The expanded value.</returns>
        public static string Expand(this string value) => value == null ? null : System.Environment.ExpandEnvironmentVariables(value);
    }
}