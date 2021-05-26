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
    using System.CommandLine.IO;
    using System.CommandLine.Parsing;
    using System.CommandLine.Rendering;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The program class.
    /// </summary>
    internal sealed class Program
    {
        private const bool DefaultForce = false;

        private const string DefaultType = "plist";

        private const string DefaultLyricProvider = "wikia";

        private const string DefaultComposerProvider = "apra_amcos";

        private Program()
        {
        }

        private static Task<int> Main(string[] args)
        {
            const string lyricsCommand = "lyrics";
            const string listCommand = "list";
            const string allCommand = "all";
            const string composerCommand = "composer";
            const string check = "check";

            var inputArgument = new Argument<System.IO.FileSystemInfo>("input") { Description = "The input", Arity = ArgumentArity.ZeroOrOne }.ExistingOnly();
            var typeOption = new Option<string>(new[] { "-t", "--type" }, "The type of input");
            var propertiesOptions = new Option<string>("--property", "A property to set on the input provider", ArgumentArity.ZeroOrMore);
            propertiesOptions.AddAlias("-");
            var artistArgument = new Argument<string>("artist") { Description = "The artist" };
            var songArgument = new Argument<string>("song") { Description = "The song" };

            static Option CreateProviderOption(string defaultValue)
            {
                return new Option<string>(new[] { "-p", "--provider" }, () => defaultValue, "The type of provider");
            }

            var listCommandBuilder = new CommandBuilder(new Command(listCommand, "Lists the files from the specified input") { Handler = CommandHandler.Create<IHost, System.IO.FileSystemInfo, string, System.Threading.CancellationToken, string[]>(List) })
                .AddArgument(inputArgument)
                .AddOption(typeOption)
                .AddOption(propertiesOptions);

            var composerCommandBuilder = new CommandBuilder(new Command(composerCommand, "Gets the composers for a specific song/artist") { Handler = CommandHandler.Create<IHost, string, string, string, System.Threading.CancellationToken>(Composer) })
                .AddArgument(artistArgument)
                .AddArgument(songArgument)
                .AddOption(CreateProviderOption(DefaultComposerProvider));

            var lyricsCommandBuilder = new CommandBuilder(new Command(lyricsCommand, "Gets the lyrics for a specific song/artist") { Handler = CommandHandler.Create<IHost, string, string, string, System.Threading.CancellationToken>(Lyrics) })
                .AddArgument(artistArgument)
                .AddArgument(songArgument)
                .AddOption(CreateProviderOption(DefaultLyricProvider));

            var mediaInfoCommandBuilder = new CommandBuilder(new Command(nameof(Editor.MediaInfo).ToLower(System.Globalization.CultureInfo.CurrentCulture), "Gets the media info for a specific file") { Handler = CommandHandler.Create<IHost, string, System.Threading.CancellationToken>(MediaInfo) })
                .AddArgument(new Argument<string>("file") { Description = "The file to get information for" });

            var forceOption = new Option<bool>(new[] { "-f", "--force" }, "Whether to force the update");
            var fileArgument = new Argument<System.IO.FileInfo>("file") { Description = "The file" };

            var updateComposerFileCommandBuilder = new CommandBuilder(new Command(composerCommand, "Updates the composer in the specific file") { Handler = CommandHandler.Create<IHost, System.IO.FileInfo, bool, System.Threading.CancellationToken>(UpdateComposerFile) })
                .AddArgument(fileArgument)
                .AddOption(forceOption);

            var updateLyricsFileCommandBuilder = new CommandBuilder(new Command(lyricsCommand, "Updates the lyrics in the specific file") { Handler = CommandHandler.Create<IHost, System.IO.FileInfo, bool, System.Threading.CancellationToken>(UpdateLyricsFile) })
                .AddArgument(fileArgument)
                .AddOption(forceOption);

            var updateAllFileCommandBuilder = new CommandBuilder(new Command(allCommand, "Updates the specific file using all the updaters") { Handler = CommandHandler.Create<IHost, System.IO.FileInfo, bool, System.Threading.CancellationToken>(UpdateAllFile) })
                .AddArgument(fileArgument)
                .AddOption(forceOption);

            var updateFileCommandBuilder = new CommandBuilder(new Command("file", "Updates a specific file"))
                .AddCommand(updateComposerFileCommandBuilder.Command)
                .AddCommand(updateLyricsFileCommandBuilder.Command)
                .AddCommand(updateAllFileCommandBuilder.Command);

            var updateComposerListCommandBuilder = new CommandBuilder(new Command(composerCommand, "Updates the composer in the specific list") { Handler = CommandHandler.Create<IHost, System.IO.FileSystemInfo, string, bool, System.Threading.CancellationToken>(UpdateComposerList) })
                .AddArgument(inputArgument)
                .AddOption(typeOption)
                .AddOption(forceOption);

            var updateLyricsListCommandBuilder = new CommandBuilder(new Command(lyricsCommand, "Updates the lyrics in the specific list") { Handler = CommandHandler.Create<IHost, System.IO.FileSystemInfo, string, bool, System.Threading.CancellationToken>(UpdateLyricsList) })
                .AddArgument(inputArgument)
                .AddOption(typeOption)
                .AddOption(forceOption);

            var updateAllListCommandBuilder = new CommandBuilder(new Command(allCommand, "Updates the specific list using all the updaters") { Handler = CommandHandler.Create<IHost, System.IO.FileSystemInfo, string, bool, System.Threading.CancellationToken>(UpdateAllList) })
                .AddArgument(inputArgument)
                .AddOption(typeOption)
                .AddOption(forceOption);

            var updateListCommandBuilder = new CommandBuilder(new Command(listCommand, "Updates a specific list"))
                .AddCommand(updateComposerListCommandBuilder.Command)
                .AddCommand(updateLyricsListCommandBuilder.Command)
                .AddCommand(updateAllListCommandBuilder.Command);

            var updateCommandBuilder = new CommandBuilder(new Command("update", "Updates a specific file or list"))
                .AddCommand(updateFileCommandBuilder.Command)
                .AddCommand(updateListCommandBuilder.Command);

            var checkCommandBuilder = new CommandBuilder(new Command(check) { Handler = CommandHandler.Create<IHost, IConsole, System.IO.FileSystemInfo, string, System.IO.DirectoryInfo, System.Threading.CancellationToken>(Check) })
                .AddArgument(inputArgument)
                .AddOption(typeOption)
                .AddOption(new Option<System.IO.DirectoryInfo>(new[] { "-f", "--folder" }, "The folder").ExistingOnly());

            var builder = new CommandLineBuilder()
                .UseDefaults()
                .UseAnsiTerminalWhenAvailable()
                .UseHost(Host.CreateDefaultBuilder, configureHost => configureHost
                    .UseDefaultITunes()
                    .ConfigureServices(services =>
                    {
                        services.Configure<InvocationLifetimeOptions>(options => options.SuppressStatusMessages = true);
                        services.AddTransient<IConfigurator<ITunesLib.ITunesSongsProvider>, NullConfigurator<ITunesLib.ITunesSongsProvider>>();
                    }))
                .AddCommand(listCommandBuilder.Command)
                .AddCommand(composerCommandBuilder.Command)
                .AddCommand(lyricsCommandBuilder.Command)
                .AddCommand(mediaInfoCommandBuilder.Command)
                .AddCommand(updateCommandBuilder.Command)
                .AddCommand(checkCommandBuilder.Command);

            return builder
                .Build()
                .InvokeAsync(args.Select(Environment.ExpandEnvironmentVariables).ToArray());
        }

        private static async Task List(IHost host, System.IO.FileSystemInfo input, string type = DefaultType, System.Threading.CancellationToken cancellationToken = default, params string[] property)
        {
            //// var options = new System.IO.EnumerationOptions { AttributesToSkip = System.IO.FileAttributes.Hidden, RecurseSubdirectories = true };
            //// await System.IO.File.WriteAllLinesAsync("C:\\Temp\\folder.txt", System.IO.Directory.EnumerateFiles("D:\\Users\\rskin\\OneDrive\\Music\\iTunes\\iTunes Media", "*", options).OrderBy(file => file), System.Text.Encoding.Default).ConfigureAwait(false);

            var songsProvider = host.Services
                .GetRequiredService<ISongsProvider>(type)
                .SetProperties(property)
                .SetPath(input);
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            //// await System.IO.File.WriteAllLinesAsync("C:\\Temp\\itunes.txt", songsProvider.GetTagInformationAsync(cancellationToken).Where(song => song.Name?.Length > 0).OrderBy(song => song.Name).Select(song => song.Name).ToEnumerable()).ConfigureAwait(false);
            //// using var stream = new System.IO.StreamWriter("C:\\Temp\\itunes.txt");
            await foreach (var song in songsProvider.GetTagInformationAsync(cancellationToken).ConfigureAwait(false))
            {
                //// await stream.WriteLineAsync(song.Name).ConfigureAwait(false);
                logger.LogInformation(Console.Properties.Resources.ListLog, song.Performers.ToJoinedString(), song.Title, song.Name, System.IO.File.Exists(song.Name));
            }
        }

        private static async Task Composer(IHost host, string artist, string song, string provider = DefaultComposerProvider, System.Threading.CancellationToken cancellationToken = default)
        {
            var composerProvider = host.Services.GetRequiredService<IComposerProvider>(provider);
            var logger = host.Services.GetRequiredService<ILogger<Program>>();
            await foreach (var composer in composerProvider
                .GetComposersAsync(new SongInformation(song, artist, artist, albumPerformer: null, sortAlbumPerformer: null, album: null, name: null), cancellationToken)
                .ConfigureAwait(false))
            {
                logger.LogInformation(Console.Properties.Resources.ComposerLog, composer);
            }
        }

        private static async Task Lyrics(IHost host, string artist, string song, string provider = DefaultLyricProvider, System.Threading.CancellationToken cancellationToken = default)
        {
            var lyrics = await host.Services.GetRequiredService<ILyricsProvider>(provider)
               .GetLyricsAsync(new SongInformation(song, artist, artist, albumPerformer: null, sortAlbumPerformer: null, album: null, name: null), cancellationToken)
               .ConfigureAwait(false);
            host.Services.GetRequiredService<ILogger<Program>>().LogInformation(Console.Properties.Resources.LyricsLog, lyrics);
        }

        private static async Task MediaInfo(IHost host, string file, System.Threading.CancellationToken cancellationToken = default)
        {
            var mediaInfoTagProvider = new MediaInfo.MediaInfoTagProvider { File = file };
            var mediaInfo = await mediaInfoTagProvider.GetTagAsync(cancellationToken).ConfigureAwait(false);
            if (mediaInfo is null)
            {
                return;
            }

            host.Services.GetRequiredService<ILogger<Program>>().LogInformation(Console.Properties.Resources.MediaInfoLog, mediaInfo.JoinedPerformers, mediaInfo.Title);
        }

        private static async Task UpdateComposerFile(IHost host, System.IO.FileInfo file, bool force = DefaultForce, System.Threading.CancellationToken cancellationToken = default)
        {
            var service = host.Services.GetRequiredService<IUpdateComposerService>();
            var song = await SongInformation.FromFileAsync(file.FullName, cancellationToken).ConfigureAwait(false);
            await service.UpdateAsync(song, force, cancellationToken).ConfigureAwait(false);
        }

        private static async Task UpdateLyricsFile(IHost host, System.IO.FileInfo file, bool force = DefaultForce, System.Threading.CancellationToken cancellationToken = default)
        {
            var service = host.Services.GetRequiredService<IUpdateLyricsService>();
            var song = await SongInformation.FromFileAsync(file.FullName, cancellationToken).ConfigureAwait(false);
            await service.UpdateAsync(song, force, cancellationToken).ConfigureAwait(false);
        }

        private static async Task UpdateAllFile(IHost host, System.IO.FileInfo file, bool force = DefaultForce, System.Threading.CancellationToken cancellationToken = default)
        {
            var songInformation = await SongInformation.FromFileAsync(file.FullName, cancellationToken).ConfigureAwait(false);
            await host.Services.GetRequiredService<IUpdateComposerService>().UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
            await host.Services.GetRequiredService<IUpdateLyricsService>().UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
        }

        private static async Task UpdateList(IHost host, System.IO.FileSystemInfo input, string type, bool force, Func<SongInformation, bool, System.Threading.CancellationToken, ValueTask<SongInformation>> updateFunction, System.Threading.CancellationToken cancellationToken)
        {
            var songsProvider = host.Services
                .GetRequiredService<ISongsProvider>(type)
                .SetPath(input);

            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            await foreach (var song in songsProvider
                .GetTagInformationAsync(cancellationToken)
                .Where(_ => _.Name is not null && System.IO.File.Exists(_.Name))
                .ConfigureAwait(false))
            {
                // check the location
                var mediaType = song.GetMediaKind();
                switch (mediaType)
                {
                    case MediaKind.Song:
                        logger.LogInformation(Console.Properties.Resources.Processing, song);
                        break;
                    default:
                        logger.LogInformation(Console.Properties.Resources.Skipping, song, mediaType);
                        continue;
                }

                await updateFunction(song, force, cancellationToken).ConfigureAwait(false);
            }
        }

        private static Task UpdateComposerList(IHost host, System.IO.FileSystemInfo input, string type = DefaultType, bool force = DefaultForce, System.Threading.CancellationToken cancellationToken = default) => UpdateList(host, input, type, force, host.Services.GetRequiredService<IUpdateComposerService>().UpdateAsync, cancellationToken);

        private static Task UpdateLyricsList(IHost host, System.IO.FileSystemInfo input, string type = DefaultType, bool force = DefaultForce, System.Threading.CancellationToken cancellationToken = default) => UpdateList(host, input, type, force, host.Services.GetRequiredService<IUpdateLyricsService>().UpdateAsync, cancellationToken);

        private static Task UpdateAllList(IHost host, System.IO.FileSystemInfo input, string type = DefaultType, bool force = DefaultForce, System.Threading.CancellationToken cancellationToken = default)
        {
            var composerService = default(IUpdateComposerService);
            var lyricsService = default(IUpdateLyricsService);
            return UpdateList(
                host,
                input,
                type,
                force,
                Update,
                cancellationToken);

            async ValueTask<SongInformation> Update(SongInformation songInformation, bool force, System.Threading.CancellationToken cancellationToken)
            {
                composerService ??= host.Services.GetRequiredService<IUpdateComposerService>();
                lyricsService ??= host.Services.GetRequiredService<IUpdateLyricsService>();

                songInformation = await composerService.UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
                return await lyricsService.UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
            }
        }

        private static async Task Check(IHost host, IConsole console, System.IO.FileSystemInfo input, string type, System.IO.DirectoryInfo folder, System.Threading.CancellationToken cancellationToken = default)
        {
            var songsProvider = host.Services
                .GetRequiredService<ISongsProvider>(type)
                .SetPath(input);

            var songs = await songsProvider
                .GetTagInformationAsync(cancellationToken)
                .ToArrayAsync(cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var options = new System.IO.EnumerationOptions { AttributesToSkip = System.IO.FileAttributes.Hidden, RecurseSubdirectories = true };
            var files = folder
                .EnumerateFiles("*", options)
                .ToArray();

            var onlyInInput = songs.ExceptBy(files.Select(file => file.FullName), song => song.Name);
            var onlyInFolder = files.ExceptBy(songs.Select(song => song.Name), file => file.FullName);

            foreach (var item in onlyInInput)
            {
                console.Out.WriteLine($"{type,12}: {item.Name}");
            }

            foreach (var item in onlyInFolder)
            {
                console.Out.WriteLine($"folder      : {item.FullName}");
            }
        }
    }
}
