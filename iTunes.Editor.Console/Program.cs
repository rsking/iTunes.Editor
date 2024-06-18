// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.CommandLine;
using System.CommandLine.Hosting;
using ITunes.Editor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

const string lyricsCommand = "lyrics";
const string updateCommand = "update";
const string listCommand = "list";
const string fileCommand = "file";
const string allCommand = "all";
const string composerCommand = "composer";
const string tempoCommand = "tempo";
const string checkCommand = "check";

var inputArgument = new CliArgument<FileSystemInfo>("input") { Description = "The input", Arity = ArgumentArity.ZeroOrOne }.AcceptExistingOnly();
var typeOption = new CliOption<string>("-t", "--type") { DefaultValueFactory = _ => DefaultType, Description = "The type of input" };
var artistArgument = new CliArgument<string>("artist") { Description = "The artist" };
var songArgument = new CliArgument<string>("song") { Description = "The song" };

var rootCommand = new CliRootCommand
{
    CreateListCommand(inputArgument, typeOption),
    CreateComposerCommand(artistArgument, songArgument),
    CreateLyricsCommand(artistArgument, songArgument),
    CreateMediaInfoCommand(),
    CreateUpdateCommand(inputArgument, typeOption),
    CreateCheckCommand(inputArgument, typeOption),
};

var configuration = new CliConfiguration(rootCommand)
    .UseHost(Host.CreateDefaultBuilder, configureHost => configureHost
        .UseDefaultITunes()
        .ConfigureServices(services =>
        {
            _ = services.Configure<InvocationLifetimeOptions>(options => options.SuppressStatusMessages = true);
            _ = services.AddTransient<IConfigurator<ITunes.Editor.ITunesLib.ITunesSongsProvider>, NullConfigurator<ITunes.Editor.ITunesLib.ITunesSongsProvider>>();
        }));

return await configuration
    .InvokeAsync(args.Select(Environment.ExpandEnvironmentVariables).ToArray())
    .ConfigureAwait(false);

static CliOption<string> CreateProviderOption(string defaultValue)
{
    return new("-p", "--provider") { DefaultValueFactory = _ => defaultValue, Description = "The type of provider" };
}

static CliCommand CreateProviderListCommand<T>()
{
    var command = new CliCommand("list", "Lists the providers");
    command.SetAction((parseResult, _) =>
    {
        var names = parseResult.GetHost().Services.GetRequiredService<Neleus.DependencyInjection.Extensions.IServiceByNameFactory<T>>().GetNames();
        foreach (var name in names)
        {
            parseResult.Configuration.Output.WriteLine(name);
        }

        return Task.CompletedTask;
    });

    return command;
}

static CliCommand CreateListCommand(CliArgument<FileSystemInfo> inputArgument, CliOption<string> typeOption)
{
    var propertiesOptions = new CliOption<string[]>("--property", "-") { Description = "A property to set on the input provider", Arity = ArgumentArity.ZeroOrMore };
    var command = new CliCommand(listCommand, "Lists the files from the specified input")
    {
        inputArgument,
        typeOption,
        propertiesOptions,
    };

    command.SetAction((parseResult, cancellationToken) =>
    {
        var host = parseResult.GetHost();
        var input = parseResult.GetValue(inputArgument)!;
        var type = parseResult.GetValue(typeOption)!;
        var properties = parseResult.GetValue(propertiesOptions);
        return List(host.Services, input, type, cancellationToken, properties);
    });
    return command;
}

static CliCommand CreateComposerCommand(CliArgument<string> artistArgument, CliArgument<string> songArgument)
{
    var providerOption = CreateProviderOption(DefaultComposerProvider);
    var command = new CliCommand(composerCommand, "Gets the composers for a specific song/artist")
    {
        artistArgument,
        songArgument,
        providerOption,
    };

    command.SetAction((parseResult, cancellationToken) => Composer(
        parseResult.GetHost().Services,
        parseResult.GetValue(artistArgument)!,
        parseResult.GetValue(songArgument)!,
        parseResult.GetValue(providerOption)!,
        cancellationToken));

    return command;
}

