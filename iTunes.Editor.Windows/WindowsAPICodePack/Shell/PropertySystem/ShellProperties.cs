// <copyright file="ShellProperties.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.Resources;
using MS.WindowsAPICodePack.Internal;

/// <summary>
/// Defines a partial class that implements helper methods for retrieving Shell properties using a canonical name, property key, or a
/// strongly-typed property. Also provides access to all the strongly-typed system properties and default properties collections.
/// </summary>
public sealed class ShellProperties
{
    private readonly ShellObject parentShellObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellProperties"/> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    internal ShellProperties(ShellObject parent) => this.parentShellObject = parent;

    /// <summary>Returns a property available in the default property collection using the given property key.</summary>
    /// <param name="key">The property key.</param>
    /// <returns>An IShellProperty.</returns>
    public IShellProperty GetProperty(PropertyKey key) => this.CreateTypedProperty(key);

    /// <summary>Returns a strongly typed property available in the default property collection using the given property key.</summary>
    /// <typeparam name="T">The type of property to retrieve.</typeparam>
    /// <param name="key">The property key.</param>
    /// <returns>A strongly-typed ShellProperty for the given property key.</returns>
    public ShellProperty<T> GetProperty<T>(PropertyKey key) => (ShellProperty<T>)this.CreateTypedProperty(key);

    /// <summary>
    /// Creates a typed property.
    /// </summary>
    /// <param name="propKey">The property key.</param>
    /// <returns>The typed property.</returns>
    internal IShellProperty CreateTypedProperty(PropertyKey propKey) => ShellPropertyFactory.CreateShellProperty(propKey, this.parentShellObject);

    /// <summary>
    /// Creates a typed property.
    /// </summary>
    /// <param name="canonicalName">The canonical name.</param>
    /// <returns>The typed property.</returns>
    internal IShellProperty CreateTypedProperty(string canonicalName)
    {
        // Otherwise, call the native PropertyStore method
        var result = PropertySystemNativeMethods.PSGetPropertyKeyFromName(canonicalName, out var propKey);

        if (!CoreErrorHelper.Succeeded(result))
        {
            throw new ArgumentException(
                LocalizedMessages.ShellInvalidCanonicalName,
                nameof(canonicalName),
                Marshal.GetExceptionForHR(result));
        }

        return this.CreateTypedProperty(propKey);
    }
}
