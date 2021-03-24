// -----------------------------------------------------------------------
// <copyright file="ApraAmcosComposerProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The <see cref="IComposerProvider"/> for APRA AMCOS.
    /// </summary>
    public sealed class ApraAmcosComposerProvider : IComposerProvider, IDisposable
    {
        private readonly Uri uri = new("https://apraamcos.com.au/search");

        private readonly HttpClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApraAmcosComposerProvider"/> class.
        /// </summary>
        public ApraAmcosComposerProvider()
        {
            var clientHandler = new HttpClientHandler();
            if (clientHandler.SupportsAutomaticDecompression)
            {
                clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }

            this.client = new HttpClient(clientHandler, disposeHandler: true);
            this.client.DefaultRequestHeaders.Pragma.Add(new NameValueHeaderValue("no-cache"));
            this.client.DefaultRequestHeaders.ExpectContinue = false;
            this.client.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("text/html"));
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
            this.client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
            this.client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-AU"));
            this.client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en", 0.5));
            this.client.DefaultRequestHeaders.Referrer = this.uri;
            this.client.DefaultRequestHeaders.Host = this.uri.Host;
            this.client.DefaultRequestHeaders.Connection.Add("Keep-Alive");
        }

        /// <inheritdoc/>
        public void Dispose() => this.client.Dispose();

        /// <inheritdoc />
        public async System.Collections.Generic.IAsyncEnumerable<Name> GetComposersAsync(
            SongInformation tagInformation,
            [System.Runtime.CompilerServices.EnumeratorCancellation] System.Threading.CancellationToken cancellationToken)
        {
            if (tagInformation is null)
            {
                yield break;
            }

            foreach (var name in await this.GetNamesAsync(tagInformation, cancellationToken).ConfigureAwait(false))
            {
                yield return name;
            }
        }

        private async Task<System.Collections.Generic.IEnumerable<Name>> GetNamesAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken = default)
        {
            var title = tagInformation.Title;
            var writer = string.Empty;
            var performer = tagInformation.Performers.FirstOrDefault() ?? string.Empty;

            HttpResponseMessage result;

            var query = $"searchtype=works&keywords={System.Web.HttpUtility.UrlEncode(title)}&x=20&y=40";
            var content = $"keywords={System.Web.HttpUtility.UrlEncode(title)}&writer={System.Web.HttpUtility.UrlEncode(writer)}&performer={System.Web.HttpUtility.UrlEncode(performer)}&searchtype=works";
            using (var stringContent = new StringContent(content, Encoding.UTF8, "application/x-www-form-urlencoded"))
            {
                var uriWithQuery = new Uri(this.uri, $"?{query}");
                result = await this.client.PostAsync(uriWithQuery, stringContent, cancellationToken).ConfigureAwait(false);
            }

            var pageText = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(pageText);
            var span = document.DocumentNode
                .SelectSingleNode("//div[@class='searchresultcontainer']")?
                .SelectSingleNode("//table")?
                .Descendants("tr").Skip(1).First()?
                .Descendants("td").Last()?
                .Descendants("span").First();

            return span?.InnerText.Contains("Not available") == false
                ? span.InnerText.Split('/').Select(Name.FromInversedName).OrderBy(name => name.Last).ThenBy(name => name.First)
                : Enumerable.Empty<Name>();
        }
    }
}
