// <copyright file="ITunesConfigureView.axaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Views;

using global::Avalonia.Controls;
using global::Avalonia.Markup.Xaml;

/// <summary>
/// The ITunes configuration view.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "This is for iTunes")]
public partial class ITunesConfigureView : UserControl
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ITunesConfigureView"/> class.
    /// </summary>
    public ITunesConfigureView() => this.InitializeComponent();

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
