// <copyright file="ShellView.axaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Views;

using global::Avalonia;
using global::Avalonia.Markup.Xaml;

/// <summary>
/// Interaction logic for <c>ShellView.axaml</c>.
/// </summary>
public partial class ShellView : global::Avalonia.ReactiveUI.ReactiveWindow<ViewModels.ShellViewModel>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellView"/> class.
    /// </summary>
    public ShellView()
        : this(new ViewModels.ShellViewModel())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellView"/> class.
    /// </summary>
    /// <param name="screen">The screen.</param>
    public ShellView(ReactiveUI.IScreen screen)
    {
        this.InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
        this.DataContext = screen;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
}
