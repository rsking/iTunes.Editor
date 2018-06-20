// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Reviewed.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileHeaderFileNameDocumentationMustMatchTypeName", Justification = "Reviewed.")]

namespace ITunes.Editor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using McMaster.Extensions.CommandLineUtils;
    using Microsoft.Extensions.Configuration;
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
        private const string AppSettings = "appsettings.json";

        /// <summary>
        /// Initializes a new instance of the <see cref="Program"/> class.
        /// </summary>
        public Program()
        {
            var builder = new ConfigurationBuilder().SetBasePath(System.IO.Directory.GetCurrentDirectory());
            if (System.IO.File.Exists(AppSettings))
            {
                builder = builder.AddJsonFile(AppSettings);
            }

            this.Kernel.Bind<IConfiguration>().ToConstant(builder.Build());
        }

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
