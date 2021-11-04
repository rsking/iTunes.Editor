// <copyright file="SongsView.xaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Views;

/// <summary>
/// The songs view.
/// </summary>
public partial class SongsView
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SongsView"/> class.
    /// </summary>
    /// <param name="songs">The songs.</param>
    public SongsView(Models.ISongs songs)
    {
        this.InitializeComponent();
        this.DataContext = songs;
    }
}
