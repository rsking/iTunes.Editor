// <copyright file="OvhLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Lyrics.Ovh;

using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Serializers.Json;

/// <summary>
/// The <see cref="ILyricsProvider"/> for OVH.
/// </summary>
public class OvhLyricsProvider : ILyricsProvider
{
    private static readonly Uri Uri = new("https://api.lyrics.ovh/v1");

    private readonly ILogger logger;

    private readonly RestClient client = new RestClient(new RestClientOptions(Uri) { MaxTimeout = 3000 })
        .UseSystemTextJson(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    /// <summary>
    /// Initializes a new instance of the <see cref="OvhLyricsProvider" /> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public OvhLyricsProvider(ILogger<OvhLyricsProvider> logger) => this.logger = logger;

    /// <inheritdoc />
    public async ValueTask<string?> GetLyricsAsync(SongInformation tagInformation, CancellationToken cancellationToken = default)
    {
        if (tagInformation is null)
        {
            return default;
        }

        this.logger.LogTrace(Properties.Resources.GettingLyrics, tagInformation);
        var request = new RestRequest("{artist}/{title}", Method.Get) { Timeout = 3000 }
            .AddUrlSegment("artist", tagInformation.Performers.ToJoinedString())
            .AddUrlSegment("title", tagInformation.Title);

        var response = await this.client
            .ExecuteGetAsync<GetLyricsResponse>(request, cancellationToken)
            .ConfigureAwait(false);
        if (response is { IsSuccessful: true, Data.Error: null })
        {
            return response.Data.Lyrics;
        }

        if (!string.IsNullOrEmpty(response.ErrorMessage))
        {
            this.logger.LogError("{Message}", response.ErrorMessage);
        }

        return default;
    }

    private sealed record GetLyricsResponse(string? Lyrics, string? Error);
}
