// <copyright file="WpfApplicationLifetime.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The WPF application lifetime.
    /// </summary>
    internal class WpfApplicationLifetime : IHostLifetime, IDisposable
    {
        private readonly System.Threading.ManualResetEvent shutdownBlock = new System.Threading.ManualResetEvent(false);
        private readonly WpfApplicationLifetimeOptions options;
        private readonly IHostEnvironment environment;
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly ILogger logger;
        private System.Threading.CancellationTokenRegistration applicationStartedRegistration;
        private System.Threading.CancellationTokenRegistration applicationStoppingRegistration;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfApplicationLifetime"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="environment">The environment.</param>
        /// <param name="applicationLifetime">The application lifetime.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        public WpfApplicationLifetime(
            Microsoft.Extensions.Options.IOptions<WpfApplicationLifetimeOptions> options,
            IHostEnvironment environment,
            IHostApplicationLifetime applicationLifetime,
            ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            this.environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this.applicationLifetime =
                applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            this.logger = loggerFactory.CreateLogger("Microsoft.Hosting.Lifetime");
        }

        /// <inheritdoc/>
        public async Task WaitForStartAsync(System.Threading.CancellationToken cancellationToken)
        {
            if (System.Windows.Application.Current == null || System.Windows.Application.Current.Dispatcher == null)
            {
                throw new InvalidOperationException($"The {typeof(System.Windows.Application)} is not initialized");
            }

            var tcs = new TaskCompletionSource<bool>();

            if (System.Windows.Application.Current.Dispatcher.CheckAccess())
            {
                AttachToApplicationStartup();
            }
            else
            {
                System.Windows.Application.Current.Dispatcher.Invoke(AttachToApplicationStartup);
            }

            AppDomain.CurrentDomain.ProcessExit += this.OnProcessExit;

            if (!this.options.SuppressStatusMessages)
            {
                this.applicationStartedRegistration = this.applicationLifetime.ApplicationStarted.Register(
                    state => ((WpfApplicationLifetime)state).OnApplicationStarted(),
                    this);
                this.applicationStoppingRegistration = this.applicationLifetime.ApplicationStopping.Register(
                    state => ((WpfApplicationLifetime)state).OnApplicationStopping(),
                    this);
            }

            await tcs.Task.ConfigureAwait(true);

            void AttachToApplicationStartup()
            {
                System.Windows.Application.Current.Startup += (sender, args) => tcs.SetResult(true);
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public Task StopAsync(System.Threading.CancellationToken cancellationToken) => Task.CompletedTask;

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">Set to <see langword="true"/> to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.shutdownBlock.Set();
                this.applicationStartedRegistration.Dispose();
                this.applicationStoppingRegistration.Dispose();
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            this.applicationLifetime.StopApplication();
            this.shutdownBlock.WaitOne();
        }

        private void OnApplicationStarted()
        {
            this.logger.LogInformation("Application started.");
            this.logger.LogInformation("Hosting environment: {envName}", this.environment.EnvironmentName);
            this.logger.LogInformation("Content root path: {contentRoot}", this.environment.ContentRootPath);
        }

        private void OnApplicationStopping() => this.logger.LogInformation("Application is shutting down...");
    }
}
