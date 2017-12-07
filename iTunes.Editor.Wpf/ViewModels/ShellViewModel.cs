// -----------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ViewModels
{
    using static System.ObservableExtensions;

    /// <summary>
    /// The shell view model.
    /// </summary>
    [PropertyChanged.AddINotifyPropertyChangedInterface]
    internal class ShellViewModel : Caliburn.Micro.IHaveDisplayName
    {
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="load">The load view model.</param>
        /// <param name="songs">The songs view model.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public ShellViewModel(Models.ILoad load, Models.ISongs songs, IEventAggregator eventAggregator)
        {
            this.Load = load;
            this.Songs = songs;
            this.eventAggregator = eventAggregator;
            this.Current = this.Load;

            this.eventAggregator.GetEvent<Models.SongsLoadedEvent>().Subscribe(_ => this.Current = this.Songs);
        }

        /// <inheritdoc/>
        public string DisplayName { get; set; } = Properties.Resources.MainTitle;

        /// <summary>
        /// Gets the load view model.
        /// </summary>
        public Models.ILoad Load { get; }

        /// <summary>
        /// Gets the songs view model.
        /// </summary>
        public Models.ISongs Songs { get; }

        /// <summary>
        /// Gets the current view model.
        /// </summary>
        public object Current { get; private set; }
    }
}
