// -----------------------------------------------------------------------
// <copyright file="SongsViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// The songs view model.
    /// </summary>
    public class SongsViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject, Models.ISongs
    {
        private readonly System.Collections.Generic.ICollection<SongInformation> songs = new System.Collections.Generic.List<SongInformation>();

        private readonly System.Collections.Generic.ICollection<Models.IArtist> artists = Collections.ObservableHelper.CreateObservableCollection<Models.IArtist>();

        private SongInformation? selectedSong;

        private TagLib.File? selectedFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongsViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public SongsViewModel(IEventAggregator eventAggregator)
        {
            this.UpdateLyrics = new Microsoft.Toolkit.Mvvm.Input.AsyncRelayCommand(async () =>
            {
                var selectedSongs = this.GetSelectedSongs().ToArray();

                var service = Microsoft.Toolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<IUpdateLyricsService>();

                foreach (var song in selectedSongs)
                {
                    await service.UpdateAsync(song).ConfigureAwait(false);
                }
            });

            eventAggregator?.GetEvent<Models.SongsLoadedEvent>().Subscribe(async evt =>
            {
                this.songs.Clear();
                await foreach (var song in evt.Information.ConfigureAwait(false))
                {
                    if (string.Equals(song.GetMediaType(), "Music", StringComparison.Ordinal))
                    {
                        this.songs.Add(song);
                    }
                }

                var query = this.songs
                    .SelectMany(SelectPerfomers)
                    .GroupBy(p => p.Performer, StringComparer.Ordinal)
                    .Select(group => new ArtistViewModel(
                        this,
                        group.Key,
                        this.songs.Where(song => song.AlbumPerformers.Contains(group.Key, StringComparer.Ordinal) || song.Performers.Contains(group.Key, StringComparer.Ordinal))));

                foreach (var artist in query)
                {
                    this.artists.Add(artist);
                }

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
        }

        /// <inheritdoc/>
        public System.Collections.Generic.IEnumerable<SongInformation> Songs => this.songs;

        /// <inheritdoc/>
        public SongInformation? SelectedSong
        {
            get => this.selectedSong;
            set
            {
                this.SetProperty(ref this.selectedSong, value);

                if (this.selectedFile is not null)
                {
                    this.selectedFile.Dispose();
                    this.selectedFile = default;
                }

                if (this.selectedSong is not null)
                {
                    this.selectedFile = TagLib.File.Create(this.selectedSong.Name);
                }

                this.OnPropertyChanged(nameof(this.SelectedTag));
            }
        }

        /// <inheritdoc/>
        public TagLib.Tag? SelectedTag => this.selectedFile?.Tag;


        /// <summary>
        /// Gets the artists.
        /// </summary>
        public System.Collections.Generic.IEnumerable<Models.IArtist> Artists => this.artists;

        /// <inheritdoc/>
        public System.Windows.Input.ICommand UpdateLyrics { get; }

        private System.Collections.Generic.IEnumerable<SongInformation> GetSelectedSongs()
        {
            return this.Artists
                .Cast<SelectableViewModel>()
                .SelectMany(selectable => Select(selectable));

            static System.Collections.Generic.IEnumerable<SongInformation> Select(Models.ISelectable selectable, bool forceSelected = false)
            {
                if (!selectable.IsSelected && !forceSelected)
                {
                    foreach (var subSelectable in selectable.Children)
                    {
                        foreach (var item in Select(subSelectable))
                        {
                            yield return item;
                        }
                    }

                    yield break;
                }

                if (selectable is SongViewModel song)
                {
                    yield return song.Song;
                    yield break;
                }

                foreach (var subSelectable in selectable.Children)
                {
                    foreach (var item in Select(subSelectable, forceSelected: true))
                    {
                        yield return item;
                    }
                }
            }
        }

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

            public System.Collections.Generic.IEnumerable<Models.ISelectable> Children { get; protected internal set; } = Enumerable.Empty<Models.ISelectable>();

            public Models.ISelectable? Parent { get; }
        }

        private class ArtistViewModel : SelectableViewModel, Models.IArtist
        {
            private readonly Models.ISongs parent;

            public ArtistViewModel(Models.ISongs parent, string name, System.Collections.Generic.IEnumerable<SongInformation> songs)
                : base(default)
            {
                this.parent = parent;
                this.Name = name;
                var viewModels = songs
                    .GroupBy(song => song.Album, StringComparer.Ordinal)
                    .Select(group => new AlbumViewModel(this, group.Key, group))
                    .ToArray();
                this.Children = viewModels;
                this.Albums = viewModels;
            }

            public string? Name { get; }

            public System.Collections.Generic.IEnumerable<Models.IAlbum> Albums { get; }

            internal void SelectSong(SongInformation songInformation) => this.parent.SelectedSong = songInformation;
        }

        private class AlbumViewModel : SelectableViewModel, Models.IAlbum
        {
            public AlbumViewModel(ArtistViewModel artist, string? name, System.Collections.Generic.IEnumerable<SongInformation> songs)
                : base(artist)
            {
                this.Name = name;
                this.Artist = artist;
                var viewModels = songs
                    .Select(song => new SongViewModel(this, song))
                    .ToArray();
                this.Children = viewModels;
                this.Songs = viewModels;
            }

            public string? Name { get; }

            public Models.IArtist Artist { get; }

            public System.Collections.Generic.IEnumerable<Models.ISong> Songs { get; }

            internal void SelectSong(SongInformation songInformation)
            {
                if (this.Parent is ArtistViewModel artist)
                {
                    artist.SelectSong(songInformation);
                }
            }
        }

        private class SongViewModel : SelectableViewModel, Models.ISong
        {
            public SongViewModel(AlbumViewModel album, SongInformation song)
                : base(album)
            {
                this.Album = album;
                this.Song = song;
            }

            public string? Name => this.Song.Title;

            public Models.IAlbum Album { get; }

            public SongInformation Song { get; }

            protected override void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (string.Equals(e.PropertyName, nameof(this.IsSelected), StringComparison.Ordinal)
                    && this.Parent is AlbumViewModel viewModel)
                {
                    viewModel.SelectSong(this.Song);
                }

                base.OnPropertyChanged(e);
            }
        }
    }
}
