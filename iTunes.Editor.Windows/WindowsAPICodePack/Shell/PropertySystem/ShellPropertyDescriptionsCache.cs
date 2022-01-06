// <copyright file="ShellPropertyDescriptionsCache.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem;

/// <summary>
/// The shell property description cache.
/// </summary>
internal sealed class ShellPropertyDescriptionsCache
{
    private static ShellPropertyDescriptionsCache? cacheInstance;

    private readonly IDictionary<PropertyKey, ShellPropertyDescription> propsDictionary = new Dictionary<PropertyKey, ShellPropertyDescription>();

    /// <summary>
    /// Gets the cache.
    /// </summary>
    public static ShellPropertyDescriptionsCache Cache => cacheInstance
        ??= new ShellPropertyDescriptionsCache();

    /// <summary>
    /// Gets the property description.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The property description.</returns>
    public ShellPropertyDescription GetPropertyDescription(PropertyKey key)
    {
        if (!this.propsDictionary.ContainsKey(key))
        {
            this.propsDictionary.Add(key, new ShellPropertyDescription(key));
        }

        return this.propsDictionary[key];
    }
}
