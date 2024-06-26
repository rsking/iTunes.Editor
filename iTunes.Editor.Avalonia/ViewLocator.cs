﻿// -----------------------------------------------------------------------
// <copyright file="ViewLocator.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

using global::Avalonia.Controls;
using global::Avalonia.Controls.Templates;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The view locator.
/// </summary>
public class ViewLocator : IDataTemplate
{
    /// <inheritdoc/>
    public Control? Build(object? param)
    {
        if (param is null)
        {
            return new TextBlock { Text = Avalonia.Properties.Resources.NotFoundLabel };
        }

        var name = param.GetType().FullName?
#if NETCOREAPP
            .Replace("ViewModel", "View", StringComparison.Ordinal);
#else
            .Replace("ViewModel", "View");
#endif

        return name switch
        {
            not null when Type.GetType(name) is Type type && CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService(type) is Control control => control,
            _ => new TextBlock { Text = $"Not Found: {name}" },
        };
    }

    /// <inheritdoc/>
    public bool Match(object? data) => data is CommunityToolkit.Mvvm.ComponentModel.ObservableObject;
}
