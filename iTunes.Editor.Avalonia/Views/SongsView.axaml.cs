// <copyright file="SongsView.axaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Views;

using global::Avalonia.Markup.Xaml;
using ReactiveUI;

/// <summary>
/// The songs view.
/// </summary>
public class SongsView : global::Avalonia.ReactiveUI.ReactiveUserControl<ViewModels.SongsRoutableViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SongsView"/> class.
    /// </summary>
    public SongsView()
    {
        this.WhenActivated(_ => { });
        this.InitializeComponent();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