static CliCommand CreateLyricsCommand(CliArgument<string> artistArgument, CliArgument<string> songArgument)
{
    var providerOption = CreateProviderOption(DefaultLyricProvider);

    var command = new CliCommand(lyricsCommand, "Gets the lyrics for a specific song/artist")
    {
        artistArgument,
        songArgument,
        providerOption,
        CreateProviderListCommand<ILyricsProvider>(),
    };

    command.SetAction((parseResult, cancellationToken) =>
    {
        var host = parseResult.GetHost();
        var serviceProvider = host.Services;

        return Lyrics(
            serviceProvider.GetRequiredService<ILyricsProvider>(parseResult.GetValue(providerOption)!),
            parseResult.GetValue(artistArgument)!,
            parseResult.GetValue(songArgument)!,
            serviceProvider.GetRequiredService<ILogger<Program>>(),
            cancellationToken);
    });

    return command;
}

static CliCommand CreateMediaInfoCommand()
{
    var fileArgument = new CliArgument<string>("file") { Description = "The file to get information for" };
    var command = new CliCommand(nameof(ITunes.Editor.MediaInfo).ToLower(System.Globalization.CultureInfo.CurrentCulture), "Gets the media info for a specific file")
    {
        fileArgument,
    };

    command.SetAction((parseResult, cancellationToken) => MediaInfo(
        parseResult.GetHost().Services,
        parseResult.GetValue(fileArgument)!,
        cancellationToken));

    return command;
}

static CliCommand CreateCheckCommand(CliArgument<FileSystemInfo> inputArgument, CliOption<string> typeOption)
{
    return new CliCommand(checkCommand, "Checks a specific file or list")
    {
        CreateCheckFileCommand(inputArgument),
        CreateCheckListCommand(inputArgument, typeOption),
    };

    static CliCommand CreateCheckListCommand(CliArgument<FileSystemInfo> inputArgument, CliOption<string> typeOption)
    {
        var command = new CliCommand(listCommand, "Checks a specific list")
        {
            inputArgument,
            typeOption,
        };

        command.SetAction((parseResult, cancellationToken) => CheckList(
            parseResult.GetHost().Services,
            parseResult.GetValue(inputArgument)!,
            parseResult.GetValue(typeOption)!,
            cancellationToken));

        return command;
    }

    static CliCommand CreateCheckFileCommand(CliArgument<FileSystemInfo> inputArgument)
    {
        var command = new CliCommand(fileCommand, "Checks a specific file")
        {
            inputArgument,
        };

        command.SetAction((parseResult, cancellationToken) => CheckFile(
            parseResult.GetHost().Services,
            parseResult.GetValue(inputArgument)!,
            cancellationToken));

        return command;
    }
}

