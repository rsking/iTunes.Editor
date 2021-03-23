// <copyright file="HostBuilderExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace Microsoft.Extensions.Hosting
{
    using ITunes.Editor;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Extends <see cref="IHostBuilder"/>.
    /// </summary>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// Uses the default providers.
        /// </summary>
        /// <param name="builder">The web host builder to configure.</param>
        /// <returns>The host builder.</returns>
        public static IHostBuilder UseDefaultITunes(this IHostBuilder builder)
        {
            if (builder is null)
            {
                throw new System.ArgumentNullException(nameof(builder));
            }

            return builder.ConfigureServices((hostingContext, serviceCollection) =>
            {
                // Lyrics
                serviceCollection
                    .AddWikia()
                    .AddAZ()
                    .AddGenius()
                    .AddChartLyrics()
                    .AddOvh()
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
}
