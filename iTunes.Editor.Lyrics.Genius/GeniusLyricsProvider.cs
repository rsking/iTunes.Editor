// <copyright file="GeniusLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Lyrics.Genius;

using Humanizer;

/// <summary>
/// The "http://genius.com" <see cref="ILyricsProvider"/>.
/// </summary>
public sealed class GeniusLyricsProvider : ILyricsProvider, IDisposable
{
    private readonly HttpClient client = new();

    /// <inheritdoc/>
    public void Dispose() => this.client.Dispose();

    /// <inheritdoc/>
    public async ValueTask<string?> GetLyricsAsync(SongInformation tagInformation, CancellationToken cancellationToken = default)
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
        return ScrapeNodeBuilder(node).ToString();

        static System.Text.StringBuilder ScrapeNodeBuilder(HtmlAgilityPack.HtmlNode node, System.Text.StringBuilder? builder = default)
        {
            builder ??= new();
            foreach (var childNode in node.ChildNodes)
            {
                switch (childNode.Name)
                {
                    case "i":
                    case "b":
                    case "a":
                    case "span":
                        ScrapeNodeBuilder(childNode, builder);
                        break;
                    case "#text":
                        // scrape this
                        var text = HtmlAgilityPack.HtmlEntity.DeEntitize(childNode.InnerText.Trim('\n'));
                        builder.Append(text);
                        break;
                    case "br":
                        builder.AppendLine();
                        break;
                }
            }

            return builder;
        }
    }

    private async Task<string?> ScrapeLyricsAsync(string address, CancellationToken cancellationToken)
    {
        string pageText;
        try
        {
            pageText = await this.client.GetStringAsync(address).ConfigureAwait(false);
        }
        catch (HttpRequestException)
        {
            return null;
        }

        cancellationToken.ThrowIfCancellationRequested();

        // parse the HTML
        var document = new HtmlAgilityPack.HtmlDocument();
        document.LoadHtml(pageText);

        // select the div
        var paragraphNode = GetExactNode(document.DocumentNode, "lyrics") ?? SearchNodes(document.DocumentNode, "Lyrics__Container");

        return paragraphNode is null ? null : ScrapeNode(paragraphNode);

        static HtmlAgilityPack.HtmlNode? GetExactNode(HtmlAgilityPack.HtmlNode documentNode, string @class)
        {
            var nodes = documentNode
                .SelectNodes($"//div[@class='{@class}']");
            if (nodes is null)
            {
                return default;
            }

            var node = nodes.FirstOrDefault();
            if (node is null)
            {
                return default;
            }

            return node.SelectSingleNode("p");
        }

        static HtmlAgilityPack.HtmlNode? SearchNodes(HtmlAgilityPack.HtmlNode documentNode, string prefix)
        {
            return documentNode
                .SelectNodes("//div")
                .FirstOrDefault(div => div.GetClasses().Any(@class => @class.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)));
        }
    }
}
