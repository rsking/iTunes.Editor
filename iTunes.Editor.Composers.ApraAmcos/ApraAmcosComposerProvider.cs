// <copyright file="ApraAmcosComposerProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Composers.ApraAmcos;

using RestSharp;
using RestSharp.Serializers.SystemTextJson;

/// <summary>
/// The <see cref="IComposerProvider"/> for APRA AMCOS.
/// </summary>
public sealed class ApraAmcosComposerProvider : IComposerProvider
{
    private readonly IRestClient client = new RestClient("https://www.apraamcos.com.au/api/")
        .UseSystemTextJson(new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
        .UseQueryEncoder((value, encoding) => System.Web.HttpUtility.UrlEncode(value.ToLowerInvariant(), encoding));

    /// <inheritdoc />
    public IAsyncEnumerable<Name> GetComposersAsync(
        SongInformation tagInformation,
        CancellationToken cancellationToken)
    {
        return GetNamesAsync(tagInformation, cancellationToken)
            .OrderBy(name => name.Last)
            .ThenBy(name => name.First);

        async IAsyncEnumerable<Name> GetNamesAsync(SongInformation tagInformation, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (tagInformation is null)
            {
                yield break;
            }

            var title = tagInformation.Title;
            var writer = string.Empty;
            var performer = tagInformation.Performers.FirstOrDefault() ?? string.Empty;

            var request = new RestRequest("/works");
            request.AddQueryParameter("works", bool.TrueString);
            request.AddQueryParameter(nameof(title), Sanitize(title));
            request.AddQueryParameter(nameof(writer), Sanitize(writer));
            request.AddQueryParameter(nameof(performer), Sanitize(performer));

            var response = await this.client.ExecuteGetAsync<SearchResult>(request, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessful && response.Data.Success)
            {
                var work = response.Data.Result.First();

                if (work.WorkWriters is not null)
                {
                    foreach (var workWriter in work.WorkWriters)
                    {
                        yield return Name.FromInversedName(workWriter.WriterName);
                    }
                }
                else if (work.Writers is not null)
                {
                    var writers = work.Writers.Split('/');

                    foreach (var name in writers)
                    {
                        yield return Name.FromInversedName(name);
                    }
                }
            }

            static string Sanitize(string input)
            {
                return string.IsNullOrWhiteSpace(input)
                    ? input
#if NETSTANDARD2_1_OR_GREATER
                    : input.Replace("&", "and", StringComparison.Ordinal);
#else
                    : input.Replace("&", "and");
#endif
            }
        }
    }

    private sealed record SearchResult(bool Success, IReadOnlyCollection<Work> Result);

    private sealed record Work(string Title, string? Writers, IReadOnlyCollection<Writer>? WorkWriters);

    private sealed record Writer(string Contract, string WriterName);
}