static CliCommand CreateUpdateCommand(CliArgument<FileSystemInfo> inputArgument, CliOption<string> typeOption)
{
    var forceOption = new CliOption<bool>("-f", "--force") { DefaultValueFactory = _ => DefaultForce, Description = "Whether to force the update" };
    return new CliCommand(updateCommand, "Updates a specific file or list")
    {
        CreateUpdateFileCommand(forceOption),
        CreateUpdateListCommand(inputArgument, typeOption, forceOption),
    };

    static CliCommand CreateUpdateFileCommand(CliOption<bool> forceOption)
    {
        var fileArgument = new CliArgument<FileInfo>("file") { Description = "The file" };
        return new CliCommand(fileCommand, "Updates a specific file")
        {
            CreateUpdateComposerFileCommand(fileArgument, forceOption),
            CreateUpdateLyricsFileCommand(fileArgument, forceOption),
            CreateUpdateTempoFileCommand(fileArgument, forceOption),
            CreateUpdateAllFileCommand(fileArgument, forceOption),
        };

        static CliCommand CreateUpdateComposerFileCommand(CliArgument<FileInfo> fileArgument, CliOption<bool> forceOption)
        {
            var command = new CliCommand(composerCommand, "Updates the composer in the specific file")
            {
                fileArgument,
                forceOption,
            };

            command.SetAction((parseResult, cancellationToken) => UpdateComposerFile(
                parseResult.GetHost().Services,
                parseResult.GetValue(fileArgument)!,
                parseResult.GetValue(forceOption),
                cancellationToken));

            return command;
        }

        static CliCommand CreateUpdateLyricsFileCommand(CliArgument<FileInfo> fileArgument, CliOption<bool> forceOption)
        {
            var command = new CliCommand(lyricsCommand, "Updates the lyrics in the specific file")
            {
                fileArgument,
                forceOption,
            };

            command.SetAction((parseResult, cancellationToken) => UpdateLyricsFile(
                parseResult.GetHost().Services,
                parseResult.GetValue(fileArgument)!,
                parseResult.GetValue(forceOption),
                cancellationToken));

            return command;
        }

        static CliCommand CreateUpdateTempoFileCommand(CliArgument<FileInfo> fileArgument, CliOption<bool> forceOption)
        {
            var command = new CliCommand(tempoCommand, "Updates the tempo in the specific file")
            {
                fileArgument,
                forceOption,
            };

            command.SetAction((parseResult, cancellationToken) => UpdateTempoFile(
                parseResult.GetHost().Services,
                parseResult.GetValue(fileArgument)!,
                parseResult.GetValue(forceOption),
                cancellationToken));

            return command;
        }

        static CliCommand CreateUpdateAllFileCommand(CliArgument<FileInfo> fileArgument, CliOption<bool> forceOption)
        {
            var command = new CliCommand(allCommand, "Updates the specific file using all the updaters")
            {
                fileArgument,
                forceOption,
            };

            command.SetAction((parseResult, cancellationToken) => UpdateAllFile(
                parseResult.GetHost().Services,
                parseResult.GetValue(fileArgument)!,
                parseResult.GetValue(forceOption),
                cancellationToken));

            return command;
        }
    }

    static CliCommand CreateUpdateListCommand(CliArgument<FileSystemInfo> inputArgument, CliOption<string> typeOption, CliOption<bool> forceOption)
    {
        return new CliCommand(listCommand, "Updates a specific list")
        {
            CreateUpdateComposerListCommand(inputArgument, typeOption, forceOption),
            CreateUpdateLyricsListCommand(inputArgument, typeOption, forceOption),
            CreateUpdateTempoListCommand(inputArgument, typeOption, forceOption),
            CreateUpdateAllListCommand(inputArgument, typeOption, forceOption),
        };

        static CliCommand CreateUpdateComposerListCommand(CliArgument<FileSystemInfo> inputArgument, CliOption<string> typeOption, CliOption<bool> forceOption)
        {
            var command = new CliCommand(composerCommand, "Updates the composer in the specific list")
            {
                inputArgument,
                typeOption,
                forceOption,
            };

            command.SetAction((parseResult, cancellationToken) => UpdateComposerList(
                parseResult.GetHost().Services,
                parseResult.GetValue(inputArgument)!,
                parseResult.GetValue(typeOption)!,
                parseResult.GetValue(forceOption),
                cancellationToken));

            return command;
        }

        static CliCommand CreateUpdateLyricsListCommand(CliArgument<FileSystemInfo> inputArgument, CliOption<string> typeOption, CliOption<bool> forceOption)
        {
            var command = new CliCommand(lyricsCommand, "Updates the lyrics in the specific list")
            {
                inputArgument,
                typeOption,
                forceOption,
            };

            command.SetAction((parseResult, cancellationToken) => UpdateLyricsList(
                parseResult.GetHost().Services,
                parseResult.GetValue(inputArgument)!,
                parseResult.GetValue(typeOption)!,
                parseResult.GetValue(forceOption),
                cancellationToken));

            return command;
        }

        static CliCommand CreateUpdateTempoListCommand(CliArgument<FileSystemInfo> inputArgument, CliOption<string> typeOption, CliOption<bool> forceOption)
        {
            var command = new CliCommand(tempoCommand, "Updates the tempo in the specific list")
            {
                inputArgument,
                typeOption,
                forceOption,
            };

            command.SetAction((parseResult, cancellationToken) => UpdateTempoList(
                parseResult.GetHost().Services,
                parseResult.GetValue(inputArgument)!,
                parseResult.GetValue(typeOption)!,
                parseResult.GetValue(forceOption),
                cancellationToken));

            return command;
        }

        static CliCommand CreateUpdateAllListCommand(CliArgument<FileSystemInfo> inputArgument, CliOption<string> typeOption, CliOption<bool> forceOption)
        {
            var command = new CliCommand(allCommand, "Updates the specific list using all the updaters")
            {
                inputArgument,
                typeOption,
                forceOption,
            };

            command.SetAction((parseResult, cancellationToken) => UpdateAllList(
                parseResult.GetHost().Services,
                parseResult.GetValue(inputArgument)!,
                parseResult.GetValue(typeOption)!,
                parseResult.GetValue(forceOption),
                cancellationToken));

            return command;
        }
    }
}

