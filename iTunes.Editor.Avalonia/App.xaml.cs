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
    public class App : global::Avalonia.Application
    {
        /// <inheritdoc/>
        public override void Initialize() => global::Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);

        /// <inheritdoc/>
        public override void OnFrameworkInitializationCompleted()
        {
            var viewModel = new ViewModels.ShellViewModel();

            switch (this.ApplicationLifetime)
            {
                case global::Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop:
                    desktop.MainWindow = new Views.ShellView { DataContext = viewModel };
                    break;
                case global::Avalonia.Controls.ApplicationLifetimes.ISingleViewApplicationLifetime singleView:
                    singleView.MainView = new Views.ShellView { DataContext = viewModel };
                    break;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
