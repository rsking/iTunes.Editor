// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor;

using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Hosting;
using System.CommandLine.Parsing;
using System.CommandLine.Rendering;

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
        const string updateCommand = "update";
        const string listCommand = "list";
        const string fileCommand = "file";
        const string allCommand = "all";
        const string composerCommand = "composer";
        const string tempoCommand = "tempo";
        const string checkCommand = "check";

        var inputArgument = new Argument<FileSystemInfo>("input") { Description = "The input", Arity = ArgumentArity.ZeroOrOne }.ExistingOnly();
        var typeOption = new Option<string>(new[] { "-t", "--type" }, () => DefaultType, "The type of input");
        var artistArgument = new Argument<string>("artist") { Description = "The artist" };
        var songArgument = new Argument<string>("song") { Description = "The song" };

        var rootCommand = new RootCommand
        {
            CreateListCommand(inputArgument, typeOption),
            CreateComposerCommand(artistArgument, songArgument),
            CreateLyricsCommand(artistArgument, songArgument),
            CreateMediaInfoCommand(),
            CreateUpdateCommand(inputArgument, typeOption),
            CreateCheckCommand(inputArgument, typeOption),
        };

        var builder = new CommandLineBuilder(rootCommand)
            .UseDefaults()
            .UseAnsiTerminalWhenAvailable()
            .UseHost(Host.CreateDefaultBuilder, configureHost => configureHost
                .UseDefaultITunes()
                .ConfigureServices(services =>
                {
                    _ = services.Configure<InvocationLifetimeOptions>(options => options.SuppressStatusMessages = true);
                    _ = services.AddTransient<IConfigurator<ITunesLib.ITunesSongsProvider>, NullConfigurator<ITunesLib.ITunesSongsProvider>>();
                }));

        return builder
            .Build()
            .InvokeAsync(args.Select(Environment.ExpandEnvironmentVariables).ToArray());

        static Option<string> CreateProviderOption(string defaultValue)
        {
            return new(new[] { "-p", "--provider" }, () => defaultValue, "The type of provider");
        }

        static Command CreateListCommand(Argument<FileSystemInfo> inputArgument, Option<string> typeOption)
        {
            var propertiesOptions = new Option<string[]>("--property", "A property to set on the input provider")
            {
                Arity = ArgumentArity.ZeroOrMore,
            };
            propertiesOptions.AddAlias("-");
            var command = new Command(listCommand, "Lists the files from the specified input")
            {
                inputArgument,
                typeOption,
                propertiesOptions,
            };

            command.SetHandler(List, Bind.FromServiceProvider<IHost>(), inputArgument, typeOption, Bind.FromServiceProvider<CancellationToken>(), propertiesOptions);
            return command;
        }

        static Command CreateComposerCommand(Argument<string> artistArgument, Argument<string> songArgument)
        {
            var providerOption = CreateProviderOption(DefaultComposerProvider);
            var command = new Command(composerCommand, "Gets the composers for a specific song/artist")
            {
                artistArgument,
                songArgument,
                providerOption,
            };

            command.SetHandler(Composer, Bind.FromServiceProvider<IHost>(), artistArgument, songArgument, providerOption, Bind.FromServiceProvider<CancellationToken>());
            return command;
        }

        static Command CreateLyricsCommand(Argument<string> artistArgument, Argument<string> songArgument)
        {
            var lyricProviderOption = CreateProviderOption(DefaultLyricProvider);
            var command = new Command(lyricsCommand, "Gets the lyrics for a specific song/artist")
            {
                artistArgument,
                songArgument,
                lyricProviderOption,
            };

            command.SetHandler(Lyrics, Bind.FromServiceProvider<IHost>(), artistArgument, songArgument, lyricProviderOption, Bind.FromServiceProvider<CancellationToken>());
            return command;
        }

        static Command CreateMediaInfoCommand()
        {
            var fileArgument = new Argument<string>("file") { Description = "The file to get information for" };
            var command = new Command(nameof(Editor.MediaInfo).ToLower(System.Globalization.CultureInfo.CurrentCulture), "Gets the media info for a specific file")
            {
                fileArgument,
            };

            command.SetHandler(MediaInfo, Bind.FromServiceProvider<IHost>(), fileArgument, Bind.FromServiceProvider<CancellationToken>());
            return command;
        }

        static Command CreateCheckCommand(Argument<FileSystemInfo> inputArgument, Option<string> typeOption)
        {
            return new Command(checkCommand, "Checks a specific file or list")
            {
                CreateCheckFileCommand(inputArgument),
                CreateCheckListCommand(inputArgument, typeOption),
            };

            static Command CreateCheckListCommand(Argument<FileSystemInfo> inputArgument, Option<string> typeOption)
            {
                var command = new Command(listCommand, "Checks a specific list")
            {
                inputArgument,
                typeOption,
            };

                command.SetHandler(CheckList, Bind.FromServiceProvider<IHost>(), inputArgument, typeOption, Bind.FromServiceProvider<CancellationToken>());
                return command;
            }

            static Command CreateCheckFileCommand(Argument<FileSystemInfo> inputArgument)
            {
                var command = new Command(fileCommand, "Checks a specific file")
            {
                inputArgument,
            };

                command.SetHandler(CheckFile, Bind.FromServiceProvider<IHost>(), inputArgument, Bind.FromServiceProvider<CancellationToken>());
                return command;
            }
        }

        static Command CreateUpdateCommand(Argument<FileSystemInfo> inputArgument, Option<string> typeOption)
        {
            var forceOption = new Option<bool>(new[] { "-f", "--force" }, () => DefaultForce, "Whether to force the update");
            return new Command(updateCommand, "Updates a specific file or list")
            {
                CreateUpdateFileCommand(forceOption),
                CreateUpdateListCommand(inputArgument, typeOption, forceOption),
            };

            static Command CreateUpdateFileCommand(Option<bool> forceOption)
            {
                var fileArgument = new Argument<FileInfo>("file") { Description = "The file" };
                return new Command(fileCommand, "Updates a specific file")
                {
                    CreateUpdateComposerFileCommand(fileArgument, forceOption),
                    CreateUpdateLyricsFileCommand(fileArgument, forceOption),
                    CreateUpdateTempoFileCommand(fileArgument, forceOption),
                    CreateUpdateAllFileCommand(fileArgument, forceOption),
                };

                static Command CreateUpdateComposerFileCommand(Argument<FileInfo> fileArgument, Option<bool> forceOption)
                {
                    var command = new Command(composerCommand, "Updates the composer in the specific file")
                    {
                        fileArgument,
                        forceOption,
                    };

                    command.SetHandler(UpdateComposerFile, Bind.FromServiceProvider<IHost>(), fileArgument, forceOption, Bind.FromServiceProvider<CancellationToken>());
                    return command;
                }

                static Command CreateUpdateLyricsFileCommand(Argument<FileInfo> fileArgument, Option<bool> forceOption)
                {
                    var command = new Command(lyricsCommand, "Updates the lyrics in the specific file")
                    {
                        fileArgument,
                        forceOption,
                    };

                    command.SetHandler(UpdateLyricsFile, Bind.FromServiceProvider<IHost>(), fileArgument, forceOption, Bind.FromServiceProvider<CancellationToken>());
                    return command;
                }

                static Command CreateUpdateTempoFileCommand(Argument<FileInfo> fileArgument, Option<bool> forceOption)
                {
                    var command = new Command(tempoCommand, "Updates the tempo in the specific file")
                    {
                        fileArgument,
                        forceOption,
                    };

                    command.SetHandler(UpdateTempoFile, Bind.FromServiceProvider<IHost>(), fileArgument, forceOption, Bind.FromServiceProvider<CancellationToken>());
                    return command;
                }

                static Command CreateUpdateAllFileCommand(Argument<FileInfo> fileArgument, Option<bool> forceOption)
                {
                    var command = new Command(allCommand, "Updates the specific file using all the updaters")
                    {
                        fileArgument,
                        forceOption,
                    };

                    command.SetHandler(UpdateAllFile, Bind.FromServiceProvider<IHost>(), fileArgument, forceOption, Bind.FromServiceProvider<CancellationToken>());
                    return command;
                }
            }

            static Command CreateUpdateListCommand(Argument<FileSystemInfo> inputArgument, Option<string> typeOption, Option<bool> forceOption)
            {
                return new Command(listCommand, "Updates a specific list")
                {
                    CreateUpdateComposerListCommand(inputArgument, typeOption, forceOption),
                    CreateUpdateLyricsListCommand(inputArgument, typeOption, forceOption),
                    CreateUpdateTempoListCommand(inputArgument, typeOption, forceOption),
                    CreateUpdateAllListCommand(inputArgument, typeOption, forceOption),
                };

                static Command CreateUpdateComposerListCommand(Argument<FileSystemInfo> inputArgument, Option<string> typeOption, Option<bool> forceOption)
                {
                    var command = new Command(composerCommand, "Updates the composer in the specific list")
                    {
                        inputArgument,
                        typeOption,
                        forceOption,
                    };

                    command.SetHandler(UpdateComposerList, Bind.FromServiceProvider<IHost>(), inputArgument, typeOption, forceOption, Bind.FromServiceProvider<CancellationToken>());
                    return command;
                }

                static Command CreateUpdateLyricsListCommand(Argument<FileSystemInfo> inputArgument, Option<string> typeOption, Option<bool> forceOption)
                {
                    var command = new Command(lyricsCommand, "Updates the lyrics in the specific list")
                    {
                        inputArgument,
                        typeOption,
                        forceOption,
                    };

                    command.SetHandler(UpdateLyricsList, Bind.FromServiceProvider<IHost>(), inputArgument, typeOption, forceOption, Bind.FromServiceProvider<CancellationToken>());
                    return command;
                }

                static Command CreateUpdateTempoListCommand(Argument<FileSystemInfo> inputArgument, Option<string> typeOption, Option<bool> forceOption)
                {
                    var command = new Command(tempoCommand, "Updates the tempo in the specific list")
                    {
                        inputArgument,
                        typeOption,
                        forceOption,
                    };

                    command.SetHandler(UpdateTempoList, Bind.FromServiceProvider<IHost>(), inputArgument, typeOption, forceOption, Bind.FromServiceProvider<CancellationToken>());
                    return command;
                }

                static Command CreateUpdateAllListCommand(Argument<FileSystemInfo> inputArgument, Option<string> typeOption, Option<bool> forceOption)
                {
                    var command = new Command(allCommand, "Updates the specific list using all the updaters")
                    {
                        inputArgument,
                        typeOption,
                        forceOption,
                    };

                    command.SetHandler(UpdateAllList, Bind.FromServiceProvider<IHost>(), inputArgument, typeOption, forceOption, Bind.FromServiceProvider<CancellationToken>());
                    return command;
                }
            }
        }
    }

    private static async Task List(IHost host, FileSystemInfo input, string type, CancellationToken cancellationToken, params string[] property)
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
            logger.LogInformation(Console.Properties.Resources.ListLog, song.Performers.ToJoinedString(), song.Title, song.Name, File.Exists(song.Name));
        }
    }

    private static async Task Composer(IHost host, string artist, string song, string provider, CancellationToken cancellationToken)
    {
        var composerProvider = host.Services.GetRequiredService<IComposerProvider>(provider);
        var logger = host.Services.GetRequiredService<ILogger<Program>>();
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
            logger.LogInformation(Console.Properties.Resources.ComposerLog, composer);
        }
    }

    private static async Task Lyrics(IHost host, string artist, string song, string provider, CancellationToken cancellationToken)
    {
        var performers = artist.FromJoinedString().ToArray();
        var songInformation = new SongInformation(song)
        {
            Performers = performers,
            SortPerformers = performers,
        };

        var lyrics = await host.Services.GetRequiredService<ILyricsProvider>(provider)
           .GetLyricsAsync(songInformation, cancellationToken)
           .ConfigureAwait(false);
        host.Services.GetRequiredService<ILogger<Program>>().LogInformation(Console.Properties.Resources.LyricsLog, lyrics);
    }

    private static async Task MediaInfo(IHost host, string file, CancellationToken cancellationToken)
    {
        var mediaInfoTagProvider = new MediaInfo.MediaInfoTagProvider { File = file };
        var mediaInfo = await mediaInfoTagProvider.GetTagAsync(cancellationToken).ConfigureAwait(false);
        if (mediaInfo is null)
        {
            return;
        }

        host.Services.GetRequiredService<ILogger<Program>>().LogInformation(Console.Properties.Resources.MediaInfoLog, mediaInfo.JoinedPerformers, mediaInfo.Title);
    }

    private static Task UpdateComposerFile(IHost host, FileInfo file, bool force, CancellationToken cancellationToken) =>
        UpdateFile<IUpdateComposerService>(host, file, force, cancellationToken);

    private static Task UpdateLyricsFile(IHost host, FileInfo file, bool force, CancellationToken cancellationToken) =>
        UpdateFile<IUpdateLyricsService>(host, file, force, cancellationToken);

    private static Task UpdateTempoFile(IHost host, FileInfo file, bool force, CancellationToken cancellationToken) =>
        UpdateFile<IUpdateTempoService>(host, file, force, cancellationToken);

    private static async Task UpdateAllFile(IHost host, FileInfo file, bool force, CancellationToken cancellationToken)
    {
        var songInformation = await SongInformation.FromFileAsync(file.FullName, cancellationToken).ConfigureAwait(false);
        _ = await host.Services.GetRequiredService<IUpdateComposerService>().UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
        _ = await host.Services.GetRequiredService<IUpdateLyricsService>().UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
    }

    private static async Task UpdateFile<T>(IHost host, FileInfo file, bool force, CancellationToken cancellationToken)
        where T : IUpdateService
    {
        var service = host.Services.GetRequiredService<T>();
        var song = await SongInformation.FromFileAsync(file.FullName, cancellationToken).ConfigureAwait(false);
        _ = await service.UpdateAsync(song, force, cancellationToken).ConfigureAwait(false);
    }

    private static async Task UpdateList(IHost host, FileSystemInfo input, string type, bool force, Func<SongInformation, bool, CancellationToken, ValueTask<SongInformation>> updateFunction, CancellationToken cancellationToken)
    {
        var songsProvider = host.Services
            .GetRequiredService<ISongsProvider>(type)
            .SetPath(input);

        var logger = host.Services.GetRequiredService<ILogger<Program>>();

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
                    logger.LogInformation(Console.Properties.Resources.Processing, song);
                    break;
                default:
                    logger.LogInformation(Console.Properties.Resources.Skipping, song, mediaType);
                    continue;
            }

            _ = await updateFunction(song, force, cancellationToken).ConfigureAwait(false);
        }
    }

    private static Task UpdateComposerList(IHost host, FileSystemInfo input, string type, bool force, CancellationToken cancellationToken) => UpdateList(host, input, type, force, host.Services.GetRequiredService<IUpdateComposerService>().UpdateAsync, cancellationToken);

    private static Task UpdateLyricsList(IHost host, FileSystemInfo input, string type, bool force, CancellationToken cancellationToken) => UpdateList(host, input, type, force, host.Services.GetRequiredService<IUpdateLyricsService>().UpdateAsync, cancellationToken);

    private static Task UpdateTempoList(IHost host, FileSystemInfo input, string type, bool force, CancellationToken cancellationToken) => UpdateList(host, input, type, force, host.Services.GetRequiredService<IUpdateTempoService>().UpdateAsync, cancellationToken);

    private static Task UpdateAllList(IHost host, FileSystemInfo input, string type, bool force, CancellationToken cancellationToken)
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

        async ValueTask<SongInformation> Update(SongInformation songInformation, bool force, CancellationToken cancellationToken)
        {
            composerService ??= host.Services.GetRequiredService<IUpdateComposerService>();
            lyricsService ??= host.Services.GetRequiredService<IUpdateLyricsService>();

            songInformation = await composerService.UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
            return await lyricsService.UpdateAsync(songInformation, force, cancellationToken).ConfigureAwait(false);
        }
    }

    private static async Task CheckList(IHost host, FileSystemInfo input, string type, CancellationToken cancellationToken = default)
    {
        var songsProvider = host.Services
            .GetRequiredService<ISongsProvider>(type)
            .SetPath(input);

        var logger = host.Services.GetRequiredService<ILogger<Program>>();

        await foreach (var song in songsProvider
            .GetTagInformationAsync(cancellationToken)
            .Where(_ => _.Name is not null && File.Exists(_.Name))
            .ConfigureAwait(false))
        {
            CheckFileImpl(logger, song);
        }
    }

    private static async Task CheckFile(IHost host, FileSystemInfo file, CancellationToken cancellationToken = default)
    {
        var songInformation = await SongInformation.FromFileAsync(file.FullName, cancellationToken).ConfigureAwait(false);
        CheckFileImpl(host.Services.GetRequiredService<ILogger<Program>>(), songInformation);
    }

    private static void CheckFileImpl(ILogger logger, SongInformation song)
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
                return;
        }

        (var genre, var level) = GetGenre(song.Genre);
        if (genre is null)
        {
            logger.LogWarning("Failed to match {genre} to an approvied genre", song.Genre);
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
