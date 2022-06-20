// -----------------------------------------------------------------------
// <copyright file="UpdateLyricsService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

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
    public ValueTask<SongInformation> UpdateAsync(
        SongInformation songInformation,
        bool force = false,
        CancellationToken cancellationToken = default)
        => this.UpdateAsync(songInformation, force, force, cancellationToken);

    /// <inheritdoc />
    public async ValueTask<SongInformation> UpdateAsync(
        SongInformation songInformation,
        bool forceSearch = false,
        bool forceExplicit = false,
        CancellationToken cancellationToken = default)
    {
        if (songInformation is null)
        {
            return songInformation!;
        }

        using var updater = new Updater(this.logger, songInformation);
        if (updater.ShouldUpdateLyrics(forceSearch))
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

        var instance = forceExplicit && updater.CanUpdateExplicit()
            ? this.explicitLyrics
            : NullExplicitLyricsProvider.Instance;
        return await updater
            .UpdateAsync(instance, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
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
            if (this.file is not null)
            {
                this.appleTag = this.file.GetTag(TagLib.TagTypes.Apple) as TagLib.Mpeg4.AppleTag;
            }
        }

        public bool ShouldUpdateLyrics(bool force) => this.appleTag is not null && (string.IsNullOrEmpty(this.appleTag.Lyrics) || this.appleTag.Lyrics.Trim().Length == 0 || force);

        public bool CanUpdateExplicit() => this.appleTag is not null && !string.IsNullOrEmpty(this.appleTag.Lyrics) && this.appleTag.Lyrics.Trim().Length > 0;

        public async ValueTask<SongInformation> UpdateAsync(IExplicitLyricsProvider explicitLyricsProvider, string? lyrics = null, CancellationToken cancellationToken = default)
        {
            if (this.file is null || this.appleTag is null)
            {
                return this.songInformation;
            }

            var updated = false;
            lyrics ??= this.appleTag.Lyrics;
            lyrics = lyrics.SafeReplace("\r\n", NewLine);

            if (string.IsNullOrEmpty(lyrics) || InvalidLyrics.Any(temp => lyrics!.StartsWith(temp, StringComparison.CurrentCultureIgnoreCase)))
            {
                this.logger.LogDebug(Properties.Resources.NoLyricsFound, this.songInformation);

                // this has no lyrics, so update
#pragma warning disable RCS1233, S2178
                if (this.appleTag.AddNoLyrics() | this.appleTag.SetUnrated())
#pragma warning restore RCS1233, S2178
                {
                    updated = true;
                    this.logger.LogDebug(Properties.Resources.SavingWithNoLyricsAndUnrated, this.songInformation);
                    this.file.Save();
                }

                if (!string.IsNullOrEmpty(this.appleTag.Lyrics))
                {
                    this.logger.LogDebug(Properties.Resources.SettingLyricsToNull, this.songInformation);
                    updated = true;
                    this.appleTag.Lyrics = null;
                    this.file.Save();
                }
            }
            else
            {
                updated = await Update(explicitLyricsProvider, lyrics, updated, cancellationToken).ConfigureAwait(false);

                if (updated)
                {
                    this.file.Save();
                }

                async Task<bool> Update(IExplicitLyricsProvider explicitLyricsProvider, string? lyrics, bool updated, CancellationToken cancellationToken)
                {
                    if (!string.Equals(this.appleTag.Lyrics, lyrics, StringComparison.Ordinal))
                    {
                        updated = true;
                        this.appleTag.Lyrics = lyrics;
                        this.logger.LogDebug(Properties.Resources.UpdatedLyrics, this.songInformation);
                    }

                    if (this.appleTag.CleanLyrics(NewLine))
                    {
                        updated = true;
                        this.logger.LogDebug(Properties.Resources.UpdatedCleanedLyrics, this.songInformation);
                    }

                    if (this.appleTag.RemoveNoLyrics())
                    {
                        updated = true;
                        this.logger.LogDebug(Properties.Resources.RemovedNoLyricsTag, this.songInformation);
                    }

                    if (await this.appleTag.UpdateRatingAsync(explicitLyricsProvider, cancellationToken).ConfigureAwait(false))
                    {
                        updated = true;
                        this.logger.LogDebug(Properties.Resources.UpdatedRating, this.songInformation);
                    }

                    return updated;
                }
            }

            if (updated)
            {
                this.songInformation = (SongInformation)this.file;
            }

            return this.songInformation;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            this.file?.Dispose();
            this.file = null;
        }
    }

    private sealed class NullExplicitLyricsProvider : IExplicitLyricsProvider
    {
        public static readonly IExplicitLyricsProvider Instance = new NullExplicitLyricsProvider();

        private NullExplicitLyricsProvider()
        {
        }

        public ValueTask<bool?> IsExplicitAsync(string lyrics, CancellationToken cancellationToken) => new(result: false);
    }
}
