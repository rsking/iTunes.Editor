// -----------------------------------------------------------------------
// <copyright file="ChartLyricsLyricsProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ProxyBuilder")]

namespace ITunes.Editor.ChartLyrics
{
    using System.Threading.Tasks;

    /// <summary>
    /// The <see cref="ILyricsProvider"/> for www.chartlyrics.com.
    /// </summary>
    public class ChartLyricsLyricsProvider : ILyricsProvider
    {
        private readonly System.Uri uri = new System.Uri("http://api.chartlyrics.com/apiv1.asmx");

        private readonly apiv1SoapClient channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLyricsLyricsProvider"/> class.
        /// </summary>
        public ChartLyricsLyricsProvider()
        {
            var binding = new System.ServiceModel.BasicHttpBinding { Name = "apiv1Soap" };
            this.channel = new apiv1SoapClient(binding, new System.ServiceModel.EndpointAddress(this.uri));
        }

        /// <inheritdoc/>
        public async Task<string?> GetLyricsAsync(SongInformation tagInformation, System.Threading.CancellationToken cancellationToken = default)
        {
            if (tagInformation is null)
            {
                return null;
            }

            var artist = string.Join("; ", tagInformation.Performers);
            var songTitle = tagInformation.Title;
            SearchLyricDirectResponse searchLyricDirectResponse;
            try
            {
                searchLyricDirectResponse = await this.channel.SearchLyricDirectAsync(artist, songTitle).ConfigureAwait(false);
            }
            catch (System.ServiceModel.FaultException)
            {
                return null;
            }
            catch (System.ServiceModel.CommunicationException)
            {
                return null;
            }

            cancellationToken.ThrowIfCancellationRequested();

            return GetLyricsImpl(this.logger, artist, songTitle, searchLyricDirectResponse?.Body?.SearchLyricDirectResult);
        }
        }

        private static string GetLyricsImpl(string artist, string song, api.chartlyrics.com.GetLyricResult getLyricResult)
        {
            if (getLyricResult?.LyricId <= 0)
            {
                return null;
            }

            if (getLyricResult.LyricArtist.Equals(artist, System.StringComparison.InvariantCultureIgnoreCase)
                && getLyricResult.LyricSong.Equals(song, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return getLyricResult.Lyric;
            }

            System.Console.WriteLine($"\tChartLyrics - Incorrect Lyrics found, Expected {artist}|{song}, but got {getLyricResult.LyricArtist}|{getLyricResult.LyricSong}");
            return null;
        }
    }
}
