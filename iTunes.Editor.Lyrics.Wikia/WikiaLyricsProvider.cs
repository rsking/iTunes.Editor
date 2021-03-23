// -----------------------------------------------------------------------
// <copyright file="WikiaLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.Lyrics.Wikia
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The <see cref="ILyricsProvider"/> for lyrics.fandom.com.
    /// </summary>
    public sealed class WikiaLyricsProvider : ILyricsProvider, IDisposable
    {
        private static readonly IEnumerable<string> PossibleNodes = new[]
                                                               {
                                                                   "//div[@class='lyricbox']",
                                                                   "//div[@id='lyrics']",
                                                                   "//p[@id='songLyricsDiv']",
                                                               };

        private readonly System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

        /// <inheritdoc/>
        public async Task<string?> GetLyricsAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken)
        {
            if (tagInformation is null)
            {
                return default;
            }

            var artist = Escape(string.Join(" & ", tagInformation.Performers)).Replace(' ', '_');
            var songTitle = Escape(tagInformation.Title).Replace(' ', '_');

            var webAddress = $"https://lyrics.fandom.com/wiki/{artist}:{songTitle}";

            var scraped = await this.ScrapeLyricsAsync(webAddress, cancellationToken).ConfigureAwait(false);

            return scraped?.Trim();
        }

        /// <inheritdoc/>
        public void Dispose() => this.client.Dispose();

        private static string Escape(string unescaped) => new System.Xml.Linq.XText(unescaped).ToString();

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
                        var innerText = childNode.InnerText;
                        foreach (var letter in innerText.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            var index = letter.IndexOf("&#", StringComparison.OrdinalIgnoreCase);
                            if (index < 0)
                            {
                                continue;
                            }

                            if (index > 0)
                            {
                                // see what's at the start
                                lyrics.Append(letter, 0, index);
                            }

                            index += 2;
                            var code = letter.Substring(index);

                            // get the value
                            lyrics.Append((char)int.Parse(code, System.Globalization.CultureInfo.InvariantCulture));
                        }

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
            var nodes = PossibleNodes.Select(document.DocumentNode.SelectNodes).FirstOrDefault(node => node is not null);

            return nodes is null ? null : ScrapeNode(nodes[0]);
        }
    }
}