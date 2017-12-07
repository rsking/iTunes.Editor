// -----------------------------------------------------------------------
// <copyright file="SongsViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ViewModels
{
    using System;

    /// <summary>
    /// The songs view model.
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    public class SongsViewModel : Models.ISongs
    {
        private readonly System.Collections.ObjectModel.ObservableCollection<SongInformation> songs = new System.Collections.ObjectModel.ObservableCollection<SongInformation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SongsViewModel"/> class.
        /// </summary>
        /// <param name="eventAggregator">The event aggregator.</param>
        public SongsViewModel(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<Models.SongsLoadedEvent>().Subscribe(evt =>
            {
                this.songs.Clear();
                foreach (var song in evt.Information)
                {
                    this.songs.Add(song);
                }
            });
        }

        /// <summary>
        /// Gets the songs.
        /// </summary>
        public System.Collections.Generic.IEnumerable<SongInformation> Songs => this.songs;
    }
}
