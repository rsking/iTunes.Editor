// <copyright file="ShellObjectFactory.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell;

using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.Resources;
using MS.WindowsAPICodePack.Internal;

/// <summary>
/// The shell object factory.
/// </summary>
internal static class ShellObjectFactory
{
    /// <summary>Creates a <see cref="ShellObject"/> given a native <see cref="IShellItem"/> interface.</summary>
    /// <param name="nativeShellItem">The native shell item.</param>
    /// <returns>A newly constructed ShellObject object.</returns>
    internal static ShellObject Create(IShellItem nativeShellItem)
    {
        // Sanity check
        Debug.Assert(nativeShellItem is not null, "nativeShellItem should not be null");

        // Need to make sure we're running on Vista or higher
        if (!CoreHelpers.RunningOnVista)
        {
            throw new PlatformNotSupportedException(LocalizedMessages.ShellObjectFactoryPlatformNotSupported);
        }

        // A lot of APIs need IShellItem2, so just keep a copy of it here
        if (nativeShellItem is not IShellItem2 nativeShellItem2)
        {
            throw new InvalidOperationException();
        }

        // Get the System.ItemType property
        var itemType = ShellHelper.GetItemType(nativeShellItem2) is string tempType
            ? tempType.ToLowerInvariant()
            : default;

        // Get some IShellItem attributes
        nativeShellItem2.GetAttributes(ShellNativeMethods.ShellFileGetAttributesOptions.FileSystem | ShellNativeMethods.ShellFileGetAttributesOptions.Folder, out var sfgao);

        // Is this item a FileSystem item?
        var isFileSystem = (sfgao & ShellNativeMethods.ShellFileGetAttributesOptions.FileSystem) != 0;

        // Is this item a Folder?
        var isFolder = (sfgao & ShellNativeMethods.ShellFileGetAttributesOptions.Folder) != 0;

        // Create the right type of ShellObject based on the above information

        // 1. First check if this is a Shell Link
        if (StringComparer.OrdinalIgnoreCase.Equals(itemType, ".lnk"))
        {
            throw new NotSupportedException();
        }

        // 2. Check if this is a container or a single item (entity)
        if (isFolder)
        {
            throw new NotSupportedException();
        }

        // 6. If this is an entity (single item), check if its filesystem or not
        if (isFileSystem)
        {
            return new ShellFile(nativeShellItem2);
        }

        return new ShellNonFileSystemItem(nativeShellItem2);
    }

    /// <summary>Creates a <see cref="ShellObject"/> given a parsing name.</summary>
    /// <param name="parsingName">The parsing name.</param>
    /// <returns>A newly constructed ShellObject object.</returns>
    internal static ShellObject Create(string parsingName)
    {
        if (string.IsNullOrEmpty(parsingName))
        {
            throw new ArgumentNullException(nameof(parsingName));
        }

        // Create a native shellitem from our path
        var guid = new Guid(ShellIIDGuid.IShellItem2);
        var retCode = ShellNativeMethods.SHCreateItemFromParsingName(
            parsingName,
            IntPtr.Zero,
            ref guid,
            out var nativeShellItem); // Create a native shellitem from our path

        if (!CoreErrorHelper.Succeeded(retCode))
        {
            throw new ShellException(LocalizedMessages.ShellObjectFactoryUnableToCreateItem, Marshal.GetExceptionForHR(retCode));
        }

        return Create(nativeShellItem);
    }
}
