// -----------------------------------------------------------------------
// <copyright file="ShellView.xaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Views
{
    using global::Avalonia;
    using global::Avalonia.Controls;
    using global::Avalonia.Markup.Xaml;

    /// <summary>
    /// The shell view.
    /// </summary>
    public class ShellView : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellView"/> class.
        /// </summary>
        public ShellView()
        {
            this.InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
