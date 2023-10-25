// -----------------------------------------------------------------------
// <copyright file="HostBuilderExtensions.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Extensions.Hosting;

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
    public static IHostBuilder UseDefaultITunes(this IHostBuilder builder) => builder is null
        ? throw new ArgumentNullException(nameof(builder))
        : builder.ConfigureServices((hostingContext, serviceCollection) =>
        {
            // Lyrics
            _ = serviceCollection
                .AddAZ()
                .AddGenius()
                .AddChartLyrics()
                .AddOvh()
                .AddPurgoMalum();

            // Composers
            _ = serviceCollection
                .AddApraAmcos();

            // song providers
            _ = serviceCollection
                .AddFolder()
                .AddIPod()
                .AddPList()
                .AddITunes()
                .AddShell();

            // tag provider
            _ = serviceCollection
                .AddTagLib()
                .AddMediaInfo();

            // add services
            _ = serviceCollection
                .AddTransient<IUpdateComposerService, UpdateComposerService>()
                .AddTransient<IUpdateLyricsService, UpdateLyricsService>()
                .AddTransient<IUpdateTempoService, UpdateTempoService>();
        });
}
