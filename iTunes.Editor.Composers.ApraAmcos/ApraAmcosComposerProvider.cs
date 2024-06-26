﻿// <copyright file="ApraAmcosComposerProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Composers.ApraAmcos;

using RestSharp;
using RestSharp.Serializers.Json;

/// <summary>
/// The <see cref="IComposerProvider"/> for APRA AMCOS.
/// </summary>
public sealed class ApraAmcosComposerProvider : IComposerProvider
{
    private readonly RestClient client = new(
        new RestClientOptions("https://www.apraamcos.com.au/api/")
        {
            EncodeQuery = (value, encoding) => System.Web.HttpUtility.UrlEncode(value.ToLowerInvariant(), encoding),
            UserAgent = typeof(ApraAmcosComposerProvider).FullName,
        },
        configureSerialization: s => s.UseSystemTextJson(new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        }));

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
            var performer = tagInformation.GetPerformer();

            var request = new RestRequest("/works");
            request.AddQueryParameter("works", bool.TrueString);
            request.AddQueryParameter(nameof(title), Sanitize(title));
            request.AddQueryParameter(nameof(writer), Sanitize(writer));
            request.AddQueryParameter(nameof(performer), Sanitize(performer));

            if (await this.client.ExecuteGetAsync<SearchResult>(request, cancellationToken).ConfigureAwait(false) is { IsSuccessful: true, Data.Success: true } response)
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
                    foreach (var name in work.Writers.Split('/'))
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
