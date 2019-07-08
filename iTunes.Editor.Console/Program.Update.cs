// -----------------------------------------------------------------------
// <copyright file="Program.Update.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Ninject;

    /// <summary>
    /// The update command.
    /// </summary>
    [Command(Description = "Updates a specific file or list")]
    [Subcommand("file", typeof(UpdateFileCommand))]
    [Subcommand("list", typeof(UpdateListCommand))]
    internal sealed class UpdateCommand : CommandBase
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
        /// <summary>
        /// Gets the accessible parent.
        /// </summary>
        internal UpdateCommand Parent { get; }
    }

    /// <summary>
    /// The update command base.
    /// </summary>
    [Command(Description = "Updates a specific file")]
    [Subcommand("composer", typeof(UpdateFileComposerCommand))]
    [Subcommand("lyrics", typeof(UpdateFileLyricsCommand))]
    [Subcommand("all", typeof(UpdateFileAllCommand))]
    internal sealed class UpdateFileCommand : UpdateCommandBase
    {
    }

    /// <summary>
    /// The update sub-command base.
    /// </summary>
    internal abstract class UpdateSubcommandBase : CommandBase
    {
        /// <summary>
        /// Gets or sets a value indicating whether to force the update.
        /// </summary>
        [Option("-f|--force", "Whether to force the update", CommandOptionType.NoValue)]
        public bool Force { get; set; }

        /// <summary>
        /// Updates the specified song information.
        /// </summary>
        /// <param name="songInformation">The song information.</param>
        /// <returns>The update task.</returns>
        protected abstract Task Update(SongInformation songInformation);
    }

    /// <summary>
    /// The update file command base.
    /// </summary>
    internal abstract class UpdateFileCommandBase : UpdateSubcommandBase
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

        /// <summary>
        /// Gets the parent command.
        /// </summary>
        protected UpdateFileCommand Parent { get; }

        /// <inheritdoc/>
        protected async override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            foreach (var file in System.IO.Directory.EnumerateFiles(System.IO.Path.GetDirectoryName(this.File), System.IO.Path.GetFileName(this.File)))
            {
                var songInformation = SongInformation.FromFile(file);
                Console.WriteLine($"Processing {songInformation.Name}");
                await this.Update(songInformation).ConfigureAwait(false);
            }

            return 0;
        }
    }

    /// <summary>
    /// The update composer command.
    /// </summary>
    [Command(Description = "Updates the composer in the specific file")]
    internal sealed class UpdateFileComposerCommand : UpdateFileCommandBase
    {
        private IUpdateComposerService service;

        /// <inheritdoc/>
        protected override Task Update(SongInformation songInformation) => this.service.UpdateAsync(songInformation, this.Force);

        /// <inheritdoc/>
        protected override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (this.service == null)
            {
                this.service = this.Parent.Parent.Parent.Kernel.Get<IUpdateComposerService>();
            }

            return base.OnExecuteAsync(app);
        }
    }

    /// <summary>
    /// The update file lyrics command.
    /// </summary>
    [Command(Description = "Updates the lyrics in the specific file")]
    internal sealed class UpdateFileLyricsCommand : UpdateFileCommandBase
    {
        private IUpdateLyricsService service;

        /// <inheritdoc/>
        protected override Task Update(SongInformation songInformation) => this.service.UpdateAsync(songInformation, this.Force);

        /// <inheritdoc/>
        protected override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (this.service == null)
            {
                this.service = this.Parent.Parent.Parent.Kernel.Get<IUpdateLyricsService>();
            }

            return base.OnExecuteAsync(app);
        }
    }

    /// <summary>
    /// The update file lyrics command.
    /// </summary>
    [Command(Description = "Updates the specific file using all the updaters")]
    internal sealed class UpdateFileAllCommand : UpdateFileCommandBase
    {
        private IUpdateComposerService composerService;

        private IUpdateLyricsService lyricsService;

        /// <inheritdoc/>
        protected async override Task Update(SongInformation songInformation)
        {
            await this.composerService.UpdateAsync(songInformation, this.Force).ConfigureAwait(false);
            await this.lyricsService.UpdateAsync(songInformation, this.Force).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (this.composerService == null)
            {
                this.composerService = this.Parent.Parent.Parent.Kernel.Get<IUpdateComposerService>();
            }

            if (this.lyricsService == null)
            {
                this.lyricsService = this.Parent.Parent.Parent.Kernel.Get<IUpdateLyricsService>();
            }

            return base.OnExecuteAsync(app);
        }
    }

    /// <summary>
    /// The update command base.
    /// </summary>
    [Command(Description = "Updates a specific list")]
    [Subcommand("composer", typeof(UpdateListComposerCommand))]
    [Subcommand("lyrics", typeof(UpdateListLyricsCommand))]
    [Subcommand("all", typeof(UpdateListAllCommand))]
    internal sealed class UpdateListCommand : UpdateCommandBase
    {
    }

    /// <summary>
    /// The update list command base.
    /// </summary>
    internal abstract class UpdateListCommandBase : UpdateSubcommandBase
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

        /// <summary>
        /// Gets the parent command.
        /// </summary>
        protected UpdateListCommand Parent { get; }

        /// <inheritdoc/>
        protected override async Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            var songsProvider = this.Parent.Parent.Parent.Kernel.Get<ISongsProvider>(this.Type);
            switch (songsProvider)
            {
                case IFolderProvider folderProvider:
                    folderProvider.Folder = this.Input;
                    break;
                case IFileProvider fileProvider:
                    fileProvider.File = this.Input;
                    break;
            }

            var songs = await songsProvider
                .GetTagInformationAsync()
                .ConfigureAwait(false);

            foreach (var song in songs.Where(_ => _.Name != null && System.IO.File.Exists(_.Name)))
            {
                // check the location
                var mediaType = song.GetMediaType();
                switch (mediaType)
                {
                    case "Music":
                    case null:
                        Console.WriteLine($"Processing {song}");
                        break;
                    default:
                        Console.WriteLine($"Skipping {song}: {mediaType}");
                        continue;
                }

                await this.Update(song).ConfigureAwait(false);
            }

            return 0;
        }
    }

    /// <summary>
    /// The update composer command.
    /// </summary>
    [Command(Description = "Updates the composer in all the files in the specified list")]
    internal sealed class UpdateListComposerCommand : UpdateListCommandBase
    {
        private IUpdateComposerService service;

        /// <inheritdoc/>
        protected override Task Update(SongInformation songInformation) => this.service.UpdateAsync(songInformation, this.Force);

        /// <inheritdoc/>
        protected override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (this.service == null)
            {
                this.service = this.Parent.Parent.Parent.Kernel.Get<IUpdateComposerService>();
            }

            return base.OnExecuteAsync(app);
        }
    }

    /// <summary>
    /// The update file lyrics command.
    /// </summary>
    [Command(Description = "Updates the lyrics in all the files in the specified list")]
    internal sealed class UpdateListLyricsCommand : UpdateListCommandBase
    {
        private IUpdateLyricsService service;

        /// <inheritdoc/>
        protected override Task Update(SongInformation songInformation) => this.service.UpdateAsync(songInformation, this.Force);

        /// <inheritdoc/>
        protected override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (this.service == null)
            {
                this.service = this.Parent.Parent.Parent.Kernel.Get<IUpdateLyricsService>();
            }

            return base.OnExecuteAsync(app);
        }
    }

    /// <summary>
    /// The update list command.
    /// </summary>
    [Command(Description = "Updates all the files in the specified list using all the updaters")]
    internal sealed class UpdateListAllCommand : UpdateListCommandBase
    {
        private IUpdateComposerService composerService;

        private IUpdateLyricsService lyricsService;

        /// <inheritdoc/>
        protected async override Task Update(SongInformation songInformation)
        {
            await this.composerService.UpdateAsync(songInformation, this.Force).ConfigureAwait(false);
            await this.lyricsService.UpdateAsync(songInformation, this.Force).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        protected override Task<int> OnExecuteAsync(CommandLineApplication app)
        {
            if (this.composerService == null)
            {
                this.composerService = this.Parent.Parent.Parent.Kernel.Get<IUpdateComposerService>();
            }

            if (this.lyricsService == null)
            {
                this.lyricsService = this.Parent.Parent.Parent.Kernel.Get<IUpdateLyricsService>();
            }

            return base.OnExecuteAsync(app);
        }
    }
}