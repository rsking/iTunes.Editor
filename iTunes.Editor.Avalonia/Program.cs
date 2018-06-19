// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using Avalonia;
    using Avalonia.Logging.Serilog;
    using ITunes.Editor.Models;
    using ITunes.Editor.ViewModels;
    using ITunes.Editor.Views;
    using Ninject;

    /// <summary>
    /// The program entry.
    /// </summary>
    internal static class Program
    {
        private static IKernel container;

        /// <summary>
        /// Builds the Avalonia application.
        /// </summary>
        /// <returns>The application builder.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .UseReactiveUI()
                .LogToDebug()
                .UseNinject();

        /// <summary>
        /// The main entry point.
        /// </summary>
        private static void Main() => BuildAvaloniaApp().Start<ShellView>(() => container.Get<IShell>());

        /// <summary>
        /// Use Ninject.
        /// </summary>
        /// <typeparam name="TAppBuilder">The type of app builder.</typeparam>
        /// <param name="builder">The builder.</param>
        /// <returns>The updated builder.</returns>
        private static TAppBuilder UseNinject<TAppBuilder>(this TAppBuilder builder)
            where TAppBuilder : Avalonia.Controls.AppBuilderBase<TAppBuilder>, new()
            => builder.AfterSetup(_ =>
            {
                container = new StandardKernel();
                container.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

                container.Bind<IShell>().To<ShellViewModel>();
                container.Bind<ILoad>().To<LoadViewModel>();
                container.Bind<ISongs>().To<SongsViewModel>();

                // Services
                container.Bind<Services.Contracts.ISelectFolder>().To<Services.SelectFolderDialog>();
                container.Bind<Services.Contracts.IOpenFile>().To<Services.OpenFileDialog>();
            });
    }
}
