// -----------------------------------------------------------------------
// <copyright file="Module.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Ninject.Extensions.ITunes.Editor
{
    using global::ITunes.Editor;

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
            this.Bind<ISongsProvider>().To<global::ITunes.Editor.IPod.IPodSongsProvider>().Named("ipod");
            this.Bind<ISongsProvider>().To<global::ITunes.Editor.PList.PListSongsProvider>().Named("plist");
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
            {
                this.Bind<ISongsProvider>().To<global::ITunes.Editor.ITunesLib.ITunesSongsProvider>().Named("itunes");
            }

            // Tags
            this.Bind<ITagProvider>().To<TagLibTagProvider>().Named("taglib");
            this.Bind<ITagProvider>().To<global::ITunes.Editor.MediaInfo.MediaInfoTagProvider>().Named("mediainfo");

            // Composers
            this.Bind<IComposerProvider>().To<ApraAmcosComposerProvider>().Named("apra_amcos");

            // Lyrics
            this.Bind<ILyricsProvider>().To<global::ITunes.Editor.Lyrics.Wikia.WikiaLyricsProvider>().Named("wikia");
            this.Bind<ILyricsProvider>().To<global::ITunes.Editor.ChartLyrics.ChartLyricsProvider>().Named("chart");

            // Services
            this.Bind<IUpdateComposerService>().To<UpdateComposerService>();
            this.Bind<IUpdateLyricsService>().To<UpdateLyricsService>();
        }
    }
}
