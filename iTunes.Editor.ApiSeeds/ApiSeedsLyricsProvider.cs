// -----------------------------------------------------------------------
// <copyright file="ApiSeedsLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ApiSeeds
{
    using Microsoft.Extensions.Logging;
    using RestSharp;

    /// <summary>
    /// Represents an <see cref="ILyricsProvider"/> using the API Seeds Lyrics service.
    /// </summary>
    public class ApiSeedsLyricsProvider : ILyricsProvider
    {
        private static readonly System.Uri Uri = new("https://orion.apiseeds.com/api/music/lyric");

        private readonly ILogger logger;

        private readonly IRestClient client = new RestClient(Uri);

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is done via reflection")]
        private class GetLyricsResponse
        {
            public GetLyricsResult? Result { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is done via reflection")]
        private class GetLyricsResult
        {
            public Artist? Artist { get; set; }

            public Track? Track { get; set; }

            public Copyright? Copyright { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is done via reflection")]
        private class Artist
        {
            public string? Name { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is done via reflection")]
        private class Track
        {
            public string? Name { get; set; }

            public string? Text { get; set; }

            public Language? Lang { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is done via reflection")]
        private class Language
        {
            public string? Code { get; set; }

            public string? Name { get; set; }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is done via reflection")]
        private class Copyright
        {
            public string? Notice { get; set; }

            public string? Artist { get; set; }

            public string? Text { get; set; }
        }
    }
}
