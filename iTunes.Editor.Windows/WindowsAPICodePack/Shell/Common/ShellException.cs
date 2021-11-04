// <copyright file="ShellException.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell;

using System;
using System.Runtime.InteropServices;
using Microsoft.WindowsAPICodePack.Shell.Resources;
using MS.WindowsAPICodePack.Internal;

/// <summary>An exception thrown when an error occurs while dealing with ShellObjects.</summary>
[Serializable]
public class ShellException : ExternalException
{
    /// <inheritdoc cref="ExternalException()" />
    public ShellException()
    {
    }

    /// <inheritdoc cref="ExternalException(string)" />
    public ShellException(string message)
        : base(message)
    {
    }

    /// <inheritdoc cref="ExternalException(string,Exception)" />
    public ShellException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <inheritdoc cref="ExternalException(string,int)" />
    public ShellException(string message, int errorCode)
        : base(message, errorCode)
    {
    }

    /// <inheritdoc cref="ExternalException(string,int)" />
    public ShellException(int errorCode)
        : base(LocalizedMessages.ShellExceptionDefaultText, errorCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellException"/> class.
    /// </summary>
    /// <param name="result">The result.</param>
    internal ShellException(HResult result)
        : this((int)result)
    {
    }

    /// <inheritdoc cref="ExternalException(string,int)" />
    internal ShellException(string message, HResult errorCode)
        : this(message, (int)errorCode)
    {
    }

    /// <inheritdoc cref="ExternalException(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)" />
    protected ShellException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context)
        : base(info, context)
    {
    }
}
