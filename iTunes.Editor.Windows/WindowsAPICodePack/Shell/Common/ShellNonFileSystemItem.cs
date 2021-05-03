// <copyright file="ShellNonFileSystemItem.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell
{
    /// <summary>Represents a non filesystem item (e.g. virtual items inside Control Panel).</summary>
    public class ShellNonFileSystemItem : ShellObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellNonFileSystemItem"/> class.
        /// </summary>
        /// <param name="shellItem">The shell item.</param>
        internal ShellNonFileSystemItem(IShellItem2 shellItem) => this.SetNativeShellItem(shellItem);
    }
}
