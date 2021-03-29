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

    /// <summary>
    /// The program entry.
    /// </summary>
    internal sealed class Program
    {
        private Program()
        {
        }

        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <remarks>
        /// Don't use any Avalonia, third-party APIs or any SynchronizationContext-reliant code before AppMain is called:
        /// things aren't initialized yet and stuff might break.
        /// </remarks>
        public static void Main(string[] args) => BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        /// <summary>
        /// Builds the Avalonia application.
        /// </summary>
        /// <returns>The application builder.</returns>
        /// <remarks>Avalonia configuration, don't remove; also used by visual designer.</remarks>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseHost(Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder, builder => builder.ConfigureServices((_, services) => services.Configure<Microsoft.Extensions.Hosting.ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true)))
                .UseReactiveUI();
    }
}
