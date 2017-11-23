// -----------------------------------------------------------------------
// <copyright file="Module.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Ninject.Extensions.ITunes.Editor
{
    using global::ITunes.Editor;
    using global::ITunes.Editor.IPod;
    using global::ITunes.Editor.Lyrics.Wikia;
    using global::ITunes.Editor.PList;

    /// <summary>
    /// The Ninject module.
    /// </summary>
    public class Module : Modules.NinjectModule
    {
        /// <inheritdoc />
        public override void Load()
        {
            // Songs
            this.Bind<ISongLoader>().To<FolderSongLoader>().Named("folder");
            this.Bind<ISongLoader>().To<IPodSongLoader>().Named("ipod");
            this.Bind<ISongLoader>().To<PListSongLoader>().Named("plist");

            // Composers
            this.Bind<IComposerProvider>().To<ApraAmcosComposerProvider>().Named("apra_amcos");

            // Lyrics
            this.Bind<ILyricsProvider>().To<WikiaLyricsProvider>().Named("wikia");

            // Services
            this.Bind<IUpdateComposerService>().To<UpdateComposerService>();
            this.Bind<IUpdateLyricsService>().To<UpdateLyricsService>();
        }
    }
}
