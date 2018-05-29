// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Reviewed.")]

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Ninject;

    /// <summary>
    /// The program class.
    /// </summary>
    [Command]
    [VersionOption("--version", "1.0.0")]
    [Subcommand("list", typeof(ListCommand))]
    [Subcommand("composer", typeof(ComposerCommand))]
    [Subcommand("lyrics", typeof(LyricsCommand))]
    [Subcommand("mediainfo", typeof(MediaInfoCommand))]
    [Subcommand("update", typeof(UpdateCommand))]
    internal class Program : CommandBase
    {
        /// <summary>
        /// Gets the Ninject kernel.
        /// </summary>
        internal IKernel Kernel { get; } = new StandardKernel();

        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <returns>The return code.</returns>
        private static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);
    }

    /// <summary>
    /// The list command.
    /// </summary>
    [Command(Description = "Lists the files from the specified input")]
    internal class ListCommand : CommandBase
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
                Console.WriteLine($"{performers}|{song.Title}|{song.Name}");
            }

            return 0;
        }
    }

    /// <summary>
    /// The composer command.
    /// </summary>
    [Command(Description = "Gets the composers for a specific song/artist")]
    internal class ComposerCommand : CommandBase
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
    internal class LyricsCommand : CommandBase
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
    internal class MediaInfoCommand : CommandBase
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

        private Program Parent { get; }

        /// <inheritdoc/>
        protected async override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var mediaInfoTagProvider = new MediaInfo.MediaInfoTagProvider { File = this.File };
            var mediaInfo = await mediaInfoTagProvider.GetTagAsync().ConfigureAwait(false);
            Console.WriteLine($"{mediaInfo.JoinedPerformers} - {mediaInfo.Title}");
            return 0;
        }
    }

    /// <summary>
    /// The update command.
    /// </summary>
    [Command(Description = "Updates a specific file")]
    [Subcommand("composer", typeof(UpdateComposerCommand))]
    [Subcommand("lyrics", typeof(UpdateLyricsCommand))]
    internal class UpdateCommand : CommandBase
    {
        /// <summary>
        /// Gets the accessible parent.
        /// </summary>
        internal Program Parent { get; }
    }

    /// <summary>
    /// The update command base.
    /// </summary>
    internal abstract class UpdateCommandBase : CommandBase
    {
        private string file;

        /// <summary>
        /// Gets or sets the file to update.
        /// </summary>
        [Argument(0, "file", "The file")]
        public string File
        {
             get => this.file;
             set => this.file = value.Expand();
        }
    }

    /// <summary>
    /// The update composer command.
    /// </summary>
    [Command(Description = "Updates the composer in the specific file")]
    internal class UpdateComposerCommand : UpdateCommandBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether to force the update.
        /// </summary>
        [Option("-f|--force", "Whether to force the update", CommandOptionType.NoValue)]
        public bool Force { get; set; }

        private UpdateCommand Parent { get; }

        /// <inheritdoc/>
        protected async override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var service = this.Parent.Parent.Kernel.Get<IUpdateComposerService>();
            foreach (var file in System.IO.Directory.EnumerateFiles(System.IO.Path.GetDirectoryName(this.File), System.IO.Path.GetFileName(this.File)))
            {
                var songInformation = SongInformation.FromFile(file);
                Console.WriteLine($"Processing {songInformation.Name}");
                await service.UpdateAsync(songInformation, this.Force).ConfigureAwait(false);
            }

            return 0;
        }
    }

    /// <summary>
    /// The update lyrics command.
    /// </summary>
    [Command(Description = "Updates the lyrics in the specific file")]
    internal class UpdateLyricsCommand : UpdateCommandBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether to force the update.
        /// </summary>
        [Option("-f|--force", "Whether to force the update", CommandOptionType.NoValue)]
        public bool Force { get; set; }

        private UpdateCommand Parent { get; }

        /// <inheritdoc/>
        protected async override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var service = this.Parent.Parent.Kernel.Get<IUpdateLyricsService>();
            foreach (var file in System.IO.Directory.EnumerateFiles(System.IO.Path.GetDirectoryName(this.File), System.IO.Path.GetFileName(this.File)))
            {
                var songInformation = SongInformation.FromFile(file);
                Console.WriteLine($"Processing {songInformation.Name}");
                await service.UpdateAsync(songInformation, this.Force).ConfigureAwait(false);
            }

            return 0;
        }
    }

    /// <summary>
    /// The command base.
    /// </summary>
    [HelpOption("-h|--help")]
    internal abstract class CommandBase
    {
        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The task.</returns>
        protected virtual Task<int> OnExecuteAsync(CommandLineApplication app) => Task.FromResult(0);
    }
}
