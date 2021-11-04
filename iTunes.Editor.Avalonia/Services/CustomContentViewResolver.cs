// <copyright file="CustomContentViewResolver.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Services;

/// <summary>
/// The cusom content view resolver.
/// </summary>
internal class CustomContentViewResolver : global::Avalonia.Controls.Templates.IDataTemplate
{
    /// <inheritdoc/>
    public bool Match(object data) => data is Models.IConfigure;

    /// <inheritdoc/>
    public global::Avalonia.Controls.IControl Build(object param) =>
        param switch
        {
            ViewModels.ITunesConfigureViewModel _ => new Views.ITunesConfigureView(),
            _ => throw new InvalidOperationException(),
        };
}
