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
    public class ApraAmcosComposerProvider : IComposerProvider
    {
        private readonly Uri uri = new Uri("http://apraamcos.com.au/search");

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

            this.client = new HttpClient(clientHandler, true);
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

        /// <inheritdoc />
        public System.Collections.Generic.IEnumerable<Name> GetComposers(SongInformation tagInformation)
        {
            return this.GetComposersAsync(tagInformation).Result;
        }

        /// <inheritdoc />
        public async Task<System.Collections.Generic.IEnumerable<Name>> GetComposersAsync(SongInformation tagInformation)
        {
            var title = tagInformation.Title;
            var writer = string.Empty;
            var performer = tagInformation.Performers.FirstOrDefault() ?? string.Empty;

            var stringContent = $"keywords={title.Replace(' ', '+')}&writer={writer.Replace(' ', '+')}&performer={performer.Replace(' ', '+')}&searchtype=works";
            var result = await this.client.PostAsync(this.uri, new StringContent(stringContent, Encoding.UTF8, "application/x-www-form-urlencoded")).ConfigureAwait(false);
            var pageText = await result.Content.ReadAsStringAsync().ConfigureAwait(false);

            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(pageText);
            var div = document.DocumentNode.SelectSingleNode("//div[@class='searchresultcontainer']");
            if (div == null)
            {
                return null;
            }

            var table = div.SelectSingleNode("//table");
            if (table == null)
            {
                return null;
            }

            var row = table.Descendants("tr").Skip(1).First();
            if (row == null)
            {
                return null;
            }

            var td = row.Descendants("td").Last();
            var span = td.Descendants("span").First();
            return span.InnerText.Contains("Not available")
                ? null
                : span.InnerText.Split('/').Select(Name.FromInversedName).OrderBy(name => name.Last).ToArray();
        }
    }
}