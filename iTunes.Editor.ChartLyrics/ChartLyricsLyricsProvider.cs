// -----------------------------------------------------------------------
// <copyright file="ChartLyricsLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ProxyBuilder")]

namespace ITunes.Editor.ChartLyrics;

using Microsoft.Extensions.Logging;

/// <summary>
/// The <see cref="ILyricsProvider"/> for www.chartlyrics.com.
/// </summary>
public sealed class ChartLyricsLyricsProvider : ILyricsProvider, IDisposable
{
    private readonly Uri uri = new("http://api.chartlyrics.com/apiv1.asmx");

    private readonly ILogger logger;

    private readonly apiv1SoapClient channel;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChartLyricsLyricsProvider"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public ChartLyricsLyricsProvider(ILogger<ChartLyricsLyricsProvider> logger)
    {
        this.logger = logger;
        this.channel = new apiv1SoapClient(
            new System.ServiceModel.BasicHttpBinding { Name = "apiv1Soap" },
            new System.ServiceModel.EndpointAddress(this.uri));
    }

    /// <inheritdoc/>
    public async ValueTask<string?> GetLyricsAsync(SongInformation tagInformation, CancellationToken cancellationToken = default)
    {
        if (tagInformation is null)
        {
            return null;
        }

        this.logger.LogTrace(Properties.Resources.GettingLyrics, tagInformation);
        var artist = tagInformation.GetPerformer();
        var songTitle = tagInformation.Title;
        SearchLyricDirectResponse searchLyricDirectResponse;
        try
        {
            searchLyricDirectResponse = await this.channel.SearchLyricDirectAsync(artist, songTitle).ConfigureAwait(false);
        }
        catch (System.ServiceModel.FaultException)
        {
            return null;
        }
        catch (System.ServiceModel.CommunicationException)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        return GetLyricsImpl(this.logger, artist, songTitle, searchLyricDirectResponse?.Body?.SearchLyricDirectResult);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.channel is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }

    private static string? GetLyricsImpl(ILogger logger, string artist, string song, api.chartlyrics.com.GetLyricResult? getLyricResult)
    {
        if (getLyricResult is null || getLyricResult.LyricId <= 0)
        {
            return null;
        }

        if (getLyricResult.LyricArtist.Equals(artist, StringComparison.OrdinalIgnoreCase)
            && getLyricResult.LyricSong.Equals(song, StringComparison.OrdinalIgnoreCase))
        {
            return getLyricResult.Lyric;
        }

        logger.LogWarning(Properties.Resources.IncorrectLyricsFound, $"{artist}|{song}", $"{getLyricResult.LyricArtist}|{getLyricResult.LyricSong}");
        return null;
    }
}
