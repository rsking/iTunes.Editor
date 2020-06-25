﻿namespace ITunes.Editor.Lyrics.Ovh
{
    using Microsoft.Extensions.Logging;
    using RestSharp;

    public class OvhLyricsProvider : ILyricsProvider
    {
        private static readonly System.Uri Uri = new System.Uri("https://api.lyrics.ovh/v1");

        private readonly ILogger logger;

        private readonly IRestClient client = new RestClient(Uri);

        /// <summary>
        /// Initializes a new instance of the <see cref="OvhLyricsProvider" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="apiKey">The API key.</param>
        public OvhLyricsProvider(ILogger<OvhLyricsProvider> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task<string?> GetLyricsAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken)
        {
            if (tagInformation is null)
            {
                return default;
            }

            this.logger.LogTrace(Properties.Resources.GettingLyrics, tagInformation);
            var request = new RestRequest("{artist}/{title}", Method.GET)
                .AddUrlSegment("artist", string.Join("; ", tagInformation.Performers))
                .AddUrlSegment("title", tagInformation.Title);
            var response = await this.client.ExecuteAsync<GetLyricsResponse>(request, cancellationToken).ConfigureAwait(false);
            return response?.Data?.Lyrics;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "This is done via reflection")]
        private class GetLyricsResponse
        {
            public string? Lyrics { get; set; }
        }
    }
}
