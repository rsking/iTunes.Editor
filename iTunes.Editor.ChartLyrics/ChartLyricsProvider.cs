// -----------------------------------------------------------------------
// <copyright file="ChartLyricsProvider.cs" company="RossKing">
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
    public class ChartLyricsProvider : ILyricsProvider
    {
        private readonly System.Uri uri = new System.Uri("http://api.chartlyrics.com/apiv1.asmx");

        private readonly apiv1SoapClient channel;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChartLyricsProvider"/> class.
        /// </summary>
        public ChartLyricsProvider()
        {
            var binding = new System.ServiceModel.BasicHttpBinding { Name = "apiv1Soap" };
            this.channel = new apiv1SoapClient(binding, new System.ServiceModel.EndpointAddress(this.uri));
        }

        /// <inheritdoc/>
        public string GetLyrics(SongInformation tagInformation)
        {
            var artist = string.Join("; ", tagInformation.Performers);
            var songTitle = tagInformation.Title;
            api.chartlyrics.com.GetLyricResult getLyricResult;
            try
            {
                getLyricResult = this.channel.SearchLyricDirect(artist, songTitle);
            }
            catch (System.ServiceModel.FaultException)
            {
                return null;
            }

            return GetLyricsImpl(artist, songTitle, getLyricResult);
        }

        /// <inheritdoc/>
        public async Task<string> GetLyricsAsync(SongInformation tagInformation)
        {
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

            return GetLyricsImpl(artist, songTitle, searchLyricDirectResponse?.Body?.SearchLyricDirectResult);
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
