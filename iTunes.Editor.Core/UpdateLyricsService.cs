﻿// -----------------------------------------------------------------------
// <copyright file="UpdateLyricsService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The implementation of <see cref="IUpdateLyricsService"/>.
    /// </summary>
    public class UpdateLyricsService : IUpdateLyricsService
    {
        private readonly IEnumerable<ILyricsProvider> providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateLyricsService"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        public UpdateLyricsService(IEnumerable<ILyricsProvider> providers)
        {
            this.providers = providers;
        }

        /// <inheritdoc />
        public SongInformation Update(SongInformation songInformation, bool force = false)
        {
            using (var updater = new Updater(songInformation))
            {
                if (!updater.ShouldUpdate(force))
                {
                    return songInformation;
                }

                foreach (var provider in this.providers)
                {
                    var lyrics = provider.GetLyrics(songInformation);
                    if (lyrics != null)
                    {
                        return updater.Update(lyrics);
                    }
                }
            }

            return songInformation;
        }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task<SongInformation> UpdateAsync(SongInformation songInformation, bool force = false)
        {
            using (var updater = new Updater(songInformation))
            {
                if (!updater.ShouldUpdate(force))
                {
                    return songInformation;
                }

                foreach (var provider in this.providers)
                {
                    var lyrics = await provider.GetLyricsAsync(songInformation).ConfigureAwait(false);
                    if (lyrics != null)
                    {
                        return updater.Update(lyrics);
                    }
                }
            }

            return songInformation;
        }

        private sealed class Updater : IDisposable
        {
            private static readonly string[] InvalidLyrics = new string[] { "Not found", "We haven't lyrics" };

            private readonly TagLib.Mpeg4.AppleTag appleTag;

            private TagLib.File file;

            private SongInformation songInformation;

            public Updater(SongInformation songInformation)
            {
                this.songInformation = songInformation;
                this.file = TagLib.File.Create(songInformation.Name);
                if (this.file != null)
                {
                    this.appleTag = this.file.GetTag(TagLib.TagTypes.Apple) as TagLib.Mpeg4.AppleTag;
                }
            }

            public bool ShouldUpdate(bool force)
            {
                return this.appleTag != null && (string.IsNullOrEmpty(this.appleTag.Lyrics) || this.appleTag.Lyrics.Trim().Length == 0 || force);
            }

            public SongInformation Update(string lyrics)
            {
                if (string.IsNullOrEmpty(lyrics) || InvalidLyrics.Any(temp => lyrics.StartsWith(temp)))
                {
                    // this has no lyrics, so update
                    if (this.appleTag.AddNoLyrics())
                    {
                        this.file.Save();
                    }

                    if (!string.IsNullOrEmpty(this.appleTag.Lyrics))
                    {
                        this.appleTag.Lyrics = null;
                        this.file.Save();
                    }
                }
                else
                {
                    this.appleTag.Lyrics = lyrics;
                    this.appleTag.CleanLyrics();
                    this.appleTag.RemoveNoLyrics();
                    this.file.Save();
                }
                
                return this.songInformation = (SongInformation)this.file;
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
