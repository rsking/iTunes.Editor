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
    using System.ServiceModel.Channels;
    using System.Threading.Tasks;

    /// <summary>
    /// The <see cref="ILyricsProvider"/> for lyrics.wikia.com.
    /// </summary>
    public class WikiaLyricsProvider : ILyricsProvider
    {
        private static readonly IEnumerable<string> PossibleNodes = new[]
                                                               {
                                                                   "//div[@class='lyricbox']",
                                                                   "//div[@id='lyrics']",
                                                                   "//p[@id='songLyricsDiv']"
                                                               };

        private readonly Uri uri = new Uri("http://lyrics.wikia.com/server.php");

        private readonly System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();

        private readonly LyricWikiPortTypeChannel channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="WikiaLyricsProvider"/> class.
        /// </summary>
        public WikiaLyricsProvider()
        {
            IEnumerable<BindingElement> bindingElements = new BindingElement[]
            {
                new GZipMessageEncodingBindingElement(new TextMessageEncodingBindingElement { MessageVersion = MessageVersion.Soap11 }),
                new HttpTransportBindingElement()
            };

            var customBinding = new CustomBinding(bindingElements)
            {
                Name = "LyricWikiBinding",
                Namespace = "http://tempuri.org/bindings",
            };

            var channelFactory = new System.ServiceModel.ChannelFactory<LyricWikiPortTypeChannel>(
                customBinding,
                new System.ServiceModel.EndpointAddress(this.uri));

            this.channel = channelFactory.CreateChannel();
        }

        /// <inheritdoc/>
        public string GetLyrics(SongInformation tagInformation) => this.GetLyricsAsync(tagInformation).Result;

        /// <inheritdoc/>
        public async Task<string> GetLyricsAsync(SongInformation tagInformation)
        {
            var artist = Escape(string.Join("; ", tagInformation.Performers));
            var songTitle = Escape(tagInformation.Title);

            if (await this.channel.checkSongExistsAsync(artist, songTitle).ConfigureAwait(false))
            {
                // get the song
                var lyricResult = await this.channel.getSongResultAsync(artist, songTitle).ConfigureAwait(false);

                if (lyricResult.lyrics == "Not found")
                {
                    return null;
                }

                var webAddress = lyricResult.url;

                var scraped = await this.ScrapeLyrics(webAddress).ConfigureAwait(false);

                return scraped?.Trim();
            }

            return null;
        }

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
                            lyrics.Append((char)int.Parse(code));
                        }

                        break;
                    case "br":
                        lyrics.AppendLine();
                        break;
                }
            }

            return lyrics.ToString();
        }

        private async Task<string> ScrapeLyrics(string address)
        {
            var pageText = await this.client.GetStringAsync(address).ConfigureAwait(false);

            // parse the HTML
            var document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(pageText);

            // select the div
            var nodes = PossibleNodes.Select(document.DocumentNode.SelectNodes).FirstOrDefault(node => node != null);

            return nodes != null ? ScrapeNode(nodes[0]) : string.Empty;
        }
    }
}