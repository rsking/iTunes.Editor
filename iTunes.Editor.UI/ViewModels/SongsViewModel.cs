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
    public class SongsViewModel : ViewModelBase, Models.ISongs
    {
        private readonly System.Collections.Generic.ICollection<SongInformation> songs = new System.Collections.Generic.List<SongInformation>();

        private readonly System.Collections.Generic.ICollection<Models.IArtist> artists = Collections.ObservableHelper.CreateObservableCollection<Models.IArtist>();

        private SongInformation? selectedSong;

        private TagLib.File? selectedFile;

        private string? progress;

        private int percentage;

        /// <summary>
        /// Initializes a new instance of the <see cref="SongsViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public SongsViewModel(IEventAggregator eventAggregator)
        {
            this.UpdateLyrics = new Microsoft.Toolkit.Mvvm.Input.AsyncRelayCommand(() =>
            {
                var service = Microsoft.Toolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<IUpdateLyricsService>();
                return UpdateSongsAsync(song => service.UpdateAsync(song, this.ForceLyricsSearch, this.ForceLyricsUpdate));
            });
            this.UpdateComposers = new Microsoft.Toolkit.Mvvm.Input.AsyncRelayCommand(() =>
            {
                var service = Microsoft.Toolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<IUpdateComposerService>();
                return UpdateSongsAsync(song => service.UpdateAsync(song, this.ForceComposersSearch));
            });

            eventAggregator?.GetEvent<Models.SongsLoadedEvent>().Subscribe(async evt =>
            {
                this.songs.Clear();
                await foreach (var song in evt.Information.ConfigureAwait(false))
                {
                    if (song.GetMediaKind() == MediaKind.Song)
                    {
                        this.songs.Add(song);
                    }
                }

                var query = this.songs
                    .SelectMany(SelectPerfomers)
                    .GroupBy(p => p.Performer, p => p.Song, StringComparer.Ordinal)
                    .OrderBy(g => g.Key)
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

            async Task UpdateSongsAsync(Func<SongInformation, Task<SongInformation>> processor)
            {
                var selectedSongs = this.GetSelectedSongs().ToArray();
                var count = selectedSongs.Length;
                var current = 0;
                foreach (var song in selectedSongs)
                {
                    // update the UI
                    this.Progress = $"Processing {song.Performers.ToJoinedString()}|{song.Title}";
                    var currentPercentage = 100 * current / count;
                    if (this.percentage != currentPercentage)
                    {
                        this.Percentage = currentPercentage;
                    }

                    await processor(song).ConfigureAwait(false);

                    current++;
                }

                this.Percentage = 0;
                this.Progress = default;
            }
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

        /// <inheritdoc/>
        public System.Windows.Input.ICommand UpdateComposers { get; }

        /// <inheritdoc/>
        public string? Progress
        {
            get => this.progress;
            protected set => this.SetProperty(ref this.progress, value);
        }

        /// <inheritdoc/>
        public int Percentage
        {
            get => this.percentage;
            protected set => this.SetProperty(ref this.percentage, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to force the lyrics search.
        /// </summary>
        public bool ForceLyricsSearch { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to force the lyrics update.
        /// </summary>
        public bool ForceLyricsUpdate { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to force the composer search.
        /// </summary>
        public bool ForceComposersSearch { get; set; } = false;

        private System.Collections.Generic.IEnumerable<SongInformation> GetSelectedSongs()
        {
            var selected = this.Artists
                .Cast<Models.ISelectable>()
                .SelectMany(SelectBase);

            if (selected.Any())
            {
                return selected;
            }

            return this.artists
                .Cast<Models.ISelectable>()
                .SelectMany(selectable => SelectChildren(selectable, forceSelected: true));

            static System.Collections.Generic.IEnumerable<SongInformation> SelectBase(Models.ISelectable selectable)
            {
                return SelectChildren(selectable, forceSelected: false);
            }

            static System.Collections.Generic.IEnumerable<SongInformation> SelectChildren(Models.ISelectable selectable, bool forceSelected)
            {
                if (!selectable.IsSelected && !forceSelected)
                {
                    foreach (var subSelectable in selectable.Children)
                    {
                        foreach (var item in SelectChildren(subSelectable, forceSelected: false))
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
                    foreach (var item in SelectChildren(subSelectable, forceSelected: true))
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
