// <copyright file="WpfApplicationLifetimeHostBuilderExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor
{
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// The WPF extensions.
    /// </summary>
    internal static class WpfApplicationLifetimeHostBuilderExtensions
    {
        /// <summary>
        /// Use WPF application lifetime.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns>The host builder.</returns>
        public static Microsoft.Extensions.Hosting.IHostBuilder UseWpfApplicationLifetime(this Microsoft.Extensions.Hosting.IHostBuilder hostBuilder) => hostBuilder.ConfigureServices(
                (_, serviceCollection) => serviceCollection.AddSingleton<Microsoft.Extensions.Hosting.IHostLifetime, WpfApplicationLifetime>());
    }
}
