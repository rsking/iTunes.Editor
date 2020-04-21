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
        public override void Initialize() => Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);

        /// <inheritdoc/>
        public override void OnFrameworkInitializationCompleted()
        {
            switch (this.ApplicationLifetime)
            {
                case Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop:
                    desktop.MainWindow = new Views.ShellView { DataContext = new ViewModels.ShellViewModel() };
                    break;
                case Avalonia.Controls.ApplicationLifetimes.ISingleViewApplicationLifetime singleView:
                    singleView.MainView = new Views.ShellView { DataContext = new ViewModels.ShellViewModel() };
                    break;
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
