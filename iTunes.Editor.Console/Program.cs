// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    using System;
    using System.Reflection;
    using Ninject;

    /// <summary>
    /// The program class.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// The Ninject kernel.
        /// </summary>
        private IKernel kernel;

        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        /// <returns>The return code.</returns>
        private static int Main(string[] args)
        {
            var app = new Microsoft.Extensions.CommandLineUtils.CommandLineApplication();
            app.HelpOption("-h|--help");
            app.VersionOption(
                "-v|--version",
                () => $"{typeof(Program).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product} {typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}",
                () => $"{typeof(Program).Assembly.GetCustomAttribute<AssemblyProductAttribute>().Product}{Environment.NewLine}{typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}");

            app.Command("list", configuration =>
            {
                configuration.Description = "Lists the files from the specified input";
                configuration.HelpOption("-h|--help");
                var inputArgument = configuration.Argument("input", "The input");
                var typeOption = configuration.Option("-t|--type <type>", "The type of input", Microsoft.Extensions.CommandLineUtils.CommandOptionType.SingleValue);

                configuration.OnExecute(async () =>
                {
                    var program = new Program();
                    program.Configure();

                    var songLoader = program.kernel.Get<ISongLoader>(typeOption.Value());

                    foreach (var song in await songLoader.GetTagInformationAsync(inputArgument.Value).ConfigureAwait(false))
                    {
                        Console.WriteLine(song.Name);
                    }

                    return 0;
                });
            });

            return app.Execute(args);
        }

        /// <summary>
        /// Configurs this instance.
        /// </summary>
        private void Configure()
        {
            this.kernel = new StandardKernel();
        }
    }
}
