// <copyright file="AZLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.Lyrics.AZLyrics;

using Humanizer;

/// <summary>
/// The "http://www.azlyrics.com" <see cref="ILyricsProvider"/>.
/// </summary>
public sealed class AZLyricsProvider : ILyricsProvider, IDisposable
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
                    _ = lyrics.Append(ScrapeNode(childNode));
                    break;
                case "#text":
                    // scrape this
                    _ = lyrics.Append(childNode.InnerText.Trim('\r', '\n'));
                    break;
                case "br":
                    _ = lyrics.AppendLine();
                    break;
            }
        }

        return lyrics.ToString();
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
        var lyricsNode = document.DocumentNode
            .SelectSingleNode("//div[@class='lyricsh']")?
            .ParentNode?
            .SelectNodes("div")
            .FirstOrDefault(div => div.Attributes.Count == 0 || div.Attributes.All(attribute => !string.Equals(attribute.Name, "class", StringComparison.Ordinal)));

        return lyricsNode is null ? null : ScrapeNode(lyricsNode);
    }
}
