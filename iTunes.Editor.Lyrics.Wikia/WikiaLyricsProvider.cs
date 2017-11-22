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
    using System.Xml.Linq;
    using SoapHttpClient;
    using SoapHttpClient.Enums;
    using SoapHttpClient.Extensions;

    /// <summary>
    /// The <see cref="ILyricsProvider"/> for lyrics.wikia.com
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

        private readonly SoapClient soapClient = new SoapClient();

        /// <inheritdoc/>
        public string GetLyrics(SongInformation tagInformation)
        {
            return this.GetLyricsAsync(tagInformation).Result;
        }

        /// <inheritdoc/>
        public async Task<string> GetLyricsAsync(SongInformation tagInformation)
        {
            object artist = string.Join("; ", tagInformation.Performers);
            object songTitle = tagInformation.Title;

            var document = await this.GetResult("checkSongExists", Tuple.Create("artist", artist), Tuple.Create("song", songTitle)).ConfigureAwait(false);

            if (!document.Descendants().Where(_ => _.Name == "return").Select(_ => int.Parse(_.Value) != 0).First())
            {
                return null;
            }

            document = await this.GetResult("getSongResult", Tuple.Create("artist", artist), Tuple.Create("song", songTitle)).ConfigureAwait(false);

            var songResult = document.Descendants().FirstOrDefault(_ => _.Name == "songResult");
            if (songResult == null || songResult.Descendants("lyrics").First().Value == "Not found")
            {
                return null;
            }

            var webAddress = songResult.Descendants("url").First().Value;

            var scraped = await this.ScrapeLyrics(webAddress).ConfigureAwait(false);

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

        private async Task<XDocument> GetResult(string method, params Tuple<string, object>[] values)
        {
            var ns = XNamespace.Get("urn:LyricWiki");
            var body = new XElement(ns.GetName(method));
            foreach (var value in values)
            {
                body.Add(new XElement(value.Item1, value.Item2));
            }

            var result = await this.soapClient.PostAsync(this.uri, SoapVersion.Soap11, body, (XElement)null, $"{ns.NamespaceName}#{method}").ConfigureAwait(false);

            var bytes = await result.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            if (bytes[0] != '<')
            {
                byte[] decompressedBytes;

                using (var stream = new System.IO.MemoryStream(bytes))
                {
                    using (var decompression = new System.IO.Compression.GZipStream(stream, System.IO.Compression.CompressionMode.Decompress))
                    {
                        using (var decompressed = new System.IO.MemoryStream())
                        {
                            await decompression.CopyToAsync(decompressed).ConfigureAwait(false);
                            decompressedBytes = decompressed.ToArray();
                        }
                    }
                }

                bytes = decompressedBytes;
            }

            XDocument document;
            using (var stream = new System.IO.MemoryStream(bytes))
            {
                document = XDocument.Load(stream);
            }

            return document;
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