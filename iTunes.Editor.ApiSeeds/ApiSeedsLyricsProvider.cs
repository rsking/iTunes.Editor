// -----------------------------------------------------------------------
// <copyright file="ApiSeedsLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ApiSeeds;

using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Serializers.Json;

/// <summary>
/// Represents an <see cref="ILyricsProvider"/> using the API Seeds Lyrics service.
/// </summary>
public class ApiSeedsLyricsProvider : ILyricsProvider
{
    private static readonly Uri Uri = new("https://orion.apiseeds.com/api/music/lyric");

    private readonly ILogger logger;

    private readonly RestClient client = new(
        new RestClientOptions(Uri),
        configureSerialization: s => s.UseSystemTextJson(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiSeedsLyricsProvider" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="apiKey">The API key.</param>
    public ApiSeedsLyricsProvider(ILogger<ApiSeedsLyricsProvider> logger, string? apiKey)
    {
        this.logger = logger;
        if (apiKey is not null)
        {
            _ = this.client.AddDefaultParameter("apikey", apiKey, ParameterType.QueryString);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApiSeedsLyricsProvider" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options.</param>
    public ApiSeedsLyricsProvider(ILogger<ApiSeedsLyricsProvider> logger, Microsoft.Extensions.Options.IOptions<ApiSeedsOptions> options)
        : this(logger, options is null ? throw new ArgumentNullException(nameof(options)) : options.Value.ApiKey)
    {
    }

    /// <inheritdoc />
    public async ValueTask<string?> GetLyricsAsync(SongInformation tagInformation, CancellationToken cancellationToken = default)
    {
        if (tagInformation is null)
        {
            return default;
        }

        this.logger.LogTrace(Properties.Resources.GettingLyrics, tagInformation);
        var request = new RestRequest("{artist}/{track}", Method.Get)
            .AddUrlSegment("artist", tagInformation.Performers.ToJoinedString())
            .AddUrlSegment("track", tagInformation.Title);
        var response = await this.client.ExecuteGetAsync<GetLyricsResponse>(request, cancellationToken).ConfigureAwait(false);
        if (!response.IsSuccessful)
        {
            this.logger.LogError(response.ErrorMessage);
            return default;
        }

        return GetLyricsImpl(this.logger, request, response.Data?.Result);

        static string? GetLyricsImpl(ILogger logger, RestRequest request, GetLyricsResult? result)
        {
            if (result is null)
            {
                return null;
            }

            var (artist, track) = GetArtistAndTrackFromRequest(request.Parameters);

            if (result.Artist?.Name is not null
                && CheckName(result.Artist.Name, artist)
                && result.Track?.Name is not null
                && CheckName(result.Track.Name, track))
            {
                return result.Track.Text;
            }

            logger.LogWarning(Properties.Resources.IncorrectLyricsFound, $"{artist}|{track}", $"{result.Artist?.Name}|{result.Track?.Name}");
            return null;

            static (string? Artist, string? Track) GetArtistAndTrackFromRequest(ParametersCollection parameters)
            {
                var enumerator = parameters.GetEnumerator();
                enumerator.MoveNext();
                var artist = enumerator.Current.Value as string;
                enumerator.MoveNext();
                var track = enumerator.Current.Value as string;

                return (artist, track);
            }

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

    private sealed record GetLyricsResponse(GetLyricsResult? Result);

    private sealed record GetLyricsResult(Artist? Artist, Track? Track, Copyright? Copyright);

    private sealed record Artist(string? Name);

    private sealed record Track(string? Name, string? Text, Language? Lang);

    private sealed record Language(string? Code, string? Name);

    private sealed record Copyright(string? Notice, string? Artist, string? Text);
}
