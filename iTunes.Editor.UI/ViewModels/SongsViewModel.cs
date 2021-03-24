// -----------------------------------------------------------------------
// <copyright file="SongsViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ViewModels
{
    using System;
    using System.Linq;

    /// <summary>
    /// The songs view model.
    /// </summary>
    public class SongsViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject, Models.ISongs
    {
        private readonly System.Collections.ObjectModel.ObservableCollection<SongInformation> songs = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="SongsViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public SongsViewModel(IEventAggregator eventAggregator) => eventAggregator?.GetEvent<Models.SongsLoadedEvent>().Subscribe(async evt =>
        {
            this.songs.Clear();
            await foreach (var song in evt.Information)
            {
                if (song.GetMediaType() == "Music")
                {
                    this.songs.Add(song);
                }
            }

            var performers = this.songs
                .SelectMany(SelectPerfomers).ToArray();
            var groupedPerformers = performers.GroupBy(p => p.Performer);
            var artists = groupedPerformers.Select(group => new ArtistViewModel(group.Key, this.songs.Where(song => song.AlbumPerformers.Contains(group.Key) || song.Performers.Contains(group.Key))));
            this.Artists = artists;

            static System.Collections.Generic.IEnumerable<(string Performer, SongInformation Song)> SelectPerfomers(SongInformation song)
            {
                return song.AlbumPerformers.Any()
                    ? SelectPerfomersImpl(song.AlbumPerformers, song)
                    : SelectPerfomersImpl(song.Performers, song);

                static System.Collections.Generic.IEnumerable<(string, SongInformation)> SelectPerfomersImpl(System.Collections.Generic.IEnumerable<string> performers, SongInformation song)
                {
                    return performers.Select(performer => (performer, song));
                }
            }
        });

        /// <summary>
        /// Gets the songs.
        /// </summary>
        public System.Collections.Generic.IEnumerable<SongInformation> Songs => this.songs;

        /// <summary>
        /// Gets the artists.
        /// </summary>
        public System.Collections.Generic.IEnumerable<Models.IArtist> Artists { get; private set; } = Enumerable.Empty<Models.IArtist>();

        private abstract class SelectableViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject, Models.ISelectable
        {
            private bool isSelected;

            private bool isExpanded;

            protected SelectableViewModel(Models.ISelectable? parent) => this.Parent = parent;

            public bool IsSelected
            {
                get => this.isSelected;
                set => this.SetProperty(ref this.isSelected, value);
            }

            public bool IsExpanded
            {
                get => this.isExpanded;
                set => this.SetProperty(ref this.isExpanded, value);
            }

            public System.Collections.Generic.IEnumerable<Models.ISelectable> Children { get; internal protected set; } = System.Linq.Enumerable.Empty<Models.ISelectable>();

            public Models.ISelectable? Parent { get; }
        }

        private class ArtistViewModel : SelectableViewModel, Models.IArtist
        {
            public ArtistViewModel(string name, System.Collections.Generic.IEnumerable<SongInformation> songs)
                : base(default)
            {
                this.Name = name;
                var viewModels = songs
                    .GroupBy(song => song.Album)
                    .Select(group => new AlbumViewModel(this, group.Key, group));
                this.Children = viewModels;
                this.Albums = viewModels;
            }

            public string? Name { get; }

            public System.Collections.Generic.IEnumerable<Models.IAlbum> Albums { get; }
        }

        private class AlbumViewModel : SelectableViewModel, Models.IAlbum
        {
            public AlbumViewModel(ArtistViewModel artist, string? name, System.Collections.Generic.IEnumerable<SongInformation> songs)
                : base(artist)
            {
                this.Name = name;
                this.Artist = artist;
                var viewModels = songs.Select(song => new SongViewModel(this, song));
                this.Children = viewModels;
                this.Songs = viewModels;
            }

            public string? Name { get; }

            public Models.IArtist Artist { get; }

            public System.Collections.Generic.IEnumerable<Models.ISong> Songs { get; }
        }

        private class SongViewModel : SelectableViewModel, Models.ISong
        {
            private readonly SongInformation song;

            public SongViewModel(AlbumViewModel album, SongInformation song)
                : base(album)
            {
                this.Album = album;
                this.song = song;
            }

            public string? Name => this.song.Title;

            public Models.IAlbum Album { get; }
        }
    }
}
