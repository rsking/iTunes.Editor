// -----------------------------------------------------------------------
// <copyright file="HappiDevLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.HappiDev
{
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using RestSharp;
    using RestSharp.Serializers.SystemTextJson;

    /// <summary>
    /// Represents an <see cref="ILyricsProvider"/> using the happi.dev Lyrics service.
    /// </summary>
    public class HappiDevLyricsProvider : ILyricsProvider
    {
        private static readonly System.Uri Uri = new("https://api.happi.dev/v1/music/");

        private readonly ILogger logger;

        private readonly System.Net.Http.HttpClient httpClient;

        private readonly IRestClient restClient = new RestClient(Uri)
            .UseSystemTextJson(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        /// <summary>
        /// Initializes a new instance of the <see cref="HappiDevLyricsProvider" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        /// <param name="options">The options.</param>
        public HappiDevLyricsProvider(
            ILogger<HappiDevLyricsProvider> logger,
            System.Net.Http.IHttpClientFactory httpClientFactory,
            Microsoft.Extensions.Options.IOptions<HappiDevOptions> options)
            : this(logger, httpClientFactory.CreateClient(nameof(HappiDevLyricsProvider)), options)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HappiDevLyricsProvider" /> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="apiKey">The API key.</param>
        public HappiDevLyricsProvider(
            ILogger<HappiDevLyricsProvider> logger,
            System.Net.Http.HttpClient httpClient,
            string? apiKey)
        {
            this.logger = logger;
            this.httpClient = httpClient;
            if (apiKey is not null)
            {
                this.restClient.AddDefaultHeader("x-happi-key", apiKey);
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
            System.Net.Http.HttpClient httpClient,
            Microsoft.Extensions.Options.IOptions<HappiDevOptions> options)
            : this(logger, httpClient, options is null ? throw new System.ArgumentNullException(nameof(options)) : options.Value.ApiKey)
        {
        }

        /// <inheritdoc/>
        public async Task<string?> GetLyricsAsync(SongInformation tagInformation, CancellationToken cancellationToken = default)
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
            if (result is not null && result.Success && result.Length == 1)
            {
                return result.Result.Lyrics;
            }

            return default;

            async Task<string?> GetLyricsAddressAsync(SongInformation tagInformation, CancellationToken cancellationToken)
            {
                var request = new RestRequest();
                var title = tagInformation.Title;
                var artist = tagInformation.Performers.ToJoinedString();
                request.AddQueryParameter("q", $"{artist} {title}");
                request.AddQueryParameter("lyrics", "1");
                request.AddQueryParameter("type", "track");
                var response = await this.restClient.ExecuteGetAsync<Track>(request, cancellationToken).ConfigureAwait(false);
                if (!response.IsSuccessful)
                {
                    this.logger.LogError(response.ErrorMessage);
                    return default;
                }

                if (!response.Data.Success || response.Data.Length == 0)
                {
                    return default;
                }

                return GetLyricsImpl(
                    this.logger,
                    artist,
                    title,
                    response.Data.Result[0]);

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
                        return string.Equals(first, second, System.StringComparison.InvariantCultureIgnoreCase)
                            || string.Equals(Sanitise(first), Sanitise(second), System.StringComparison.InvariantCultureIgnoreCase);

                        static string? Sanitise(string? input)
                        {
                            if (input is null)
                            {
                                return input;
                            }

                            return input.Replace('-', ' ');
                        }
                    }
                }
            }
        }

        private record Return<T>(bool Success, int Length, T Result);

        private record Track(bool Success, int Length, TrackResult[] Result) : Return<TrackResult[]>(Success, Length, Result);

        private record TrackResult(
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

        private record Lyrics(bool Success, int Length, LyricsResult Result) : Return<LyricsResult>(Success, Length, Result);

        private record LyricsResult(
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
}
