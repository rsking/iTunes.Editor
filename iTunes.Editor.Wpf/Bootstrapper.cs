// -----------------------------------------------------------------------
// <copyright file="Bootstrapper.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The boot strapper.
    /// </summary>
    public class Bootstrapper : Caliburn.Micro.BootstrapperBase
    {
        private IHost host = default(DummyHost);

        /// <summary>
        /// Initializes a new instance of the <see cref="Bootstrapper"/> class.
        /// </summary>
        public Bootstrapper() => this.Initialize();

        /// <inheritdoc/>
        protected override async void Configure()
        {
            this.host = Host.CreateDefaultBuilder()
                .UseDefaultITunes()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection.AddSingleton<Caliburn.Micro.IWindowManager, MetroWindowManager>();
                    serviceCollection.AddSingleton<IEventAggregator, EventAggregator>();

                    serviceCollection.AddTransient<ViewModels.ShellViewModel>();

                    serviceCollection.AddTransient<Models.ILoad, ViewModels.LoadViewModel>();
                    serviceCollection.AddTransient<Models.ISongs, ViewModels.SongsViewModel>();

                    serviceCollection.AddTransient<Services.Contracts.ISelectFolder, Services.SelectFolderDialog>();
                    serviceCollection.AddTransient<Services.Contracts.IOpenFile, Services.OpenFileDialog>();
                })
                .UseWpfApplicationLifetime()
                .Build();

            await this.host.StartAsync().ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e) => this.DisplayRootViewFor<ViewModels.ShellViewModel>();

        /////// <inheritdoc/>
        ////protected override async void OnExit(object sender, EventArgs e)
        ////{
        ////    await this.host.StopAsync().ConfigureAwait(false);
        ////    base.OnExit(sender, e);
        ////}

        /// <inheritdoc/>
        protected override object GetInstance(Type service, string key) => string.IsNullOrEmpty(key)
            ? this.host.Services.GetRequiredService(service)
            : this.host.Services.GetRequiredService(service, key);

        /// <inheritdoc/>
        protected override IEnumerable<object> GetAllInstances(Type service) => this.host.Services.GetServices(service);

        /////// <inheritdoc/>
        ////protected override void BuildUp(object instance) => throw new InvalidOperationException();

        private readonly struct DummyHost : IHost
        {
            public IServiceProvider Services { get; }

            public void Dispose()
            {
            }

            public Task StartAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

            public Task StopAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        }
    }
}