// <copyright file="ShellFile.cs" company="Microsoft">
// Copyright(c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell;

using System.IO;
using Microsoft.WindowsAPICodePack.Shell.Resources;

/// <summary>A file in the Shell Namespace.</summary>
public class ShellFile : ShellObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellFile"/> class.
    /// </summary>
    /// <param name="path">The path.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "MA0056:Do not call overridable members in constructor", Justification = "This is required.")]
    internal ShellFile(string path)
    {
        // Get the absolute path
        var absPath = ShellHelper.GetAbsolutePath(path);

        // Make sure this is valid
        if (!File.Exists(absPath))
        {
            throw new FileNotFoundException(
                string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    LocalizedMessages.FilePathNotExist,
                    path));
        }

        this.ParsingName = absPath;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellFile"/> class.
    /// </summary>
    /// <param name="shellItem">The shell item.</param>
    internal ShellFile(IShellItem2 shellItem) => this.SetNativeShellItem(shellItem);

    /// <summary>Gets the path for this file.</summary>
    public virtual string Path => this.ParsingName;

    /// <summary>Constructs a new ShellFile object given a file path.</summary>
    /// <param name="path">The file or folder path.</param>
    /// <returns>ShellFile object created using given file path.</returns>
    public static ShellFile FromFilePath(string path) => new(path);
}
