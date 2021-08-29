// <copyright file="LoadView.xaml.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Views
{
    using System;

    /// <summary>
    /// Interaction logic for <c>LoadView.xaml</c>.
    /// </summary>
    public partial class LoadView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadView"/> class.
        /// </summary>
        /// <param name="viewModel">The view model.</param>
        public LoadView(Models.ILoad viewModel)
        {
            this.InitializeComponent();
            this.DataContext = viewModel;
        }

        private async void OnProviderButton(object sender, System.Windows.RoutedEventArgs e)
        {
            var frameworkElement = sender as System.Windows.FrameworkElement ?? throw new ArgumentException($"{nameof(sender)} must be a {nameof(System.Windows.FrameworkElement)}", nameof(sender));
            var provider = frameworkElement.DataContext as ISongsProvider ?? throw new ArgumentException($"{nameof(sender)} must have a data context of {nameof(ISongsProvider)}", nameof(sender));

            if (this.DataContext is Models.ILoad load)
            {
                _ = this.NavigationService.Navigate(Microsoft.Toolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<SongsView>());
                await load.LoadAsync(provider).ConfigureAwait(true);
            }
        }
    }
}
