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
            var inputArgument = new Argument<string>("input") { Description = "The input", Arity = ArgumentArity.ZeroOrOne };
            var typeOption = new Option(new[] { "-t", "--type" }, "The type of input") { Argument = new Argument<string>("type") };

            var listCommand = new Command("list", "Lists the files from the specified input");
            listCommand.AddArgument(inputArgument);
            listCommand.AddOption(typeOption);
            listCommand.Handler = CommandHandler.Create<string, string>(List);

            var composerCommand = new Command("composer", "Gets the composers for a specific song/artist");
            composerCommand.AddArgument(new Argument<string>("artist") { Description = "The artist" });
            composerCommand.AddArgument(new Argument<string>("song") { Description = "The song" });
            composerCommand.AddOption(new Option(new[] { "-p", "--provider" }, "The type of provider") { Argument = new Argument<string>("provider") });
            composerCommand.Handler = CommandHandler.Create<string, string, string>(Composer);

            var lyricsCommand = new Command("lyrics", "Gets the lyrics for a specific song/artist");
            lyricsCommand.AddArgument(new Argument<string>("artist") { Description = "The artist" });
            lyricsCommand.AddArgument(new Argument<string>("song") { Description = "The song" });
            lyricsCommand.AddOption(new Option(new[] { "-p", "--provider" }, "The type of provider") { Argument = new Argument<string>("provider") });
            lyricsCommand.Handler = CommandHandler.Create<string, string, string>(Lyrics);

            var mediaInfoCommand = new Command("mediainfo", "Gets the media info for a specific file");
            mediaInfoCommand.AddArgument(new Argument<string>("file") { Description = "The file to get information for" });
            mediaInfoCommand.Handler = CommandHandler.Create<string>(MediaInfo);

            var forceOption = new Option(new[] { "-f", "--force" }, "Whether to force the update") { Argument = new Argument<bool>() };
            var fileArgument = new Argument<string>("file") { Description = "The file" };

            var updateComposerFileCommand = new Command("composer", "Updates the composer in the specific file");
            updateComposerFileCommand.AddArgument(fileArgument);
            updateComposerFileCommand.AddOption(forceOption);
            updateComposerFileCommand.Handler = CommandHandler.Create<string, bool>(UpdateComposerFile);

            var updateLyricsFileCommand = new Command("lyrics", "Updates the lyrics in the specific file");
            updateLyricsFileCommand.AddArgument(fileArgument);
            updateLyricsFileCommand.AddOption(forceOption);
            updateLyricsFileCommand.Handler = CommandHandler.Create<string, bool>(UpdateLyricsFile);

            var updateAllFileCommand = new Command("all", "Updates the specific file using all the updaters");
            updateAllFileCommand.AddArgument(fileArgument);
            updateAllFileCommand.AddOption(forceOption);
            updateAllFileCommand.Handler = CommandHandler.Create<string, bool>(UpdateAllFile);

            var updateFileCommand = new Command("file", "Updates a specific file");
            updateFileCommand.AddCommand(updateComposerFileCommand);
            updateFileCommand.AddCommand(updateLyricsFileCommand);
            updateFileCommand.AddCommand(updateAllFileCommand);

            var updateComposerListCommand = new Command("composer", "Updates the composer in the specific list");
            updateComposerListCommand.AddArgument(inputArgument);
            updateComposerListCommand.AddOption(typeOption);
            updateComposerListCommand.AddOption(forceOption);
            updateComposerListCommand.Handler = CommandHandler.Create<string, string, bool>(UpdateComposerList);

            var updateLyricsListCommand = new Command("lyrics", "Updates the lyrics in the specific list");
            updateLyricsListCommand.AddArgument(inputArgument);
            updateLyricsListCommand.AddOption(typeOption);
            updateLyricsListCommand.AddOption(forceOption);
            updateLyricsListCommand.Handler = CommandHandler.Create<string, string, bool>(UpdateLyricsList);

            var updateAllListCommand = new Command("all", "Updates the specific list using all the updaters");
            updateAllListCommand.AddArgument(inputArgument);
            updateAllListCommand.AddOption(typeOption);
            updateAllListCommand.AddOption(forceOption);
            updateAllListCommand.Handler = CommandHandler.Create<string, string, bool>(UpdateAllList);

            var updateListCommand = new Command("list", "Updates a specific list");
            updateListCommand.AddCommand(updateComposerListCommand);
            updateListCommand.AddCommand(updateLyricsListCommand);
            updateListCommand.AddCommand(updateAllListCommand);

            var updateCommand = new Command("update", "Updates a specific file or list");
            updateCommand.AddCommand(updateFileCommand);
            updateCommand.AddCommand(updateListCommand);

            var builder = new CommandLineBuilder()
                .UseDefaults()
                .UseHost(Host.CreateDefaultBuilder, ConfigureHost)
                .AddCommand(listCommand)
                .AddCommand(composerCommand)
                .AddCommand(lyricsCommand)
                .AddCommand(mediaInfoCommand)
                .AddCommand(updateCommand);

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

        private static async Task List(IHost host, string input, string type = DefaultType, params string[] property)
        {
            var songsProvider = host.Services.GetRequiredService<ISongsProvider>(type);
            switch (songsProvider)
            {
                case IFolderProvider folderProvider:
                    folderProvider.Folder = input.Expand();
                    break;
                case IFileProvider fileProvider:
                    fileProvider.File = input.Expand();
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
            Console.WriteLine($"{mediaInfo.JoinedPerformers} - {mediaInfo.Title}");
        }

        private static Task UpdateComposerFile(IHost host, string file, bool force = DefaultForce)
        {
            var service = host.Services.GetRequiredService<IUpdateComposerService>();
            return service.UpdateAsync(SongInformation.FromFile(file.Expand()), force);
        }

        private static Task UpdateLyricsFile(IHost host, string file, bool force = DefaultForce)
        {
            var service = host.Services.GetRequiredService<IUpdateLyricsService>();
            return service.UpdateAsync(SongInformation.FromFile(file.Expand()), force);
        }

        private static async Task UpdateAllFile(IHost host, string file, bool force = DefaultForce)
        {
            var songInformation = SongInformation.FromFile(file.Expand());
            await host.Services.GetRequiredService<IUpdateComposerService>().UpdateAsync(songInformation, force).ConfigureAwait(false);
            await host.Services.GetRequiredService<IUpdateLyricsService>().UpdateAsync(songInformation, force).ConfigureAwait(false);
        }

        private static async Task UpdateList(IHost host, string input, string type, bool force, Func<SongInformation, bool, Task> updateFunction)
        {
            var songsProvider = host.Services.GetRequiredService<ISongsProvider>(type);
            switch (songsProvider)
            {
                case IFolderProvider folderProvider:
                    folderProvider.Folder = input.Expand();
                    break;
                case IFileProvider fileProvider:
                    fileProvider.File = input.Expand();
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

        private static Task UpdateComposerList(IHost host, string input, string type = DefaultType, bool force = DefaultForce) => UpdateList(host, input, type, force, host.Services.GetRequiredService<IUpdateComposerService>().UpdateAsync);

        private static Task UpdateLyricsList(IHost host, string input, string type = DefaultType, bool force = DefaultForce) => UpdateList(host, input, type, force, host.Services.GetRequiredService<IUpdateLyricsService>().UpdateAsync);

        private static Task UpdateAllList(IHost host, string input, string type = DefaultType, bool force = DefaultForce)
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
