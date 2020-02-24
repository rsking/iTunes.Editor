// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.CommandLine;
    using System.CommandLine.Builder;
    using System.CommandLine.Hosting;
    using System.CommandLine.Invocation;
    using System.CommandLine.Parsing;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// The program class.
    /// </summary>
    internal class Program
    {
        private const bool DefaultForce = false;

        private const string DefaultType = "plist";

        private static Task<int> Main(string[] args)
        {
            var inputArgument = new Argument<System.IO.FileSystemInfo>("input") { Description = "The input", Arity = ArgumentArity.ZeroOrOne }.ExistingOnly();
            var typeOption = new Option(new[] { "-t", "--type" }, "The type of input") { Argument = new Argument<string>("TYPE") };
            var propertiesOptions = new Option(new[] { "-p", "--property" }, "A property to set on the input provider") { Argument = new Argument<string>("PROPERTY") { Arity = ArgumentArity.ZeroOrMore } };

            var listCommandBuilder = new CommandBuilder(new Command("list", "Lists the files from the specified input") { Handler = CommandHandler.Create<IHost, System.IO.FileSystemInfo, string, string[]>(List) })
                .AddArgument(inputArgument)
                .AddOption(typeOption)
                .AddOption(propertiesOptions);

            var composerCommandBuilder = new CommandBuilder(new Command("composer", "Gets the composers for a specific song/artist") { Handler = CommandHandler.Create<IHost, string, string, string>(Composer) })
                .AddArgument(new Argument<string>("artist") { Description = "The artist" })
                .AddArgument(new Argument<string>("song") { Description = "The song" })
                .AddOption(new Option(new[] { "-p", "--provider" }, "The type of provider") { Argument = new Argument<string>("provider") });

            var lyricsCommandBuilder = new CommandBuilder(new Command("lyrics", "Gets the lyrics for a specific song/artist") { Handler = CommandHandler.Create<IHost, string, string, string>(Lyrics) })
                .AddArgument(new Argument<string>("artist") { Description = "The artist" })
                .AddArgument(new Argument<string>("song") { Description = "The song" })
                .AddOption(new Option(new[] { "-p", "--provider" }, "The type of provider") { Argument = new Argument<string>("provider") });

            var mediaInfoCommandBuilder = new CommandBuilder(new Command("mediainfo", "Gets the media info for a specific file") { Handler = CommandHandler.Create<string>(MediaInfo) })
                .AddArgument(new Argument<string>("file") { Description = "The file to get information for" });

            var forceOption = new Option(new[] { "-f", "--force" }, "Whether to force the update") { Argument = new Argument<bool>() };
            var fileArgument = new Argument<System.IO.FileInfo>("file") { Description = "The file" };

            var updateComposerFileCommandBuilder = new CommandBuilder(new Command("composer", "Updates the composer in the specific file") { Handler = CommandHandler.Create<IHost, System.IO.FileInfo, bool>(UpdateComposerFile) })
                .AddArgument(fileArgument)
                .AddOption(forceOption);

            var updateLyricsFileCommandBuilder = new CommandBuilder(new Command("lyrics", "Updates the lyrics in the specific file") { Handler = CommandHandler.Create<IHost, System.IO.FileInfo, bool>(UpdateLyricsFile) })
                .AddArgument(fileArgument)
                .AddOption(forceOption);

            var updateAllFileCommandBuilder = new CommandBuilder(new Command("all", "Updates the specific file using all the updaters") { Handler = CommandHandler.Create<IHost, System.IO.FileInfo, bool>(UpdateAllFile) })
                .AddArgument(fileArgument)
                .AddOption(forceOption);

            var updateFileCommandBuilder = new CommandBuilder(new Command("file", "Updates a specific file"))
                .AddCommand(updateComposerFileCommandBuilder.Command)
                .AddCommand(updateLyricsFileCommandBuilder.Command)
                .AddCommand(updateAllFileCommandBuilder.Command);

            var updateComposerListCommandBuilder = new CommandBuilder(new Command("composer", "Updates the composer in the specific list") { Handler = CommandHandler.Create<IHost, System.IO.FileSystemInfo, string, bool>(UpdateComposerList) })
                .AddArgument(inputArgument)
                .AddOption(typeOption)
                .AddOption(forceOption);

            var updateLyricsListCommandBuilder = new CommandBuilder(new Command("lyrics", "Updates the lyrics in the specific list") { Handler = CommandHandler.Create<IHost, System.IO.FileSystemInfo, string, bool>(UpdateLyricsList) })
                .AddArgument(inputArgument)
                .AddOption(typeOption)
                .AddOption(forceOption);

            var updateAllListCommandBuilder = new CommandBuilder(new Command("all", "Updates the specific list using all the updaters") { Handler = CommandHandler.Create<IHost, System.IO.FileSystemInfo, string, bool>(UpdateAllList) })
                .AddArgument(inputArgument)
                .AddOption(typeOption)
                .AddOption(forceOption);

            var updateListCommandBuilder = new CommandBuilder(new Command("list", "Updates a specific list"))
                .AddCommand(updateComposerListCommandBuilder.Command)
                .AddCommand(updateLyricsListCommandBuilder.Command)
                .AddCommand(updateAllListCommandBuilder.Command);

            var updateCommandBuilder = new CommandBuilder(new Command("update", "Updates a specific file or list"))
                .AddCommand(updateFileCommandBuilder.Command)
                .AddCommand(updateListCommandBuilder.Command);

            var builder = new CommandLineBuilder()
                .UseDefaults()
                .UseHost(Host.CreateDefaultBuilder, ConfigureHost)
                .AddCommand(listCommandBuilder.Command)
                .AddCommand(composerCommandBuilder.Command)
                .AddCommand(lyricsCommandBuilder.Command)
                .AddCommand(mediaInfoCommandBuilder.Command)
                .AddCommand(updateCommandBuilder.Command);

            return builder.Build().InvokeAsync(args);
        }

        private static void ConfigureHost(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices((hostingContext, serviceCollection) =>
            {
                // Lyrics
                serviceCollection
                    .AddApiSeeds(hostingContext.Configuration)
                    .AddChartLyrics()
                    .AddWikia()
                    .AddPurgoMalum();

                // Composers
                serviceCollection
                    .AddApraAmcos();

                // song providers
                serviceCollection
                    .AddFolder()
                    .AddIPod()
                    .AddPList()
                    .AddITunes();

                // tag provider
                serviceCollection
                    .AddTagLib()
                    .AddMediaInfo();

                // add services
                serviceCollection
                    .AddTransient<IUpdateComposerService, UpdateComposerService>()
                    .AddTransient<IUpdateLyricsService, UpdateLyricsService>();
            });
        }

        private static async Task List(IHost host, System.IO.FileSystemInfo input, string type = DefaultType, params string[] property)
        {
            var songsProvider = host.Services.GetRequiredService<ISongsProvider>(type).SetProperties(property);
            switch (songsProvider)
            {
                case IFolderProvider folderProvider:
                    folderProvider.Folder = input.FullName;
                    break;
                case IFileProvider fileProvider:
                    fileProvider.File = input.FullName;
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
        }

        private static async Task Composer(IHost host, string artist, string song, string provider = "apra_amcos")
        {
            var composerProvider = host.Services.GetRequiredService<IComposerProvider>(provider);
            await foreach (var composer in composerProvider
                .GetComposersAsync(new SongInformation(song, artist, artist, null, null, null)))
            {
                Console.WriteLine(composer);
            }
        }

        private static async Task Lyrics(IHost host, string artist, string song, string provider = "wikia")
        {
            var lyrics = await host.Services.GetRequiredService<ILyricsProvider>(provider)
               .GetLyricsAsync(new SongInformation(song, artist, artist, null, null, null))
               .ConfigureAwait(false);
            Console.WriteLine(lyrics);
        }

        private static async Task MediaInfo(string file)
        {
            var mediaInfoTagProvider = new MediaInfo.MediaInfoTagProvider { File = file };
            var mediaInfo = await mediaInfoTagProvider.GetTagAsync().ConfigureAwait(false);
            if (mediaInfo is null)
            {
                return;
            }

            Console.WriteLine($"{mediaInfo.JoinedPerformers} - {mediaInfo.Title}");
        }

        private static Task UpdateComposerFile(IHost host, System.IO.FileInfo file, bool force = DefaultForce)
        {
            var service = host.Services.GetRequiredService<IUpdateComposerService>();
            return service.UpdateAsync(SongInformation.FromFile(file.FullName), force);
        }

        private static Task UpdateLyricsFile(IHost host, System.IO.FileInfo file, bool force = DefaultForce)
        {
            var service = host.Services.GetRequiredService<IUpdateLyricsService>();
            return service.UpdateAsync(SongInformation.FromFile(file.FullName), force);
        }

        private static async Task UpdateAllFile(IHost host, System.IO.FileInfo file, bool force = DefaultForce)
        {
            var songInformation = SongInformation.FromFile(file.FullName);
            await host.Services.GetRequiredService<IUpdateComposerService>().UpdateAsync(songInformation, force).ConfigureAwait(false);
            await host.Services.GetRequiredService<IUpdateLyricsService>().UpdateAsync(songInformation, force).ConfigureAwait(false);
        }

        private static async Task UpdateList(IHost host, System.IO.FileSystemInfo input, string type, bool force, Func<SongInformation, bool, Task> updateFunction)
        {
            var songsProvider = host.Services.GetRequiredService<ISongsProvider>(type);
            switch (songsProvider)
            {
                case IFolderProvider folderProvider:
                    folderProvider.Folder = input.FullName;
                    break;
                case IFileProvider fileProvider:
                    fileProvider.File = input.FullName;
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

                await updateFunction(song, force).ConfigureAwait(false);
            }
        }

        private static Task UpdateComposerList(IHost host, System.IO.FileSystemInfo input, string type = DefaultType, bool force = DefaultForce) => UpdateList(host, input, type, force, host.Services.GetRequiredService<IUpdateComposerService>().UpdateAsync);

        private static Task UpdateLyricsList(IHost host, System.IO.FileSystemInfo input, string type = DefaultType, bool force = DefaultForce) => UpdateList(host, input, type, force, host.Services.GetRequiredService<IUpdateLyricsService>().UpdateAsync);

        private static Task UpdateAllList(IHost host, System.IO.FileSystemInfo input, string type = DefaultType, bool force = DefaultForce)
        {
            var composerService = host.Services.GetRequiredService<IUpdateComposerService>();
            var lyricsService = host.Services.GetRequiredService<IUpdateLyricsService>();
            return UpdateList(host, input, type, force, async (songInformation, force) =>
            {
                await composerService.UpdateAsync(songInformation, force).ConfigureAwait(false);
                await lyricsService.UpdateAsync(songInformation, force).ConfigureAwait(false);
            });
        }
    }
}
