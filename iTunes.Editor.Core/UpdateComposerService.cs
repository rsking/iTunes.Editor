// -----------------------------------------------------------------------
// <copyright file="UpdateComposerService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The implementation of <see cref="IUpdateComposerService"/>.
    /// </summary>
    public class UpdateComposerService : IUpdateComposerService
    {
        private readonly IEnumerable<IComposerProvider> providers;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateComposerService"/> class.
        /// </summary>
        /// <param name="providers">The providers.</param>
        public UpdateComposerService(IEnumerable<IComposerProvider> providers) => this.providers = providers;

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
                    var composers = provider.GetComposers(songInformation);
                    if (composers != null)
                    {
                        return updater.Update(composers);
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
                    var composers = await provider.GetComposersAsync(songInformation).ConfigureAwait(false);
                    if (composers != null)
                    {
                        return updater.Update(composers);
                    }
                }
            }

            return songInformation;
        }

        private sealed class Updater : IDisposable
        {
            private readonly TagLib.Tag appleTag;

            private TagLib.File.IFileAbstraction fileAbstraction;

            private TagLib.File file;

            private SongInformation songInformation;

            public Updater(SongInformation songInformation)
            {
                this.songInformation = songInformation;
                this.fileAbstraction = new LocalFileAbstraction(songInformation.Name);
                this.file = TagLib.File.Create(this.fileAbstraction);
                if (this.file == null)
                {
                    if (this.fileAbstraction is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                else
                {
                    this.appleTag = this.file.GetTag(TagLib.TagTypes.Apple);
                }
            }

            public bool ShouldUpdate(bool force) => this.appleTag != null && (string.IsNullOrEmpty(this.appleTag.FirstComposer) || force);

            public SongInformation Update(IEnumerable<Name> composers)
            {
                var valueToSet = composers.Select(_ => _.ToString()).ToArray();

                if (this.appleTag.Composers.SequenceEqual(valueToSet))
                {
                    return this.songInformation;
                }

                this.appleTag.Composers = valueToSet;
                this.file.Save();
                return this.songInformation = (SongInformation)this.file;
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                this.file?.Dispose();
                this.file = null;
                
                if (this.fileAbstraction is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                this.fileAbstraction = null;
            }
        }
    }
}
