// -----------------------------------------------------------------------
// <copyright file="ApiSeedsLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ApiSeeds
{
    using RestSharp;

    /// <summary>
    /// Represents an <see cref="ILyricsProvider"/> using the API Seeds Lyrics service.
    /// </summary>
    public class ApiSeedsLyricsProvider : ILyricsProvider
    {
        private static readonly System.Uri Uri = new System.Uri("https://orion.apiseeds.com/api/music/lyric");

        private readonly IRestClient client = new RestClient(Uri);

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiSeedsLyricsProvider" /> class.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        public ApiSeedsLyricsProvider(string apiKey)
        {
            this.client.AddDefaultParameter("apikey", apiKey, ParameterType.QueryString);
        }

        public ApiSeedsLyricsProvider(Microsoft.Extensions.Options.IOptions<ApiSeeds> options)
            : this(options.Value.ApiKey)
        {
        }

        /// <inheritdoc />
        public string? GetLyrics(SongInformation tagInformation)
        {
            if (tagInformation is null)
            {
                return default;
            }

            var request = CreateRequest(tagInformation);
            var response = this.client.Execute<GetLyricsResponse>(request);
            return GetLyricsImpl(request, response?.Data?.Result);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task<string?> GetLyricsAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken)
        {
            if (tagInformation is null)
            {
                return default;
            }

            var request = CreateRequest(tagInformation);
            return GetLyricsImpl(request, response?.Data?.Result);
            var response = await this.client.ExecuteAsync<GetLyricsResponse>(request, cancellationToken).ConfigureAwait(false);
        }

        private static IRestRequest CreateRequest(SongInformation tagInformation)
        {
            var artist = string.Join("; ", tagInformation.Performers);
            var songTitle = tagInformation.Title;

            var request = new RestRequest("{artist}/{track}", Method.GET);
            request.AddUrlSegment("artist", artist);
            request.AddUrlSegment("track", songTitle);

            return request;
        }

        private static string? GetLyricsImpl(IRestRequest request, GetLyricsResult? getLyricResult)
        {
            if (getLyricResult == null)
            {
                return null;
            }

            if (getLyricResult.Artist?.Name != null
                && getLyricResult.Artist.Name.Equals((string)request.Parameters[0].Value, System.StringComparison.InvariantCultureIgnoreCase)
                && getLyricResult.Track?.Name != null
                && getLyricResult.Track.Name.Equals((string)request.Parameters[1].Value, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return getLyricResult.Track.Text;
            }

            System.Console.WriteLine($"\tAPI Seeds - Incorrect Lyrics found, Expected {request.Parameters[0].Value}|{request.Parameters[1].Value}, but got {getLyricResult.Artist?.Name}|{getLyricResult.Track?.Name}");
            return null;
        }

        private class GetLyricsResponse
        {
            public GetLyricsResult? Result { get; set; }
        }

        private class GetLyricsResult
        {
            public Artist? Artist { get; set; }

            public Track? Track { get; set; }

            public Copyright? Copyright { get; set; }
        }

        private class Artist
        {
            public string? Name { get; set; }
        }

        private class Track
        {
            public string? Name { get; set; }

            public string? Text { get; set; }

            public Language? Lang { get; set; }
        }

        private class Language
        {
            public string? Code { get; set; }

            public string? Name { get; set; }
        }

        private class Copyright
        {
            public string? Notice { get; set; }

            public string? Artist { get; set; }

            public string? Text { get; set; }
        }
    }
}
