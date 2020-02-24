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
        public UpdateComposerService(IEnumerable<IComposerProvider> providers) => this.providers = providers.ToArray();

        /// <inheritdoc />
        public SongInformation Update(SongInformation songInformation, bool force = false)
        {
            if (songInformation is null)
            {
                return songInformation!;
            }

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
            if (songInformation is null)
            {
                return songInformation!;
            }

            using (var updater = new Updater(songInformation))
            {
                if (!updater.ShouldUpdate(force))
                {
                    return songInformation;
                }

                foreach (var provider in this.providers)
                {
                    var composers = provider.GetComposersAsync(songInformation);
                    if (composers != null)
                    {
                        return await updater.UpdateAsync(composers).ConfigureAwait(false);
                    }
                }
            }

            return songInformation;
        }

        private sealed class Updater : IDisposable
        {
            private readonly TagLib.Tag? appleTag;

            private TagLib.File.IFileAbstraction? fileAbstraction;

            private TagLib.File? file;

            private SongInformation songInformation;

            public Updater(SongInformation songInformation)
            {
                this.songInformation = songInformation ?? SongInformation.Empty;
                if (this.songInformation.Name is null)
                {
                    return;
                }

                this.fileAbstraction = new LocalFileAbstraction(this.songInformation.Name, true);
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
                if (this.file is null || this.appleTag is null)
                {
                    return this.songInformation;
                }

                var valueToSet = composers.Select(_ => _.ToString()).ToArray();

                if (this.appleTag.Composers.SequenceEqual(valueToSet))
                {
                    return this.songInformation;
                }

                this.appleTag.Composers = valueToSet;
                this.file.Save();
                return this.songInformation = (SongInformation)this.file;
            }

            public async System.Threading.Tasks.Task<SongInformation> UpdateAsync(IAsyncEnumerable<Name> composers)
            {
                if (this.file is null || this.appleTag is null)
                {
                    return this.songInformation;
                }

                var valueToSet = await composers.Select(_ => _.ToString())
                    .ToArrayAsync()
                    .ConfigureAwait(false);

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
