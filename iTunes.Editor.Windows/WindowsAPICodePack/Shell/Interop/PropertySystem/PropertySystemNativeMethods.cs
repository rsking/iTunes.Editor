﻿// <copyright file="PropertySystemNativeMethods.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem;

using System.Runtime.InteropServices;
using MS.WindowsAPICodePack.Internal;

#pragma warning disable SA1600, SA1602
internal static class PropertySystemNativeMethods
{
    internal enum RelativeDescriptionType
    {
        General,
        Date,
        Size,
        Count,
        Revision,
        Length,
        Duration,
        Speed,
        Rate,
        Rating,
        Priority,
    }

    [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int PSGetNameFromPropertyKey(
        ref PropertyKey propkey,
        [Out, MarshalAs(UnmanagedType.LPWStr)] out string ppszCanonicalName);

    [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern HResult PSGetPropertyDescription(
        ref PropertyKey propkey,
        ref Guid riid,
        [Out, MarshalAs(UnmanagedType.Interface)] out IPropertyDescription ppv);

    [DllImport("propsys.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int PSGetPropertyKeyFromName(
        [In, MarshalAs(UnmanagedType.LPWStr)] string pszCanonicalName,
        out PropertyKey propkey);
}
