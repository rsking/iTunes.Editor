// <copyright file="UpdateTempoService.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor;

using Microsoft.Extensions.Logging;

/// <summary>
/// The implementation of <see cref="IUpdateTempoService"/>.
/// </summary>
public class UpdateTempoService : IUpdateTempoService
{
    private readonly ILogger logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateTempoService"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public UpdateTempoService(
        ILogger<UpdateTempoService> logger) => this.logger = logger;

    /// <inheritdoc/>
    public ValueTask<SongInformation> UpdateAsync(SongInformation songInformation, bool force = false, CancellationToken cancellationToken = default)
    {
        if (songInformation is not null)
        {
            using var updater = new Updater(this.logger, songInformation);
            if (updater.ShouldUpdateTempo(force))
            {
                songInformation = updater.Update();
            }
        }

        return new(songInformation!);
    }

    private sealed class Updater : IDisposable
    {
        private static readonly IEnumerable<string> Names = new string[] { "BPM", "TEMPO" };

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

        public bool ShouldUpdateTempo(bool force) => !string.IsNullOrEmpty(this.GetTempo().Tempo) || force;

        public SongInformation Update()
        {
            if (this.file is null || this.appleTag is null)
            {
                return this.songInformation;
            }

            var updated = false;
            var (tempo, name) = this.GetTempo();
            if (uint.TryParse(tempo, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var bpm))
            {
                this.logger.LogInformation(Properties.Resources.UpdatingBeatsPerMinute, this.songInformation, bpm);
                this.appleTag.BeatsPerMinute = bpm;
                this.appleTag.SetDashBox("com.apple.iTunes", name, string.Empty);
                this.file.Save();
                updated = true;
            }

            if (updated)
            {
                this.songInformation = (SongInformation)this.file;
            }

            return this.songInformation;
        }

        private (string? Tempo, string? Name) GetTempo() => this.appleTag is not null
            ? Names.Select(name => (this.appleTag.GetDashBox("com.apple.iTunes", name), name)).FirstOrDefault(v => !string.IsNullOrEmpty(v.Item1))
            : default;

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            this.file?.Dispose();
            this.file = null;
        }
    }
}
