// -----------------------------------------------------------------------
// <copyright file="Program.Info.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Ninject;

    /// <summary>
    /// The list command.
    /// </summary>
    [Command(Description = "Lists the files from the specified input")]
    internal sealed class ListCommand : CommandBase
    {
        private string input;

        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        [Argument(0, Description = "The input", Name = "input")]
        public string Input
        {
             get => this.input;
             set => this.input = value.Expand();
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        [Option("-t|--type <type>", "The type of input", CommandOptionType.SingleValue)]
        public string Type { get; set; } = "plist";

        private Program Parent { get; }

        /// <inheritdoc/>
        protected override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var songsProvider = this.Parent.Kernel.Get<ISongsProvider>(this.Type);
            switch (songsProvider)
            {
                case IFolderProvider folderProvider:
                    folderProvider.Folder = this.Input;
                    break;
                case IFileProvider fileProvider:
                    fileProvider.File = this.Input;
                    break;
            }

            foreach (var song in await songsProvider
                .GetTagInformationAsync()
                .ConfigureAwait(false))
            {
                var performers = song.Performers == null ? null : string.Join("; ", song.Performers);
                var exists = System.IO.File.Exists(song.Name);
                Console.WriteLine($"{performers}|{song.Title}|{song.Name}|{exists}");
            }

            return 0;
        }
    }

    /// <summary>
    /// The composer command.
    /// </summary>
    [Command(Description = "Gets the composers for a specific song/artist")]
    internal sealed class ComposerCommand : CommandBase
    {
        /// <summary>
        /// Gets or sets the artist.
        /// </summary>
        [Argument(0, Description = "The artist", Name = "artist")]
        public string Artist { get; set; }

        /// <summary>
        /// Gets or sets the song.
        /// </summary>
        [Argument(1, Description = "The song", Name = "song")]
        public string Song { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Option("-p|--provider <provider>", "The type of provider", CommandOptionType.SingleValue)]
        public string Provider { get; set; } = "apra_amcos";

        private Program Parent { get; }

        /// <inheritdoc/>
        protected async override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            foreach (var composer in await this.Parent.Kernel.Get<IComposerProvider>(this.Provider)
                .GetComposersAsync(new SongInformation(this.Song, this.Artist, this.Artist, null, null, null))
                .ConfigureAwait(false))
            {
                Console.WriteLine(composer);
            }

            return 0;
        }
    }

    /// <summary>
    /// The composer command.
    /// </summary>
    [Command(Description = "Gets the lyrics for a specific song/artist")]
    internal sealed class LyricsCommand : CommandBase
    {
        /// <summary>
        /// Gets or sets the artist.
        /// </summary>
        [Argument(0, Description = "The artist", Name = "artist")]
        public string Artist { get; set; }

        /// <summary>
        /// Gets or sets the song.
        /// </summary>
        [Argument(1, Description = "The song", Name = "song")]
        public string Song { get; set; }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        [Option("-p|--provider <provider>", "The type of provider", CommandOptionType.SingleValue)]
        public string Provider { get; set; } = "wikia";

        private Program Parent { get; }

        /// <inheritdoc/>
        protected async override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var lyrics = await this.Parent.Kernel.Get<ILyricsProvider>(this.Provider)
                .GetLyricsAsync(new SongInformation(this.Song, this.Artist, this.Artist, null, null, null))
                .ConfigureAwait(false);
            Console.WriteLine(lyrics);

            return 0;
        }
    }

    /// <summary>
    /// The media info command.
    /// </summary>
    [Command(Description = "Gets the media info for a specific file")]
    internal sealed class MediaInfoCommand : CommandBase
    {
        private string file;

        /// <summary>
        /// Gets or sets the file.
        /// </summary>
        [Argument(0, Description = "The file to get information for", Name = "file")]
        public string File
        {
             get => this.file;
             set => this.file = value.Expand();
        }

        /// <inheritdoc/>
        protected async override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var mediaInfoTagProvider = new MediaInfo.MediaInfoTagProvider { File = this.File };
            var mediaInfo = await mediaInfoTagProvider.GetTagAsync().ConfigureAwait(false);
            Console.WriteLine($"{mediaInfo.JoinedPerformers} - {mediaInfo.Title}");
            return 0;
        }
    }
}