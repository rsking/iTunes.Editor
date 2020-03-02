// -----------------------------------------------------------------------
// <copyright file="PurgoMalumExplicitLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PurgoMalum
{
    using System.Threading.Tasks;
    using RestSharp;

    /// <summary>
    /// The <see cref="IExplicitLyricsProvider"/> using Purgo Malum.
    /// </summary>
    public class PurgoMalumExplicitLyricsProvider : IExplicitLyricsProvider
    {
        private readonly IRestClient client = new RestClient("https://www.purgomalum.com/service/");

        /// <inheritdoc/>
        public async Task<bool?> IsExplicitAsync(string lyrics, System.Threading.CancellationToken cancellationToken) => ParseResponse(await this.client.ExecuteAsync<string?>(GetRequest(lyrics), cancellationToken).ConfigureAwait(false));

        private static IRestRequest GetRequest(string lyrics) => new RestRequest("containsprofanity", Method.GET)
            .AddHeader("Accept", "text/html, application/xhtml+xml, application/xml, text/plain")
            .AddParameter("text", lyrics);

        private static bool? ParseResponse(IRestResponse<string?> response) => bool.TryParse(response.Content, out var boolValue) ? boolValue : (bool?)null;
    }
}
