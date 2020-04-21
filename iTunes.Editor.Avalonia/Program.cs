// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using Avalonia;
    using Avalonia.Logging.Serilog;
    using Avalonia.ReactiveUI;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The program entry.
    /// </summary>
    internal static class Program
    {
        private static IHost? host;

        /// <summary>
        /// Builds the Avalonia application.
        /// </summary>
        /// <returns>The application builder.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug()
                .UseHost(Host.CreateDefaultBuilder, ConfigureHost)
                .UseReactiveUI();

        /// <summary>
        /// The main entry point.
        /// </summary>
        private static void Main(string[] args) => BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        private static TAppBuilder UseHost<TAppBuilder>(this TAppBuilder builder, System.Func<IHostBuilder> hostBuilderFactory, System.Action<IHostBuilder> configure)
            where TAppBuilder : Avalonia.Controls.AppBuilderBase<TAppBuilder>, new() =>
            builder.AfterSetup(_ =>
            {
                var hostBuilder = hostBuilderFactory();

                configure(hostBuilder);

                hostBuilder.ConfigureServices(serviceCollection => serviceCollection.AddTransient<Models.IShell, ViewModels.ShellViewModel>());

                host = hostBuilder.Build();
            });

        private static void ConfigureHost(IHostBuilder hostBuilder) => hostBuilder.ConfigureServices((hostingContext, serviceCollection) =>
        {
            // Lyrics
            serviceCollection
                .AddWikia()
                .AddAZ()
                .AddGenius()
                .AddChartLyrics()
                .AddApiSeeds(hostingContext.Configuration)
                .AddPurgoMalum();

            // Composers
            serviceCollection
                .AddApraAmcos();

            // song providers
            serviceCollection
                .AddFolder()
                .AddIPod()
                .AddPList()
                .AddITunes();

            // tag provider
            serviceCollection
                .AddTagLib()
                .AddMediaInfo();

            // add services
            serviceCollection
                .AddTransient<IUpdateComposerService, UpdateComposerService>()
                .AddTransient<IUpdateLyricsService, UpdateLyricsService>();
        });
    }
}
