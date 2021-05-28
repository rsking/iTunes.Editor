// <copyright file="ITunesConfigureViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.ViewModels
{
    /// <summary>
    /// The configure view model for <see cref="ITunesLib.ITunesSongsProvider"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "This is for iTunes")]
    public class ITunesConfigureViewModel : ViewModelBase, Models.IConfigure
    {
        private readonly ITunesLib.ITunesSongsProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ITunesConfigureViewModel"/> class.
        /// </summary>
        /// <param name="provider">The provider.</param>
        public ITunesConfigureViewModel(ITunesLib.ITunesSongsProvider provider) => this.provider = provider;

        /// <inheritdoc cref="ITunesLib.ITunesSongsProvider.Playlists" />
        public System.Collections.Generic.IEnumerable<string> Playlists => this.provider.Playlists;

        /// <inheritdoc cref="ITunesLib.ITunesSongsProvider.SelectedPlaylist" />
        public string? SelectedPlaylist
        {
            get => this.provider.SelectedPlaylist;
            set
            {
                var selectedPlaylist = this.provider.SelectedPlaylist;
                this.SetProperty(ref selectedPlaylist, value);
                this.provider.SelectedPlaylist = selectedPlaylist;
            }
        }

        /// <inheritdoc cref="ITunesLib.ITunesSongsProvider.UpdateMetadata" />
        public bool UpdateMetadata
        {
            get => this.provider.UpdateMetadata;
            set
            {
                var updateMetadata = this.provider.UpdateMetadata;
                this.SetProperty(ref updateMetadata, value);
                this.provider.UpdateMetadata = updateMetadata;
            }
        }

        /// <inheritdoc cref="ITunesLib.ITunesSongsProvider.SetAlbumArtist" />
        public bool SetAlbumArtist
        {
            get => this.provider.SetAlbumArtist;
            set
            {
                var setAlbumArtist = this.provider.SetAlbumArtist;
                this.SetProperty(ref setAlbumArtist, value);
                this.provider.SetAlbumArtist = setAlbumArtist;
            }
        }

        /// <inheritdoc cref="ITunesLib.ITunesSongsProvider.UpdateGrouping" />
        public bool UpdateGrouping
        {
            get => this.provider.UpdateGrouping;
            set
            {
                var updateGrouping = this.provider.UpdateGrouping;
                this.SetProperty(ref updateGrouping, value);
                this.provider.UpdateGrouping = updateGrouping;
            }
        }

        /// <inheritdoc cref="ITunesLib.ITunesSongsProvider.UpdateComments" />
        public bool UpdateComments
        {
            get => this.provider.UpdateComments;
            set
            {
                var updateComments = this.provider.UpdateComments;
                this.SetProperty(ref updateComments, value);
                this.provider.UpdateComments = updateComments;
            }
        }
    }
}