/// <summary>
/// The program class.
/// </summary>
internal sealed partial class Program
{
    private const bool DefaultForce = false;

    private const string DefaultType = "plist";

    private const string DefaultLyricProvider = "wikia";

    private const string DefaultComposerProvider = "apra_amcos";

    private Program()
    {
    }

    private static async Task List(IServiceProvider serviceProvider, FileSystemInfo input, string type, CancellationToken cancellationToken, params string[]? property)
    {
        //// var options = new System.IO.EnumerationOptions { AttributesToSkip = System.IO.FileAttributes.Hidden, RecurseSubdirectories = true };
        //// await System.IO.File.WriteAllLinesAsync("C:\\Temp\\folder.txt", System.IO.Directory.EnumerateFiles("D:\\Users\\rskin\\OneDrive\\Music\\iTunes\\iTunes Media", "*", options).OrderBy(file => file), System.Text.Encoding.Default).ConfigureAwait(false);

        var songsProvider = serviceProvider
            .GetRequiredService<ISongsProvider>(type)
            .SetProperties(property)
            .SetPath(input);
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        //// await System.IO.File.WriteAllLinesAsync("C:\\Temp\\itunes.txt", songsProvider.GetTagInformationAsync(cancellationToken).Where(song => song.Name?.Length > 0).OrderBy(song => song.Name).Select(song => song.Name).ToEnumerable()).ConfigureAwait(false);
        //// using var stream = new System.IO.StreamWriter("C:\\Temp\\itunes.txt");
        await foreach (var song in songsProvider.GetTagInformationAsync(cancellationToken).ConfigureAwait(false))
        {
            //// await stream.WriteLineAsync(song.Name).ConfigureAwait(false);
            logger.LogInformation(ITunes.Editor.Console.Properties.Resources.ListLog, song.Performers.ToJoinedString(), song.Title, song.Name, File.Exists(song.Name));
        }
    }

    private static async Task Composer(IServiceProvider serviceProvider, string artist, string song, string provider, CancellationToken cancellationToken)
    {
        var composerProvider = serviceProvider.GetRequiredService<IComposerProvider>(provider);
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        var performers = artist.FromJoinedString().ToArray();
        var songInformation = new SongInformation(song)
        {
            Performers = performers,
            SortPerformers = performers,
        };

        await foreach (var composer in composerProvider
            .GetComposersAsync(songInformation, cancellationToken)
            .ConfigureAwait(false))
        {
            logger.LogInformation(ITunes.Editor.Console.Properties.Resources.ComposerLog, composer);
        }
    }

