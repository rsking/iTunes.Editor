// -----------------------------------------------------------------------
// <copyright file="HappiDevLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.HappiDev;

using System.Net.Http.Json;

using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Serializers.Json;

/// <summary>
/// Represents an <see cref="ILyricsProvider"/> using the happi.dev Lyrics service.
/// </summary>
public class HappiDevLyricsProvider : ILyricsProvider
{
    private static readonly Uri Uri = new("https://api.happi.dev/v1/music/");

    private readonly ILogger logger;

    private readonly HttpClient httpClient;

    private readonly RestClient restClient = new(
        new RestClientOptions(Uri),
        configureSerialization: s => s.UseSystemTextJson(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

    /// <summary>
    /// Initializes a new instance of the <see cref="HappiDevLyricsProvider" /> class.
    /// </summary>
    /// <param name="httpClientFactory">The HTTP client factory.</param>
    /// <param name="options">The options.</param>
    /// <param name="logger">The logger.</param>
    public HappiDevLyricsProvider(
        IHttpClientFactory httpClientFactory,
        Microsoft.Extensions.Options.IOptions<HappiDevOptions> options,
        ILogger<HappiDevLyricsProvider> logger)
        : this(logger, httpClientFactory.CreateClient(nameof(HappiDevLyricsProvider)), options)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HappiDevLyricsProvider" /> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="apiKey">The API key.</param>
    /// <param name="logger">The logger.</param>
    public HappiDevLyricsProvider(
        HttpClient httpClient,
        string? apiKey,
        ILogger<HappiDevLyricsProvider> logger)
    {
        this.logger = logger;
        this.httpClient = httpClient;
        if (apiKey is not null)
        {
            _ = this.restClient.AddDefaultHeader("x-happi-key", apiKey);
            this.httpClient.DefaultRequestHeaders.Add("x-happi-key", apiKey);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HappiDevLyricsProvider" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="options">The options.</param>
    public HappiDevLyricsProvider(
        ILogger<HappiDevLyricsProvider> logger,
        HttpClient httpClient,
        Microsoft.Extensions.Options.IOptions<HappiDevOptions> options)
        : this(httpClient, options is null ? throw new ArgumentNullException(nameof(options)) : options.Value.ApiKey, logger)
    {
    }

    /// <inheritdoc/>
    public async ValueTask<string?> GetLyricsAsync(SongInformation tagInformation, CancellationToken cancellationToken = default)
    {
        var lyricsAddress = await GetLyricsAddressAsync(tagInformation, cancellationToken)
            .ConfigureAwait(false);
        if (lyricsAddress is null)
        {
            return default;
        }

        var result = await this.httpClient
            .GetFromJsonAsync<Lyrics>(lyricsAddress, cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return result switch
        {
            not null when result.Success && result.Length == 1 => result.Result.Lyrics,
            _ => default,
        };

        async Task<string?> GetLyricsAddressAsync(SongInformation tagInformation, CancellationToken cancellationToken)
        {
            var title = tagInformation.Title;
            var artist = tagInformation.GetPerformer();
            var request = new RestRequest()
                .AddQueryParameter("q", $"{artist} {title}")
                .AddQueryParameter("lyrics", "1")
                .AddQueryParameter("type", "track");
            return await this.restClient.ExecuteGetAsync<Track>(request, cancellationToken).ConfigureAwait(false) switch
            {
                { IsSuccessful: false } response => LogError(response.ErrorMessage),
                { Data: { Success: true, Length: not 0 } } response => GetLyricsImpl(this.logger, artist, title, response.Data.Result[0]),
                _ => default,
            };

            string? LogError(string? errorMessage)
            {
                this.logger.LogError(errorMessage);
                return default;
            }

            static string? GetLyricsImpl(
                ILogger logger,
                string artist,
                string title,
                TrackResult? result)
            {
                if (result is null)
                {
                    return null;
                }

                if (result.Artist is not null
                    && CheckName(result.Artist, artist)
                    && result.Track is not null
                    && CheckName(result.Track, title))
                {
                    return result.Api_Lyrics;
                }

                logger.LogWarning(Properties.Resources.IncorrectLyricsFound, $"{artist}|{title}", $"{result.Artist}|{result.Track}");
                return null;

                static bool CheckName(string? first, string? second)
                {
                    return string.Equals(first, second, StringComparison.OrdinalIgnoreCase)
                        || string.Equals(Sanitise(first), Sanitise(second), StringComparison.OrdinalIgnoreCase);

                    static string? Sanitise(string? input)
                    {
                        return input switch
                        {
                            null => input,
                            _ => input.Replace('-', ' '),
                        };
                    }
                }
            }
        }
    }

    private record class Return<T>(bool Success, int Length, T Result);

    private sealed record class Track(bool Success, int Length, TrackResult[] Result) : Return<TrackResult[]>(Success, Length, Result);

    private sealed record class TrackResult(
        string Track,
        int Id_Track,
        string Artist,
        bool HasLyrics,
        int Id_Artist,
        string Album,
        int Bpm,
        int Id_Album,
        string Cover,
        string Lang,
        string Api_Artist,
        string Api_Albums,
        string Api_Album,
        string Api_Tracks,
        string Api_Track,
        string Api_Lyrics);

    private sealed record class Lyrics(bool Success, int Length, LyricsResult Result) : Return<LyricsResult>(Success, Length, Result);

    private sealed record class LyricsResult(
        string Artist,
        int Id_Artist,
        string Track,
        int Id_Track,
        string Album,
        string Lyrics,
        string Api_Artist,
        string Api_Albums,
        string Api_Album,
        string Api_Tracks,
        string Api_Track,
        string Api_Lyrics,
        string Lang,
        string Copyright_Label,
        string Copyright_Notice,
        string Copyright_Text);
}
