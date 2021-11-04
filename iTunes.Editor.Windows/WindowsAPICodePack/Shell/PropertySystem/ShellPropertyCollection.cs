// <copyright file="ShellPropertyCollection.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

/// <summary>Creates a readonly collection of IProperty objects.</summary>
public class ShellPropertyCollection : ReadOnlyCollection<IShellProperty>, IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellPropertyCollection"/> class.
    /// </summary>
    /// <param name="parent">Parent ShellObject.</param>
    public ShellPropertyCollection(ShellObject parent)
        : base(new List<IShellProperty>())
    {
        this.ParentShellObject = parent;
        IPropertyStore? nativePropertyStore = null;
        try
        {
            nativePropertyStore = CreateDefaultPropertyStore(this.ParentShellObject);
            this.AddProperties(nativePropertyStore);
        }
        catch
        {
            parent?.Dispose();
            throw;
        }
        finally
        {
            if (nativePropertyStore is not null)
            {
                Marshal.ReleaseComObject(nativePropertyStore);
            }
        }
    }

    private IPropertyStore? NativePropertyStore { get; set; }

    private ShellObject ParentShellObject { get; }

    /// <summary>Release the native objects.</summary>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Creates the default property store.
    /// </summary>
    /// <param name="shellObj">The shell object.</param>
    /// <returns>The default property store.</returns>
    internal static IPropertyStore CreateDefaultPropertyStore(ShellObject shellObj)
    {
        var guid = new Guid(ShellIIDGuid.IPropertyStore);
        var hr = shellObj.GetNativeShellItem2().GetPropertyStore(
               ShellNativeMethods.GetPropertyStoreOptions.BestEffort,
               ref guid,
               out var nativePropertyStore);

        // throw on failure
        if (nativePropertyStore is null || !CoreErrorHelper.Succeeded(hr))
        {
            throw new ShellException(hr);
        }

        return nativePropertyStore;
    }

    /// <summary>
    /// Creates the typed property.
    /// </summary>
    /// <param name="propKey">The property key.</param>
    /// <param name="nativePropertyStore">The native property store.</param>
    /// <returns>The shell property.</returns>
    internal static IShellProperty CreateTypedProperty(PropertyKey propKey, IPropertyStore nativePropertyStore) => ShellPropertyFactory.CreateShellProperty(propKey, nativePropertyStore);

    /// <summary>Release the native and managed objects.</summary>
    /// <param name="disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (this.NativePropertyStore is not null)
        {
            Marshal.ReleaseComObject(this.NativePropertyStore);
            this.NativePropertyStore = default;
        }
    }

    private void AddProperties(IPropertyStore nativePropertyStore)
    {
        if (this.NativePropertyStore is null)
        {
            throw new InvalidOperationException();
        }

        // Populate the property collection
        nativePropertyStore.GetCount(out var propertyCount);
        for (uint i = 0; i < propertyCount; i++)
        {
            nativePropertyStore.GetAt(i, out var propKey);

            if (this.ParentShellObject is not null)
            {
                this.Items.Add(this.ParentShellObject.Properties.CreateTypedProperty(propKey));
            }
            else
            {
                this.Items.Add(CreateTypedProperty(propKey, this.NativePropertyStore));
            }
        }
    }
}
