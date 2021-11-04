// -----------------------------------------------------------------------
// <copyright file="UpdateComposerService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

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
    public async ValueTask<SongInformation> UpdateAsync(SongInformation songInformation, bool force = false, CancellationToken cancellationToken = default)
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
                var composers = provider.GetComposersAsync(songInformation, cancellationToken);
                if (composers is not null)
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

        private TagLib.File? file;

        private SongInformation songInformation;

        public Updater(SongInformation songInformation)
        {
            this.songInformation = songInformation ?? SongInformation.Empty;
            if (this.songInformation.Name is null)
            {
                return;
            }

            this.file = TagLib.File.Create(this.songInformation.Name);
            if (this.file is not null)
            {
                this.appleTag = this.file.GetTag(TagLib.TagTypes.Apple);
            }
        }

        public bool ShouldUpdate(bool force) => this.appleTag is not null && (string.IsNullOrEmpty(this.appleTag.FirstComposer) || force);

        public async Task<SongInformation> UpdateAsync(IAsyncEnumerable<Name> composers)
        {
            if (this.file is null || this.appleTag is null)
            {
                return this.songInformation;
            }

            var valueToSet = await composers.Select(_ => _.ToString())
                .ToArrayAsync()
                .ConfigureAwait(false);

            if (this.appleTag.Composers.SequenceEqual(valueToSet, StringComparer.Ordinal))
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
        }
    }
}
