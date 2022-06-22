// -----------------------------------------------------------------------
// <copyright file="ExtensionMethods.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

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

        foreach (var prop in string
            .Join(';', properties)
            .Split(';')
            .Where(p => !string.IsNullOrEmpty(p)))
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
                propertyInfo.SetValue(subject, boolValue, index: null);
            }
            else if (propertyInfo.PropertyType == typeof(int) && int.TryParse(value, System.Globalization.NumberStyles.Integer, System.Globalization.NumberFormatInfo.InvariantInfo, out var intValue))
            {
                propertyInfo.SetValue(subject, intValue, index: null);
            }
            else if (propertyInfo.PropertyType == typeof(string))
            {
                propertyInfo.SetValue(subject, value, index: null);
            }
        }

        return subject;
    }
}