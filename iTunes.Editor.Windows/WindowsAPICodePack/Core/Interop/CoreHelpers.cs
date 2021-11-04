// <copyright file="CoreHelpers.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace MS.WindowsAPICodePack.Internal;

using System;

/// <summary>Common Helper methods.</summary>
public static class CoreHelpers
{
    /// <summary>Gets a value indicating whether the application is running on Vista.</summary>
    public static bool RunningOnVista => System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows)
        && Environment.OSVersion.Version.Major >= 6;
}
