// -----------------------------------------------------------------------
// <copyright file="Module.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Ninject.Extensions.ITunes.Editor
{
    using global::ITunes.Editor;
    using global::ITunes.Editor.IPod;
    using global::ITunes.Editor.ITunesLib;
    using global::ITunes.Editor.Lyrics.Wikia;
    using global::ITunes.Editor.MediaInfo;
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
            this.Bind<ISongsProvider>().To<FolderSongsProvider>().Named("folder");
            this.Bind<ISongsProvider>().To<IPodSongsProvider>().Named("ipod");
            this.Bind<ISongsProvider>().To<PListSongsProvider>().Named("plist");
            this.Bind<ISongsProvider>().To<ITunesSongsProvider>().Named("itunes");

            // Tags
            this.Bind<ITagProvider>().To<TagLibTagProvider>().Named("taglib");
            this.Bind<ITagProvider>().To<MediaInfoTagProvider>().Named("mediainfo");

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
