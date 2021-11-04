// <copyright file="ShellProperty{T}.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System.Diagnostics;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

/// <summary>
/// Defines a strongly-typed property object. All writable property objects must be of this type to be able to call the value setter.
/// </summary>
/// <typeparam name="T">The type of this property's value. Because a property value can be empty, only nullable types are allowed.</typeparam>
public class ShellProperty<T> : IShellProperty
{
    private PropertyKey propertyKey;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellProperty{T}"/> class.
    /// </summary>
    /// <param name="propertyKey">The property key.</param>
    /// <param name="description">The description.</param>
    /// <param name="parent">The parent.</param>
    internal ShellProperty(
        PropertyKey propertyKey,
        ShellPropertyDescription description,
        ShellObject parent)
    {
        this.propertyKey = propertyKey;
        this.Description = description;
        this.ParentShellObject = parent;
        this.AllowSetTruncatedValue = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellProperty{T}"/> class.
    /// </summary>
    /// <param name="propertyKey">The property key.</param>
    /// <param name="description">The description.</param>
    /// <param name="propertyStore">The property store.</param>
    internal ShellProperty(
        PropertyKey propertyKey,
        ShellPropertyDescription description,
        IPropertyStore propertyStore)
    {
        this.propertyKey = propertyKey;
        this.Description = description;
        this.NativePropertyStore = propertyStore;
        this.AllowSetTruncatedValue = false;
    }

    /// <summary>Gets or sets a value indicating whether a value can be truncated. The default for this property is false.</summary>
    /// <remarks>
    /// An <see cref="ArgumentOutOfRangeException"/> will be thrown if this property is not set to true, and a property value was set but
    /// later truncated.
    /// </remarks>
    public bool AllowSetTruncatedValue { get; set; }

    /// <summary>Gets the property description object.</summary>
    public ShellPropertyDescription Description { get; }

    /// <summary>Gets the property key identifying this property.</summary>
    public PropertyKey PropertyKey => this.propertyKey;

    /// <summary>
    /// Gets the strongly-typed value of this property. The value of the property is cleared if the value is set to null.
    /// </summary>
    /// <exception cref="COMException">
    /// If the property value cannot be retrieved or updated in the Property System.
    /// </exception>
    /// <exception cref="NotSupportedException">If the type of this property is not supported; e.g. writing a binary object.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if <see cref="AllowSetTruncatedValue"/> is false, and either a string value was truncated or a numeric value was rounded.
    /// </exception>
    public T? Value
    {
        get
        {
            // Make sure we load the correct type
            Debug.Assert(this.ValueType == ShellPropertyFactory.VarEnumToSystemType(this.Description.VarEnumType), "Incorrect value type");

            using var propVar = new PropVariant();
            if (this.ParentShellObject is not null)
            {
                // If there is a valid property store for this shell object, then use it.
                if (this.ParentShellObject.NativePropertyStore is not null)
                {
                    this.ParentShellObject.NativePropertyStore.GetValue(ref this.propertyKey, propVar);
                }
                else
                {
                    // Use IShellItem2.GetProperty instead of creating a new property store The file might be locked. This is probably
                    // quicker, and sufficient for what we need
                    this.ParentShellObject.GetNativeShellItem2().GetProperty(ref this.propertyKey, propVar);
                }
            }
            else if (this.NativePropertyStore is not null)
            {
                this.NativePropertyStore.GetValue(ref this.propertyKey, propVar);
            }

            // Get the value
            return propVar.Value is not null ? (T)propVar.Value : default;
        }
    }

    /// <summary>
    /// Gets the value for this property using the generic Object type. To obtain a specific type for this value, use the more type
    /// strong Property{T}; class. Also, you can only set a value for this type using Property{T}.
    /// </summary>
    public object? ValueAsObject
    {
        get
        {
            using var propVar = new PropVariant();
            if (this.ParentShellObject is not null)
            {
                var store = ShellPropertyCollection.CreateDefaultPropertyStore(this.ParentShellObject);

                store.GetValue(ref this.propertyKey, propVar);

                Marshal.ReleaseComObject(store);
            }
            else if (this.NativePropertyStore is not null)
            {
                this.NativePropertyStore.GetValue(ref this.propertyKey, propVar);
            }

            return propVar?.Value;
        }
    }

    /// <summary>Gets the associated runtime type.</summary>
    public Type ValueType
    {
        get
        {
            // The type for this object need to match that of the description
            Debug.Assert(this.Description.ValueType == typeof(T), "Incorrect value type");

            return this.Description.ValueType;
        }
    }

    /// <summary>Gets the case-sensitive name of a property as it is known to the system, regardless of its localized name.</summary>
    public string CanonicalName => this.Description.CanonicalName;

    private IPropertyStore? NativePropertyStore { get; }

    private ShellObject? ParentShellObject { get; }

    /// <summary>Returns a formatted, Unicode string representation of a property value.</summary>
    /// <param name="format">One or more of the PropertyDescriptionFormat flags that indicate the desired format.</param>
    /// <returns>The formatted value as a string, or null if this property cannot be formatted for display.</returns>
    public string? FormatForDisplay(PropertyDescriptionFormatOptions format)
    {
        if (this.Description is null || this.Description.NativePropertyDescription is null)
        {
            // We cannot do anything without a property description
            return null;
        }

        if (this.ParentShellObject is null)
        {
            return null;
        }

        var store = ShellPropertyCollection.CreateDefaultPropertyStore(this.ParentShellObject);

        using var propVar = new PropVariant();
        store.GetValue(ref this.propertyKey, propVar);

        // Release the Propertystore
        Marshal.ReleaseComObject(store);

        var hr = this.Description.NativePropertyDescription.FormatForDisplay(propVar, ref format, out var formattedString);

        // Sometimes, the value cannot be displayed properly, such as for blobs or if we get argument exception
        if (!CoreErrorHelper.Succeeded(hr))
        {
            throw new ShellException(hr);
        }

        return formattedString;
    }
}
