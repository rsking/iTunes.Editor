// <copyright file="ShellHelper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell;

using System;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Microsoft.WindowsAPICodePack.Shell.Resources;
using MS.WindowsAPICodePack.Internal;

/// <summary>A helper class for Shell Objects.</summary>
internal static class ShellHelper
{
    /// <summary>
    /// The item type property key.
    /// </summary>
    private static PropertyKey itemTypePropertyKey = new(new Guid("28636AA6-953D-11D2-B5D6-00C04FD918D0"), 11);

    /// <summary>
    /// Gets the absolute path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>The absolute path.</returns>
    internal static string GetAbsolutePath(string path)
    {
        if (Uri.IsWellFormedUriString(path, UriKind.Absolute))
        {
            return path;
        }

        return Path.GetFullPath(path);
    }

    /// <summary>
    /// Gets the item type.
    /// </summary>
    /// <param name="shellItem">The shell item.</param>
    /// <returns>The item type.</returns>
    internal static string? GetItemType(IShellItem2? shellItem)
    {
        if (shellItem is not null)
        {
            var hr = shellItem.GetString(ref itemTypePropertyKey, out var itemType);
            if (hr == HResult.Ok)
            {
                return itemType;
            }
        }

        return default;
    }

    /// <summary>
    /// Gets the parsing name.
    /// </summary>
    /// <param name="shellItem">The shell item.</param>
    /// <returns>The parsing name.</returns>
    internal static string? GetParsingName(IShellItem shellItem)
    {
        if (shellItem is null)
        {
            return default;
        }

        var hr = shellItem.GetDisplayName(ShellNativeMethods.ShellItemDesignNameOptions.DesktopAbsoluteParsing, out var pszPath);

        if (hr != HResult.Ok && hr != HResult.InvalidArguments)
        {
            throw new ShellException(LocalizedMessages.ShellHelperGetParsingNameFailed, hr);
        }

        if (pszPath == IntPtr.Zero)
        {
            return default;
        }

        var path = Marshal.PtrToStringAuto(pszPath);
        Marshal.FreeCoTaskMem(pszPath);
        return path;
    }

    /// <summary>
    /// Gets the PIDL from the shell item.
    /// </summary>
    /// <param name="nativeShellItem">The native shell item.</param>
    /// <returns>The PIDL.</returns>
    internal static IntPtr PidlFromShellItem(IShellItem nativeShellItem) => PidlFromUnknown(Marshal.GetIUnknownForObject(nativeShellItem));

    /// <summary>
    /// Gets the PIDL from unknown.
    /// </summary>
    /// <param name="unknown">The unknown item.</param>
    /// <returns>The PIDL.</returns>
    internal static IntPtr PidlFromUnknown(IntPtr unknown) => CoreErrorHelper.Succeeded(ShellNativeMethods.SHGetIDListFromObject(unknown, out var pidl)) ? pidl : IntPtr.Zero;
}
