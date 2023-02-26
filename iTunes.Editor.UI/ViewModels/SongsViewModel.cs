// -----------------------------------------------------------------------
// <copyright file="SongsViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ViewModels;

using System.ComponentModel;

using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

/// <summary>
/// The songs view model.
/// </summary>
public partial class SongsViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableRecipient, Models.ISongs, IRecipient<Models.SongsLoadedEvent>
{
    private readonly ICollection<SongInformation> songs = new List<SongInformation>();

    private readonly ICollection<Models.IArtist> artists = Collections.ObservableHelper.CreateObservableCollection<Models.IArtist>();

    private SongInformation? selectedSong;

    private TagLib.File? selectedFile;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private string? progress;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private int percentage;

    [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
    private bool isLoading;

    /// <summary>
    /// Initializes a new instance of the <see cref="SongsViewModel"/> class.
    /// </summary>
    /// <param name="messenger">The messenger.</param>
    public SongsViewModel(IMessenger messenger)
        : base(messenger) => this.Messenger.RegisterAll(this);

    /// <inheritdoc/>
    public IEnumerable<SongInformation> Songs => this.songs;

    /// <inheritdoc/>
    public SongInformation? SelectedSong
    {
        get => this.selectedSong;
        set
        {
            if (this.SetProperty(ref this.selectedSong, value))
            {
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
    }

    /// <inheritdoc/>
    public TagLib.Tag? SelectedTag => this.selectedFile?.Tag;

    /// <summary>
    /// Gets the artists.
    /// </summary>
    public IEnumerable<Models.IArtist> Artists => this.artists;

    /// <summary>
    /// Gets or sets a value indicating whether to force the lyrics search.
    /// </summary>
    public bool ForceLyricsSearch { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to force the lyrics update.
    /// </summary>
    public bool ForceLyricsUpdate { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to force the composers search.
    /// </summary>
    public bool ForceComposersSearch { get; set; }

    /// <inheritdoc />
    async void IRecipient<Models.SongsLoadedEvent>.Receive(Models.SongsLoadedEvent message)
    {
        var recipient = this;
        recipient.songs.Clear();
        await foreach (var song in message.Information.ConfigureAwait(false))
        {
            recipient.IsLoading = true;
            if (song.GetMediaKind() == MediaKind.Song)
            {
                recipient.songs.Add(song);
            }
        }

        var query = recipient.songs
            .SelectMany(SelectPerfomers)
            .GroupBy(p => p.Performer, p => p.Song, StringComparer.Ordinal)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .Select(group => new ArtistViewModel(
                recipient,
                group.Key,
                recipient.songs.Where(song => song.AlbumPerformer.IsNotNullAndContains(group.Key) || song.Performers.Contains(group.Key, StringComparer.Ordinal))));

        recipient.IsLoading = false;

        foreach (var artist in query)
        {
            recipient.artists.Add(artist);
        }

        static IEnumerable<(string Performer, SongInformation Song)> SelectPerfomers(SongInformation song)
        {
            return song.AlbumPerformer is not null
                ? SelectPerfomerImpl(song.AlbumPerformer, song)
                : SelectPerfomersImpl(song.Performers, song);

            static IEnumerable<(string, SongInformation)> SelectPerfomersImpl(IEnumerable<string> performers, SongInformation song)
            {
                return performers.Select(performer => (performer, song));
            }

            static IEnumerable<(string, SongInformation)> SelectPerfomerImpl(string performer, SongInformation song)
            {
                yield return (performer, song);
            }
        }
    }

    [RelayCommand]
    private Task UpdateLyricsAsync()
    {
        var service = CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<IUpdateLyricsService>();
        return this.UpdateSongsAsync(song => service.UpdateAsync(song, this.ForceLyricsSearch, this.ForceLyricsUpdate));
    }

    [RelayCommand]
    private Task UpdateComposersAsync()
    {
        var service = CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<IUpdateComposerService>();
        return this.UpdateSongsAsync(song => service.UpdateAsync(song, this.ForceComposersSearch));
    }

    [RelayCommand]
    private Task UpdateTempoAsync()
    {
        var service = CommunityToolkit.Mvvm.DependencyInjection.Ioc.Default.GetRequiredService<IUpdateTempoService>();
        return this.UpdateSongsAsync(song => service.UpdateAsync(song, force: false));
    }

    private async Task UpdateSongsAsync(Func<SongInformation, ValueTask<SongInformation>> processor)
    {
        var selectedSongs = this.GetSelectedSongs().ToArray();
        var count = selectedSongs.Length;
        var current = 0;
        foreach (var song in selectedSongs)
        {
            // update the UI
            this.Progress = $"Processing {song.Performers.ToJoinedString()}|{song.Title}";
            var currentPercentage = 100 * current / count;
            if (this.Percentage != currentPercentage)
            {
                this.Percentage = currentPercentage;
            }

            _ = await processor(song).ConfigureAwait(false);

            current++;
        }

        this.Percentage = 0;
        this.Progress = default;
    }

    private IEnumerable<SongInformation> GetSelectedSongs()
    {
        var selected = this.Artists
            .Cast<Models.ISelectable>()
            .SelectMany(SelectBase);

        return selected.Any()
            ? selected
            : All();

        IEnumerable<SongInformation> All()
        {
            return this.artists
                .Cast<Models.ISelectable>()
                .SelectMany(selectable => SelectChildren(selectable, forceSelected: true));
        }

        static IEnumerable<SongInformation> SelectBase(Models.ISelectable selectable)
        {
            return SelectChildren(selectable, forceSelected: false);
        }

        static IEnumerable<SongInformation> SelectChildren(Models.ISelectable selectable, bool forceSelected)
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

    private abstract partial class SelectableViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject, Models.ISelectable
    {
        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool isSelected;

        [CommunityToolkit.Mvvm.ComponentModel.ObservableProperty]
        private bool isExpanded;

        protected SelectableViewModel(Models.ISelectable? parent) => this.Parent = parent;

        public IEnumerable<Models.ISelectable> Children { get; protected internal set; } = Enumerable.Empty<Models.ISelectable>();

        public Models.ISelectable? Parent { get; }
    }

    private sealed class ArtistViewModel : SelectableViewModel, Models.IArtist
    {
        private readonly Models.ISongs parent;

        public ArtistViewModel(Models.ISongs parent, string name, IEnumerable<SongInformation> songs)
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

        public IEnumerable<Models.IAlbum> Albums { get; }

        internal void SelectSong(SongInformation songInformation) => this.parent.SelectedSong = songInformation;
    }

    private sealed class AlbumViewModel : SelectableViewModel, Models.IAlbum
    {
        public AlbumViewModel(ArtistViewModel artist, string? name, IEnumerable<SongInformation> songs)
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

        public IEnumerable<Models.ISong> Songs { get; }

        internal void SelectSong(SongInformation songInformation)
        {
            if (this.Parent is ArtistViewModel artist)
            {
                artist.SelectSong(songInformation);
            }
        }
    }

    private sealed class SongViewModel : SelectableViewModel, Models.ISong
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