    private static async Task Lyrics(ILyricsProvider lyricsProvider, string artist, string song, ILogger logger, CancellationToken cancellationToken)
    {
        var performers = artist.FromJoinedString().ToArray();
        var songInformation = new SongInformation(song)
        {
            Performers = performers,
            SortPerformers = performers,
        };

        var lyrics = await lyricsProvider
           .GetLyricsAsync(songInformation, cancellationToken)
           .ConfigureAwait(false);
        logger.LogInformation(ITunes.Editor.Console.Properties.Resources.LyricsLog, lyrics);
    }

    private static async Task MediaInfo(IServiceProvider serviceProvider, string file, CancellationToken cancellationToken)
    {
        var mediaInfoTagProvider = new ITunes.Editor.MediaInfo.MediaInfoTagProvider { File = file };
        var mediaInfo = await mediaInfoTagProvider.GetTagAsync(cancellationToken).ConfigureAwait(false);
        if (mediaInfo is null)
        {
            return;
        }

        serviceProvider.GetRequiredService<ILogger<Program>>().LogInformation(ITunes.Editor.Console.Properties.Resources.MediaInfoLog, mediaInfo.JoinedPerformers, mediaInfo.Title);
    }

    private static Task UpdateComposerFile(IServiceProvider serviceProvider, FileInfo file, bool force, CancellationToken cancellationToken) =>
        UpdateFile<IUpdateComposerService>(serviceProvider, file, force, cancellationToken);

    private static Task UpdateLyricsFile(IServiceProvider serviceProvider, FileInfo file, bool force, CancellationToken cancellationToken) =>
        UpdateFile<IUpdateLyricsService>(serviceProvider, file, force, cancellationToken);

    private static Task UpdateTempoFile(IServiceProvider serviceProvider, FileInfo file, bool force, CancellationToken cancellationToken) =>
        UpdateFile<IUpdateTempoService>(serviceProvider, file, force, cancellationToken);

    private static async Task UpdateAllFile(IServiceProvider serviceProvider, FileInfo file, bool force, CancellationToken cancellationToken)
    {
        var songInformation = await SongInformation.FromFileAsync(file.FullName, cancellationToken).ConfigureAwait(false);
        _ = await serviceProvider.GetRequiredService<IUpdateComposerService>().UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
        _ = await serviceProvider.GetRequiredService<IUpdateLyricsService>().UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
    }

    private static async Task UpdateFile<T>(IServiceProvider serviceProvider, FileInfo file, bool force, CancellationToken cancellationToken)
        where T : IUpdateService
    {
        var service = serviceProvider.GetRequiredService<T>();
        var song = await SongInformation.FromFileAsync(file.FullName, cancellationToken).ConfigureAwait(false);
        _ = await service.UpdateAsync(song, force, cancellationToken).ConfigureAwait(false);
    }

    private static async Task UpdateList(IServiceProvider serviceProvider, FileSystemInfo input, string type, bool force, Func<SongInformation, bool, CancellationToken, ValueTask<SongInformation>> updateFunction, CancellationToken cancellationToken)
    {
        var songsProvider = serviceProvider
            .GetRequiredService<ISongsProvider>(type)
            .SetPath(input);

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        await foreach (var song in songsProvider
            .GetTagInformationAsync(cancellationToken)
            .Where(_ => _.Name is not null && File.Exists(_.Name))
            .ConfigureAwait(false))
        {
            // check the location
            var mediaType = song.GetMediaKind();
            switch (mediaType)
            {
                case MediaKind.Song:
                    logger.LogInformation(ITunes.Editor.Console.Properties.Resources.Processing, song);
                    break;
                default:
                    logger.LogInformation(ITunes.Editor.Console.Properties.Resources.Skipping, song, mediaType);
                    continue;
            }

            _ = await updateFunction(song, force, cancellationToken).ConfigureAwait(false);
        }
    }

