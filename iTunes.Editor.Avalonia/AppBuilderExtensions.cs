// <copyright file="AppBuilderExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The <see cref="global::Avalonia.Controls.AppBuilderBase{T}"/> extensions.
    /// </summary>
    internal static class AppBuilderExtensions
    {
        /// <summary>
        /// Use the host builder.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of app builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <param name="hostBuilderFactory">The host builder factory.</param>
        /// <param name="configure">Action to condifigure the host builder.</param>
        /// <returns>The app builder.</returns>
        public static TAppBuilder UseHost<TAppBuilder>(this TAppBuilder builder, System.Func<IHostBuilder> hostBuilderFactory, System.Action<IHostBuilder>? configure = default)
            where TAppBuilder : global::Avalonia.Controls.AppBuilderBase<TAppBuilder>, new() =>
            builder.AfterSetup(_ =>
            {
                var hostBuilder = hostBuilderFactory()
                    .UseDefaultITunes()
                    .ConfigureServices((_, services) =>
                    {
                        // services
                        services.AddSingleton<IEventAggregator, EventAggregator>();
                        services.AddTransient<Services.Contracts.IOpenFile, Services.OpenFileDialog>();
                        services.AddTransient<Services.Contracts.ISelectFolder, Services.SelectFolderDialog>();

                        // view models
                        services.AddSingleton<ReactiveUI.IScreen, ViewModels.ShellViewModel>();
                        services.AddSingleton<Models.ILoad, ViewModels.LoadRoutableViewModel>();
                        services.AddSingleton<Models.ISongs, ViewModels.SongsRoutableViewModel>();

                        // views
                        services.AddSingleton<Views.ShellView>();
                        services.AddTransient<Views.LoadView>();
                        services.AddTransient<Views.SongsView>();
                    });

                configure?.Invoke(hostBuilder);
            });
    }
}
