// -----------------------------------------------------------------------
// <copyright file="PurgoMalumExplicitLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.PurgoMalum;

using RestSharp;

/// <summary>
/// The <see cref="IExplicitLyricsProvider"/> using Purgo Malum.
/// </summary>
public class PurgoMalumExplicitLyricsProvider : IExplicitLyricsProvider
{
    private readonly IRestClient client = new RestClient(new Uri("https://www.purgomalum.com/service/"));

    /// <inheritdoc/>
    public async ValueTask<bool?> IsExplicitAsync(string lyrics, CancellationToken cancellationToken) => ParseResponse(await this.client.ExecuteAsync<string?>(GetRequest(lyrics), cancellationToken).ConfigureAwait(false));

    private static RestRequest GetRequest(string lyrics) => new RestRequest("containsprofanity", Method.Get)
        .AddHeader("Accept", "text/html, application/xhtml+xml, application/xml, text/plain")
        .AddParameter("text", lyrics);

    private static bool? ParseResponse(RestResponse<string?> response) => bool.TryParse(response.Content, out var boolValue) ? boolValue : null;
}