    private static Task UpdateComposerList(IServiceProvider serviceProvider, FileSystemInfo input, string type, bool force, CancellationToken cancellationToken) => UpdateList(serviceProvider, input, type, force, serviceProvider.GetRequiredService<IUpdateComposerService>().UpdateAsync, cancellationToken);

    private static Task UpdateLyricsList(IServiceProvider serviceProvider, FileSystemInfo input, string type, bool force, CancellationToken cancellationToken) => UpdateList(serviceProvider, input, type, force, serviceProvider.GetRequiredService<IUpdateLyricsService>().UpdateAsync, cancellationToken);

    private static Task UpdateTempoList(IServiceProvider serviceProvider, FileSystemInfo input, string type, bool force, CancellationToken cancellationToken) => UpdateList(serviceProvider, input, type, force, serviceProvider.GetRequiredService<IUpdateTempoService>().UpdateAsync, cancellationToken);

    private static Task UpdateAllList(IServiceProvider serviceProvider, FileSystemInfo input, string type, bool force, CancellationToken cancellationToken)
    {
        var composerService = default(IUpdateComposerService);
        var lyricsService = default(IUpdateLyricsService);
        return UpdateList(
            serviceProvider,
            input,
            type,
            force,
            Update,
            cancellationToken);

        async ValueTask<SongInformation> Update(SongInformation songInformation, bool force, CancellationToken cancellationToken)
        {
            composerService ??= serviceProvider.GetRequiredService<IUpdateComposerService>();
            lyricsService ??= serviceProvider.GetRequiredService<IUpdateLyricsService>();

            songInformation = await composerService.UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
            return await lyricsService.UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task CheckList(IServiceProvider serviceProvider, FileSystemInfo input, string type, CancellationToken cancellationToken = default)
    {
        var songsProvider = serviceProvider
            .GetRequiredService<ISongsProvider>(type)
            .SetPath(input);

        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();

        await foreach (var song in songsProvider
            .GetTagInformationAsync(cancellationToken)
            .Where(_ => _.Name is not null && File.Exists(_.Name))
            .ConfigureAwait(false))
        {
            CheckFileImpl(logger, song);
        }
    }

    private static async Task CheckFile(IServiceProvider serviceProvider, FileSystemInfo file, CancellationToken cancellationToken = default)
    {
        var songInformation = await SongInformation.FromFileAsync(file.FullName, cancellationToken).ConfigureAwait(false);
        CheckFileImpl(serviceProvider.GetRequiredService<ILogger<Program>>(), songInformation);
    }

    private static void CheckFileImpl(ILogger logger, SongInformation song)
    {
        // check the location
        var mediaType = song.GetMediaKind();
        switch (mediaType)
        {
            case MediaKind.Song:
                logger.LogInformation(ITunes.Editor.Console.Properties.Resources.Processing, song);
                break;
            default:
                logger.LogInformation(ITunes.Editor.Console.Properties.Resources.Skipping, song, mediaType);
                return;
        }

        (var genre, _) = GetGenre(song.Genre);
        if (genre is null)
        {
            logger.LogWarning("Failed to match {Genre} to an approvied genre", song.Genre);
        }

        static (Genre? Genre, int Level) GetGenre(string? genre)
        {
            return genre is null
                ? default
                : GetGenre(Genres.Music.SubGenres, genre, 1);

            static (Genre? Genre, int Level) GetGenre(IReadOnlyCollection<Genre> genres, string name, int level)
            {
                foreach (var genre in genres)
                {
                    if (string.Equals(genre.Name, name, StringComparison.Ordinal))
                    {
                        return (genre, level);
                    }

                    var subGenre = GetGenre(genre.SubGenres, name, level + 1);
                    if (subGenre.Genre is not null)
                    {
                        return subGenre;
                    }
                }

                return default;
            }
        }
    }
}