// -----------------------------------------------------------------------
// <copyright file="UpdateLyricsService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The implementation of <see cref="IUpdateLyricsService"/>.
    /// </summary>
    public class UpdateLyricsService : IUpdateLyricsService
    {
        private const string NewLine = "\r";

        private readonly ILogger logger;

        private readonly IExplicitLyricsProvider explicitLyrics;

        private readonly IEnumerable<ILyricsProvider> providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLyricsService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="explicitLyricsProvider">The explicit lyrics provider.</param>
        /// <param name="providers">The providers.</param>
        public UpdateLyricsService(
            ILogger<UpdateLyricsService> logger,
            IExplicitLyricsProvider explicitLyricsProvider,
            IEnumerable<ILyricsProvider> providers) => (this.logger, this.explicitLyrics, this.providers) = (logger, explicitLyricsProvider, providers.ToArray());

        /// <inheritdoc />
        public async System.Threading.Tasks.Task<SongInformation> UpdateAsync(SongInformation songInformation, bool force = false, System.Threading.CancellationToken cancellationToken = default)
        {
            if (songInformation is null)
            {
                return songInformation!;
            }

            using var updater = new Updater(this.logger, songInformation);
            if (updater.ShouldUpdate(force))
            {
                foreach (var provider in this.providers)
                {
                    var lyrics = await provider.GetLyricsAsync(songInformation, cancellationToken).ConfigureAwait(false);
                    if (!string.IsNullOrEmpty(lyrics))
                    {
                        return await updater.UpdateAsync(this.explicitLyrics, lyrics, cancellationToken).ConfigureAwait(false);
                    }
                }
            }

            return await updater.UpdateAsync(this.explicitLyrics, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        private sealed class Updater : IDisposable
        {
            private static readonly string[] InvalidLyrics = new string[] { "Not found", "We haven't lyrics" };

            private readonly ILogger logger;

            private readonly TagLib.Mpeg4.AppleTag? appleTag;

            private TagLib.File? file;

            private SongInformation songInformation;

            public Updater(ILogger logger, SongInformation songInformation)
            {
                this.logger = logger;
                this.songInformation = songInformation ?? SongInformation.Empty;
                if (this.songInformation.Name is null)
                {
                    return;
                }

                this.file = TagLib.File.Create(this.songInformation.Name);
                if (this.file != null)
                {
                    this.appleTag = this.file.GetTag(TagLib.TagTypes.Apple) as TagLib.Mpeg4.AppleTag;
                }
            }

            public bool ShouldUpdate(bool force) => this.appleTag != null && (string.IsNullOrEmpty(this.appleTag.Lyrics) || this.appleTag.Lyrics.Trim().Length == 0 || force);

            public async System.Threading.Tasks.Task<SongInformation> UpdateAsync(IExplicitLyricsProvider explicitLyricsProvider, string? lyrics = null, System.Threading.CancellationToken cancellationToken = default)
            {
                if (this.file is null || this.appleTag is null)
                {
                    return this.songInformation;
                }

                var appleTag = this.appleTag!;

                var updated = false;
                lyrics = (lyrics ?? this.appleTag?.Lyrics)?.Replace("\r\n", NewLine);

                if (string.IsNullOrEmpty(lyrics) || InvalidLyrics.Any(temp => lyrics!.StartsWith(temp, StringComparison.CurrentCultureIgnoreCase)))
                {
                    this.logger.LogDebug(Properties.Resources.NoLyricsFound, this.songInformation);

                    // this has no lyrics, so update
#pragma warning disable RCS1233 // Use short-circuiting operator.
                    if (appleTag.AddNoLyrics() | appleTag.SetUnrated())
#pragma warning restore RCS1233 // Use short-circuiting operator.
                    {
                        updated = true;
                        this.logger.LogDebug(Properties.Resources.SavingWithNoLyricsAndUnrated, this.songInformation);
                        this.file.Save();
                    }

                    if (!string.IsNullOrEmpty(appleTag.Lyrics))
                    {
                        this.logger.LogDebug(Properties.Resources.SettingLyricsToNull, this.songInformation);
                        updated = true;
                        appleTag.Lyrics = null;
                        this.file.Save();
                    }
                }
                else
                {
                    if (appleTag.Lyrics != lyrics)
                    {
                        updated = true;
                        appleTag.Lyrics = lyrics;
                        this.logger.LogDebug(Properties.Resources.UpdatedLyrics, this.songInformation);
                    }

                    if (appleTag.CleanLyrics(NewLine))
                    {
                        updated = true;
                        this.logger.LogDebug(Properties.Resources.UpdatedCleanedLyrics, this.songInformation);
                    }

                    if (appleTag.RemoveNoLyrics())
                    {
                        updated = true;
                        this.logger.LogDebug(Properties.Resources.RemovedNoLyricsTag, this.songInformation);
                    }

                    if (await appleTag.UpdateRatingAsync(explicitLyricsProvider, cancellationToken).ConfigureAwait(false))
                    {
                        updated = true;
                        this.logger.LogDebug(Properties.Resources.UpdatedRating, this.songInformation);
                    }

                    if (updated)
                    {
                        this.file.Save();
                    }
                }

                return !updated ? this.songInformation : this.songInformation = (SongInformation)this.file;
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                this.file?.Dispose();
                this.file = null;
            }
        }
    }
}
