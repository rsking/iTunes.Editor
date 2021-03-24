﻿// -----------------------------------------------------------------------
// <copyright file="ApiSeedsLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ApiSeeds
{
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using RestSharp.Serializers.SystemTextJson;

    /// <summary>
    /// Represents an <see cref="ILyricsProvider"/> using the API Seeds Lyrics service.
    /// </summary>
    public class ApiSeedsLyricsProvider : ILyricsProvider
    {
        private static readonly System.Uri Uri = new("https://orion.apiseeds.com/api/music/lyric");

        private readonly ILogger logger;

        private readonly IRestClient client = new RestClient(Uri).UseSystemTextJson();

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
                this.client.AddDefaultParameter("apikey", apiKey, ParameterType.QueryString);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiSeedsLyricsProvider" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="options">The options.</param>
        public ApiSeedsLyricsProvider(ILogger<ApiSeedsLyricsProvider> logger, Microsoft.Extensions.Options.IOptions<ApiSeedsOptions> options)
            : this(logger, options is null ? throw new System.ArgumentNullException(nameof(options)) : options.Value.ApiKey)
        {
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task<string?> GetLyricsAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken = default)
        {
            if (tagInformation is null)
            {
                return default;
            }

            this.logger.LogTrace(Properties.Resources.GettingLyrics, tagInformation);
            var request = new RestRequest("{artist}/{track}", Method.GET)
                .AddUrlSegment("artist", string.Join("; ", tagInformation.Performers))
                .AddUrlSegment("track", tagInformation.Title);
            var response = await this.client.ExecuteAsync<GetLyricsResponse>(request, cancellationToken).ConfigureAwait(false);
            return GetLyricsImpl(this.logger, request, response?.Data?.Result);
        }

        private static string? GetLyricsImpl(ILogger logger, IRestRequest request, GetLyricsResult? getLyricResult)
        {
            if (getLyricResult is null)
            {
                return null;
            }

            if (getLyricResult.Artist?.Name is not null
                && getLyricResult.Artist.Name.Equals((string?)request.Parameters[0].Value, System.StringComparison.InvariantCultureIgnoreCase)
                && getLyricResult.Track?.Name is not null
                && getLyricResult.Track.Name.Equals((string?)request.Parameters[1].Value, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return getLyricResult.Track.Text;
            }

            logger.LogWarning(Properties.Resources.IncorrectLyricsFound, $"{request.Parameters[0].Value}|{request.Parameters[1].Value}", $"{getLyricResult.Artist?.Name}|{getLyricResult.Track?.Name}");
            return null;
        }

        private record GetLyricsResponse(GetLyricsResult? Result);

        private record GetLyricsResult(Artist? Artist, Track? Track, Copyright? Copyright);

        private record Artist(string? Name);

        private record Track(string? Name, string? Text, Language? Lang);

        private record Language(string? Code, string? Name);

        private record Copyright(string? Notice, string? Artist, string? Text);
    }
}
