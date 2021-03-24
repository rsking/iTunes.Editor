// <copyright file="AZLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Lyrics.AZLyrics
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Humanizer;

    /// <summary>
    /// The "http://www.azlyrics.com" <see cref="ILyricsProvider"/>.
    /// </summary>
    public sealed class AZLyricsProvider : ILyricsProvider, IDisposable
    {
        private readonly System.Net.Http.HttpClient client = new();

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
                return new string(value.Where(chr => char.IsLetterOrDigit(chr)).ToArray());
            }

            var artist = RemoveNonAlphaNumeric(Escape(string.Join(" & ", tagInformation.Performers))).Transform(To.LowerCase);
            var songTitle = RemoveNonAlphaNumeric(Escape(tagInformation.Title)).Transform(To.LowerCase);

            var webAddress = $"https://www.azlyrics.com/lyrics/{artist}/{songTitle}.html";

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
                        lyrics.Append(childNode.InnerText.Trim('\r', '\n'));
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
            var lyricsNode = document.DocumentNode
                .SelectSingleNode("//div[@class='lyricsh']")?
                .ParentNode?
                .SelectNodes("div")
                .FirstOrDefault(div => div.Attributes.Count == 0 || div.Attributes.All(attribute => attribute.Name != "class"));

            return lyricsNode is null ? null : ScrapeNode(lyricsNode);
        }
    }
}
