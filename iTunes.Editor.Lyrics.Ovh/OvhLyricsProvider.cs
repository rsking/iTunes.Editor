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

    private readonly RestClient client = new(
        new RestClientOptions(Uri) { MaxTimeout = 3000 },
        configureSerialization: s => s.UseSystemTextJson(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }));

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
            .AddUrlSegment("artist", tagInformation.GetPerformer())
            .AddUrlSegment("title", tagInformation.Title);

        var response = await this.client
            .ExecuteGetAsync<GetLyricsResponse>(request, cancellationToken)
            .ConfigureAwait(false);
        if (response is { IsSuccessful: true, Data.Error: null })
        {
            return response.Data.Lyrics;
        }

        if (response.ErrorMessage is { Length: > 0 } errorMessage)
        {
            this.logger.LogError("{Message}", errorMessage);
        }
        else if (response.Data?.Error is { Length: > 0 } dataError)
        {
            this.logger.LogError("{Message}", dataError);
        }
        else if (response.ErrorException is { } exception)
        {
            this.logger.LogError(default, exception, "{Message}", exception.Message);
        }

        return default;
    }

    private sealed record GetLyricsResponse(string? Lyrics, string? Error);
}
