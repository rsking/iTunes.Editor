// <copyright file="GeniusLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Lyrics.Genius
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Humanizer;

    /// <summary>
    /// The "http://genius.com" <see cref="ILyricsProvider"/>.
    /// </summary>
    public sealed class GeniusLyricsProvider : ILyricsProvider, IDisposable
    {
        private readonly System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

        /// <inheritdoc/>
        public void Dispose() => this.client.Dispose();

        /// <inheritdoc/>
        public async Task<string?> GetLyricsAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken)
        {
            if (tagInformation is null)
            {
                return default;
            }

            static string Escape(string unescaped)
            {
                return new System.Xml.Linq.XText(unescaped).ToString();
            }

            static string RemoveNonAlphaNumeric(string value)
            {
                return new string(value.Where(chr => char.IsLetterOrDigit(chr) || char.IsWhiteSpace(chr)).ToArray());
            }

            var artist = RemoveNonAlphaNumeric(Escape(string.Join(" & ", tagInformation.Performers))).Transform(To.LowerCase).Replace(' ', '-');
            var songTitle = RemoveNonAlphaNumeric(Escape(tagInformation.Title)).Transform(To.LowerCase).Replace(' ', '-');

            var query = $"{artist}-{songTitle}".Transform(To.SentenceCase);

            var webAddress = $"https://genius.com/{query}-lyrics";

            var scraped = await this.ScrapeLyricsAsync(webAddress, cancellationToken).ConfigureAwait(false);

            return scraped?.Trim();
        }

        private static string ScrapeNode(HtmlAgilityPack.HtmlNode node)
        {
            var lyrics = new System.Text.StringBuilder();

            foreach (var childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "i":
                    case "b":
                        lyrics.Append(ScrapeNode(childNode));
                        break;
                    case "#text":
                        // scrape this
                        lyrics.Append(childNode.InnerText.Trim('\n'));
                        break;
                    case "br":
                        lyrics.AppendLine();
                        break;
                }
            }

            return lyrics.ToString();
        }

        private async Task<string?> ScrapeLyricsAsync(string address, System.Threading.CancellationToken cancellationToken)
        {
            string pageText;
            try
            {
                pageText = await this.client.GetStringAsync(address).ConfigureAwait(false);
            }
            catch (System.Net.Http.HttpRequestException)
            {
                return null;
            }

            cancellationToken.ThrowIfCancellationRequested();

            // parse the HTML
            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(pageText);

            // select the div
            var paragraphNode = document.DocumentNode
                .SelectNodes("//div[@class='lyrics']")
                .FirstOrDefault()?
                .SelectSingleNode("p");

            return paragraphNode is null ? null : ScrapeNode(paragraphNode);
        }
    }
}
