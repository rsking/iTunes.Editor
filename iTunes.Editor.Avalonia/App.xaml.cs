// -----------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The main application.
    /// </summary>
    public class App : Avalonia.Application
    {
        /// <inheritdoc/>
        public override void Initialize()
        {
            Avalonia.Markup.Xaml.AvaloniaXamlLoaderPortableXaml.Load(this);
        }
    }
}
