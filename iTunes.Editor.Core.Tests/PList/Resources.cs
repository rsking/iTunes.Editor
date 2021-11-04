// -----------------------------------------------------------------------
// <copyright file="Resources.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PList;

/// <summary>
/// Resources class.
/// </summary>
internal static class Resources
{
    /// <summary>
    /// Gets the test binary.
    /// </summary>
    public static Stream TestBin => typeof(Resources).Assembly.GetManifestResourceStream(typeof(Resources).Namespace + ".testBin.plist")
        ?? throw new InvalidOperationException();

    /// <summary>
    /// Gets the test XML.
    /// </summary>
    public static Stream TestXml => typeof(Resources).Assembly.GetManifestResourceStream(typeof(Resources).Namespace + ".testXml.plist")
        ?? throw new InvalidOperationException();
}
