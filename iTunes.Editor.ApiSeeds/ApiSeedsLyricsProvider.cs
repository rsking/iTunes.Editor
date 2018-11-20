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

        /// <inheritdoc />
        public string GetLyrics(SongInformation tagInformation)
        {
            var request = CreateRequest(tagInformation);
            var response = this.client.Execute<GetLyricsResponse>(request);
            return GetLyricsImpl(request, response?.Data?.Result);
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task<string> GetLyricsAsync(SongInformation tagInformation)
        {
            var request = CreateRequest(tagInformation);
            var response = await this.client.ExecuteTaskAsync<GetLyricsResponse>(request);
            return GetLyricsImpl(request, response?.Data?.Result);
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

        private static string GetLyricsImpl(IRestRequest request, GetLyricsResult getLyricResult)
        {
            if (getLyricResult == null)
            {
                return null;
            }

            if (getLyricResult.Artist.Name != (string)request.Parameters[0].Value || getLyricResult.Track.Name != (string)request.Parameters[1].Value)
            {
                var returnAnyway = false;
                System.Console.WriteLine($"\tIncorrect Lyrics found, Expected {request.Parameters[0].Value}|{request.Parameters[1].Value}, but got {getLyricResult.Artist?.Name}|{getLyricResult.Track?.Name}");
                if (!returnAnyway)
                {
                    return null;
                }
            }

            return getLyricResult.Track.Text;
        }

        private class GetLyricsResponse
        {
            public GetLyricsResult Result { get; set; }
        }

        private class GetLyricsResult
        {
            public Artist Artist { get; set; }

            public Track Track { get; set; }

            public Copyright Copyright { get; set; }
        }

        private class Artist
        {
            public string Name { get; set; }
        }

        private class Track
        {
            public string Name { get; set; }

            public string Text { get; set; }

            public Language Lang { get; set; }
        }

        private class Language
        {
            public string Code { get; set; }

            public string Name { get; set; }
        }

        private class Copyright
        {
            public string Notice { get; set; }

            public string Artist { get; set; }

            public string Text { get; set; }
        }
    }
}
