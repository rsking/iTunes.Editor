// <copyright file="ShellEnums.cs" company="Microsoft">
// Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell
{
#pragma warning disable MA0048, SA1649
    /// <summary>One of the values that indicates how the ShellObject DisplayName should look.</summary>
    public enum DisplayNameType
    {
        /// <summary>Returns the parsing name relative to the parent folder.</summary>
        RelativeToParent = unchecked((int)0x80018001),

        /// <summary>Returns the parsing name relative to the desktop.</summary>
        RelativeToDesktop = unchecked((int)0x80028000),

        /// <summary>Returns the editing name relative to the parent folder.</summary>
        RelativeToParentEditing = unchecked((int)0x80031001),

        /// <summary>Returns the editing name relative to the desktop.</summary>
        RelativeToDesktopEditing = unchecked((int)0x8004c000),

        /// <summary>Returns the display name relative to the file system path.</summary>
        FileSystemPath = unchecked((int)0x80058000),

        /// <summary>Returns the display name relative to a URL.</summary>
        Url = unchecked((int)0x80068000),

        /// <summary>Returns the path relative to the parent folder in a friendly format as displayed in an address bar.</summary>
        RelativeToParentAddressBar = unchecked((int)0x8007c001),

        /// <summary>Returns the display name relative to the desktop.</summary>
        Default = 0x00000000,
    }
#pragma warning restore MA0048, SA1649
}
