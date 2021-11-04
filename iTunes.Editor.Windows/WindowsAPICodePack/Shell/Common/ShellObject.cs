// <copyright file="ShellObject.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell;

using System.IO;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell.Resources;
using MS.WindowsAPICodePack.Internal;

/// <summary>The base class for all Shell objects in Shell Namespace.</summary>
public abstract class ShellObject : IDisposable, IEquatable<ShellObject>, IEqualityComparer<ShellObject>, System.Collections.IEqualityComparer
{
    private IShellItem2? nativeShellItem;

    private string? internalName;

    private string? internalParsingName;

    private IntPtr internalPIDL = IntPtr.Zero;

    private int? hashValue;

    private ShellProperties? properties;

    /// <summary>Gets a value indicating whether this feature is supported on the current platform.</summary>
    public static bool IsPlatformSupported => CoreHelpers.RunningOnVista; // We need Windows Vista onwards ...

    /// <summary>Gets a value indicating whether if this ShellObject is a file system object.</summary>
    public bool IsFileSystemObject
    {
        get
        {
            try
            {
                this.NativeShellItem.GetAttributes(ShellNativeMethods.ShellFileGetAttributesOptions.FileSystem, out var sfgao);
                return (sfgao & ShellNativeMethods.ShellFileGetAttributesOptions.FileSystem) != 0;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (NullReferenceException)
            {
                // NativeShellItem is null
                return false;
            }
        }
    }

    /// <summary>Gets a value indicating whether this ShellObject is a link or shortcut.</summary>
    public bool IsLink
    {
        get
        {
            try
            {
                this.NativeShellItem.GetAttributes(ShellNativeMethods.ShellFileGetAttributesOptions.Link, out var sfgao);
                return (sfgao & ShellNativeMethods.ShellFileGetAttributesOptions.Link) != 0;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (NullReferenceException)
            {
                // NativeShellItem is null
                return false;
            }
        }
    }

    /// <summary>Gets the normal display for this ShellItem.</summary>
    public virtual string? Name
    {
        get
        {
            if (this.internalName is null && this.NativeShellItem is not null)
            {
                var hr = this.NativeShellItem.GetDisplayName(ShellNativeMethods.ShellItemDesignNameOptions.Normal, out var pszString);
                if (hr == HResult.Ok && pszString != IntPtr.Zero)
                {
                    this.internalName = Marshal.PtrToStringAuto(pszString);

                    // Free the string
                    Marshal.FreeCoTaskMem(pszString);
                }
            }

            return this.internalName;
        }
    }

    /// <summary>Gets or sets the parsing name for this ShellItem.</summary>
    public virtual string ParsingName
    {
        get
        {
            if (this.internalParsingName is null
                && this.nativeShellItem is not null
                && ShellHelper.GetParsingName(this.nativeShellItem) is string parsingName)
            {
                this.internalParsingName = parsingName;
            }

            return this.internalParsingName ?? string.Empty;
        }

        protected set => this.internalParsingName = value;
    }

    /// <summary>Gets an object that allows the manipulation of ShellProperties for this shell item.</summary>
    public ShellProperties Properties => this.properties ??= new ShellProperties(this);

    /// <summary>
    /// Gets access to the native IPropertyStore (if one is already created for this item and still valid. This is usually done by the
    /// ShellPropertyWriter class. The reference will be set to null when the writer has been closed/commited).
    /// </summary>
    internal IPropertyStore? NativePropertyStore { get; private set; }

    /// <summary>Gets the native ShellFolder object.</summary>
    internal virtual IShellItem NativeShellItem => this.GetNativeShellItem2();

    /// <summary>Gets the PID List (PIDL) for this ShellItem.</summary>
    internal virtual IntPtr PIDL
    {
        get
        {
            // Get teh PIDL for the ShellItem
            if (this.internalPIDL == IntPtr.Zero && this.NativeShellItem is not null)
            {
                this.internalPIDL = ShellHelper.PidlFromShellItem(this.NativeShellItem);
            }

            return this.internalPIDL;
        }
    }

    /// <summary>Implements the != (inequality) operator.</summary>
    /// <param name="leftShellObject">First object to compare.</param>
    /// <param name="rightShellObject">Second object to compare.</param>
    /// <returns>True if leftShellObject does not equal leftShellObject; false otherwise.</returns>
    public static bool operator !=(ShellObject leftShellObject, ShellObject rightShellObject) => !(leftShellObject == rightShellObject);

    /// <summary>Implements the == (equality) operator.</summary>
    /// <param name="leftShellObject">First object to compare.</param>
    /// <param name="rightShellObject">Second object to compare.</param>
    /// <returns>True if leftShellObject equals rightShellObject; false otherwise.</returns>
    public static bool operator ==(ShellObject leftShellObject, ShellObject rightShellObject)
    {
        if (leftShellObject is null)
        {
            return rightShellObject is null;
        }

        return leftShellObject.Equals(rightShellObject);
    }

    /// <summary>
    /// Creates a ShellObject subclass given a parsing name. For file system items, this method will only accept absolute paths.
    /// </summary>
    /// <param name="parsingName">The parsing name of the object.</param>
    /// <returns>A newly constructed ShellObject object.</returns>
    public static ShellObject FromParsingName(string parsingName) => ShellObjectFactory.Create(parsingName);

    /// <summary>Release the native objects.</summary>
    public void Dispose()
    {
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc/>
    public virtual bool Equals(ShellObject other) => this.Equals(this, other);

    /// <inheritdoc/>
    public override bool Equals(object obj) => obj is ShellObject shellObject && this.Equals(shellObject);

    /// <inheritdoc/>
    public bool Equals(ShellObject x, ShellObject y)
    {
        if (x is null)
        {
            return y is null;
        }

        if (y is not null)
        {
            var ifirst = x.NativeShellItem;
            var isecond = y.NativeShellItem;
            if (ifirst is not null && isecond is not null)
            {
                var hr = ifirst.Compare(
                    isecond, SICHINTF.SICHINT_ALLFIELDS, out var result);

                return (hr == HResult.Ok) && (result == 0);
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public int GetHashCode(ShellObject obj)
    {
        if (!obj.hashValue.HasValue)
        {
            var size = ShellNativeMethods.ILGetSize(this.PIDL);
            if (size != 0)
            {
                var pidlData = new byte[size];
                Marshal.Copy(obj.PIDL, pidlData, 0, (int)size);

                // Using FNV-1a hash algorithm because a cryptographically secure algorithm is not required for this use
                const int p = 16777619;
                var hash = -2128831035;

                for (var i = 0; i < pidlData.Length; i++)
                {
                    hash = (hash ^ pidlData[i]) * p;
                }

                obj.hashValue = hash;
            }
            else
            {
                obj.hashValue = 0;
            }
        }

        return obj.hashValue.Value;
    }

    /// <inheritdoc/>
    public new bool Equals(object x, object y)
    {
        if (x == y)
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        if (x is ShellObject a
            && y is ShellObject b)
        {
            return this.Equals(a, b);
        }

        throw new ArgumentException($"{nameof(x)} or {nameof(y)} are not {nameof(ShellObject)}", nameof(x));
    }

    /// <inheritdoc/>
    public int GetHashCode(object obj)
    {
        if (obj is null)
        {
            return 0;
        }

        if (obj is ShellObject x)
        {
            return this.GetHashCode(x);
        }

        throw new ArgumentException($"{nameof(obj)} is not {nameof(ShellObject)}", nameof(obj));
    }

    /// <inheritdoc/>
    public override int GetHashCode() => this.GetHashCode(this);

    /// <inheritdoc/>
    public override string ToString() => this.Name ?? base.ToString();

    /// <summary>Return the native ShellFolder object as newer IShellItem2.</summary>
    /// <exception cref="ShellException">
    /// If the native object cannot be created. The ErrorCode member will contain the external error code.
    /// </exception>
    /// <returns>The <see cref="IShellItem2"/>.</returns>
    internal virtual IShellItem2 GetNativeShellItem2()
    {
        if (this.nativeShellItem is null)
        {
            if (this.ParsingName is null)
            {
                throw new InvalidOperationException();
            }

            var guid = new Guid(ShellIIDGuid.IShellItem2);
            var retCode = ShellNativeMethods.SHCreateItemFromParsingName(this.ParsingName, IntPtr.Zero, ref guid, out this.nativeShellItem);

            if (this.nativeShellItem is null || !CoreErrorHelper.Succeeded(retCode))
            {
                throw new ShellException(
                    LocalizedMessages.ShellObjectCreationFailed,
                    Marshal.GetExceptionForHR(retCode));
            }
        }

        return this.nativeShellItem;
    }

    /// <summary>
    /// Sets the native shell.
    /// </summary>
    /// <param name="nativeShellItem">The native shell.</param>
    internal void SetNativeShellItem(IShellItem2? nativeShellItem) => this.nativeShellItem = nativeShellItem;

    /// <summary>Release the native and managed objects.</summary>
    /// <param name="disposing">Indicates that this is being called from Dispose(), rather than the finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.internalName = null;
            this.internalParsingName = null;
            this.properties = null;
        }

        if (this.properties is not null)
        {
            this.properties = null;
        }

        if (this.internalPIDL != IntPtr.Zero)
        {
            ShellNativeMethods.ILFree(this.internalPIDL);
            this.internalPIDL = IntPtr.Zero;
        }

        if (this.nativeShellItem is not null)
        {
            Marshal.ReleaseComObject(this.nativeShellItem);
            this.nativeShellItem = null;
        }

        if (this.NativePropertyStore is not null)
        {
            Marshal.ReleaseComObject(this.NativePropertyStore);
            this.NativePropertyStore = null;
        }
    }
}
