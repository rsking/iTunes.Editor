// <copyright file="CoreErrorHelper.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace MS.WindowsAPICodePack.Internal;
#pragma warning disable MA0048, SA1649
/// <summary>HRESULT Wrapper.</summary>
public enum HResult
{
    /// <summary>E_NOINTERFACE.</summary>
    NoInterface = unchecked((int)0x80004002),

    /// <summary>E_FAIL.</summary>
    Fail = unchecked((int)0x80004005),

    /// <summary>TYPE_E_ELEMENTNOTFOUND.</summary>
    TypeElementNotFound = unchecked((int)0x8002802B),

    /// <summary>The requested resources is read-only.</summary>
    AccessDenied = unchecked((int)0x80030005),

    /// <summary>NO_OBJECT.</summary>
    NoObject = unchecked((int)0x800401E5),

    /// <summary>E_OUTOFMEMORY.</summary>
    OutOfMemory = unchecked((int)0x8007000E),

    /// <summary>E_INVALIDARG.</summary>
    InvalidArguments = unchecked((int)0x80070057),

    /// <summary>The requested resource is in use.</summary>
    ResourceInUse = unchecked((int)0x800700AA),

    /// <summary>E_ELEMENTNOTFOUND.</summary>
    ElementNotFound = unchecked((int)0x80070490),

    /// <summary>ERROR_CANCELLED.</summary>
    Canceled = unchecked((int)0x800704C7),

    /// <summary>S_OK.</summary>
    Ok = 0x0000,

    /// <summary>S_FALSE.</summary>
    False = 0x0001,

    /// <summary>Win32 Error code: ERROR_CANCELLED.</summary>
    Win32ErrorCanceled = 1223,
}

/// <summary>Provide Error Message Helper Methods. This is intended for Library Internal use only.</summary>
internal static class CoreErrorHelper
{
    /// <summary>This is intended for Library Internal use only.</summary>
    /// <param name="result">The error code.</param>
    /// <returns>True if the error code indicates success.</returns>
    public static bool Succeeded(int result) => result >= 0;

    /// <summary>This is intended for Library Internal use only.</summary>
    /// <param name="result">The error code.</param>
    /// <returns>True if the error code indicates success.</returns>
    public static bool Succeeded(HResult result) => Succeeded((int)result);
}
