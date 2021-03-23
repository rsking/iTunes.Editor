// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using global::Avalonia;
    using global::Avalonia.ReactiveUI;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The program entry.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// Builds the Avalonia application.
        /// </summary>
        /// <returns>The application builder.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseHost(Host.CreateDefaultBuilder)
                .UseReactiveUI();

        /// <summary>
        /// The main entry point.
        /// </summary>
        private static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        private static TAppBuilder UseHost<TAppBuilder>(this TAppBuilder builder, System.Func<IHostBuilder> hostBuilderFactory, System.Action<IHostBuilder>? configure = default)
            where TAppBuilder : global::Avalonia.Controls.AppBuilderBase<TAppBuilder>, new() =>
            builder.AfterSetup(_ =>
            {
                var hostBuilder = hostBuilderFactory();

                hostBuilder.UseDefaultITunes();

                configure?.Invoke(hostBuilder);

                hostBuilder.ConfigureServices(serviceCollection => serviceCollection.AddTransient<Models.IShell, ViewModels.ShellViewModel>());
            });
    }
}
