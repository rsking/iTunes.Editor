// <copyright file="App.xaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Toolkit.Mvvm.DependencyInjection;

    /// <summary>
    /// The application.
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.host = Host.CreateDefaultBuilder()
                .UseDefaultITunes()
                .ConfigureServices((_, services) =>
                {
                    // services
                    services.AddSingleton<IEventAggregator, EventAggregator>();
                    services.AddTransient<Services.Contracts.IOpenFile, Services.OpenFileDialog>();
                    services.AddTransient<Services.Contracts.ISelectFolder, Services.SelectFolderDialog>();

                    // view models
                    services.AddSingleton<Models.ILoad, ViewModels.LoadViewModel>();
                    services.AddSingleton<Models.ISongs, ViewModels.SongsViewModel>();

                    // views
                    services.AddSingleton<ShellView>();
                    services.AddTransient<Views.LoadView>();
                    services.AddTransient<Views.SongsView>();
                })
                .Build();

            Collections.ObservableHelper.SetObservableCollectionType(typeof(Collections.ObservableImmutableList<>));
            Collections.ObservableHelper.SetObservableListType(typeof(Collections.ObservableImmutableList<>));

            Ioc.Default.ConfigureServices(this.host.Services);
        }

        /// <inheritdoc/>
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            if (this.MainWindow is System.Windows.Navigation.NavigationWindow navigationWindow)
            {
                if (navigationWindow.Content is null)
                {
                    navigationWindow.NavigationFailed += this.OnNavigationFailed;
                    navigationWindow.Navigate(Ioc.Default.GetRequiredService<Views.LoadView>());
                }

                this.MainWindow.Activate();

                return;
            }

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (this.MainWindow.Content is not Frame rootFrame)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame
                {
                    // Set the default language
                    Language = System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentUICulture.Name),
                };

                rootFrame.NavigationFailed += this.OnNavigationFailed;

                // Place the frame in the current Window
                this.MainWindow.Content = rootFrame;
            }

            if (rootFrame.Content is null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(Ioc.Default.GetRequiredService<Views.LoadView>());
            }

            // Ensure the current window is active
            this.MainWindow.Activate();
        }

        /// <inheritdoc/>
        protected override async void OnStartup(StartupEventArgs e)
        {
            await this.host.StartAsync().ConfigureAwait(true);

            this.host.Services.GetRequiredService<ShellView>().Show();

            base.OnStartup(e);
        }

        /// <inheritdoc/>
        protected override async void OnExit(ExitEventArgs e)
        {
            using (this.host)
            {
                await this.host.StopAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(true);
            }

            base.OnExit(e);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails.
        /// </summary>
        /// <param name="sender">The Frame which failed navigation.</param>
        /// <param name="e">Details about the navigation failure.</param>
        private void OnNavigationFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            throw new InvalidOperationException("Failed to load Page " + e.Exception);
        }
    }
}
