// <copyright file="PropertyKey.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Resources;

/// <summary>Defines a unique key for a Shell Property.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public readonly struct PropertyKey : IEquatable<PropertyKey>
{
    /// <summary>Initializes a new instance of the <see cref="PropertyKey"/> struct.</summary>
    /// <param name="formatId">A unique GUID for the property.</param>
    /// <param name="propertyId">Property identifier (PID).</param>
    public PropertyKey(Guid formatId, int propertyId)
    {
        this.FormatId = formatId;
        this.PropertyId = propertyId;
    }

    /// <summary>Initializes a new instance of the <see cref="PropertyKey"/> struct.</summary>
    /// <param name="formatId">A string represenstion of a GUID for the property.</param>
    /// <param name="propertyId">Property identifier (PID).</param>
    public PropertyKey(string formatId, int propertyId)
    {
        this.FormatId = new Guid(formatId);
        this.PropertyId = propertyId;
    }

    /// <summary>Gets the unique GUID for the property.</summary>
    public Guid FormatId { get; }

    /// <summary>Gets the property identifier (PID).</summary>
    public int PropertyId { get; }

    /// <summary>Implements the == (equality) operator.</summary>
    /// <param name="propKey1">First property key to compare.</param>
    /// <param name="propKey2">Second property key to compare.</param>
    /// <returns>true if object a equals object b. false otherwise.</returns>
    public static bool operator ==(PropertyKey propKey1, PropertyKey propKey2) => propKey1.Equals(propKey2);

    /// <summary>Implements the != (inequality) operator.</summary>
    /// <param name="propKey1">First property key to compare.</param>
    /// <param name="propKey2">Second property key to compare.</param>
    /// <returns>true if object a does not equal object b. false otherwise.</returns>
    public static bool operator !=(PropertyKey propKey1, PropertyKey propKey2) => !propKey1.Equals(propKey2);

    /// <inheritdoc/>
    public override readonly int GetHashCode() => this.FormatId.GetHashCode() ^ this.PropertyId;

    /// <inheritdoc/>
    public readonly bool Equals(PropertyKey other) => other.Equals((object)this);

    /// <inheritdoc/>
    public override readonly bool Equals(object obj) => obj is PropertyKey other && other.FormatId.Equals(this.FormatId) && (other.PropertyId == this.PropertyId);

    /// <summary>Override ToString() to provide a user friendly string representation.</summary>
    /// <returns>String representing the property key.</returns>
    public override readonly string ToString() => string.Format(
        System.Globalization.CultureInfo.InvariantCulture,
        LocalizedMessages.PropertyKeyFormatString,
        this.FormatId.ToString("B"),
        this.PropertyId);
}
