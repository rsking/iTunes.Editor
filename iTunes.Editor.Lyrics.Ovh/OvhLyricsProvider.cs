// <copyright file="OvhLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Lyrics.Ovh
{
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using RestSharp.Serializers.SystemTextJson;

    /// <summary>
    /// The <see cref="ILyricsProvider"/> for OVH.
    /// </summary>
    public class OvhLyricsProvider : ILyricsProvider
    {
        private static readonly System.Uri Uri = new("https://api.lyrics.ovh/v1");

        private readonly ILogger logger;

        private readonly IRestClient client = new RestClient(Uri) { Timeout = 3000, ReadWriteTimeout = 3000 }
            .UseSystemTextJson(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        /// <summary>
        /// Initializes a new instance of the <see cref="OvhLyricsProvider" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public OvhLyricsProvider(ILogger<OvhLyricsProvider> logger) => this.logger = logger;

        /// <inheritdoc />
        public async System.Threading.Tasks.Task<string?> GetLyricsAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken = default)
        {
            if (tagInformation is null)
            {
                return default;
            }

            this.logger.LogTrace(Properties.Resources.GettingLyrics, tagInformation);
            var request = new RestRequest("{artist}/{title}", Method.GET) { Timeout = 3000, ReadWriteTimeout = 3000 }
                .AddUrlSegment("artist", string.Join("; ", tagInformation.Performers))
                .AddUrlSegment("title", tagInformation.Title);
            var response = await this.client.ExecuteGetAsync<GetLyricsResponse>(request, cancellationToken).ConfigureAwait(false);
            if (!response.IsSuccessful)
            {
                this.logger.LogError(response.ErrorMessage);
                return default;
            }

            return response.Data.Lyrics;
        }

        private record GetLyricsResponse(string? Lyrics);
    }
}
