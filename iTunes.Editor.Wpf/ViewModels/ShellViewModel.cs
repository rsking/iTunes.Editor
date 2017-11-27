// -----------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ViewModels
{
    /// <summary>
    /// The shell view model.
    /// </summary>
    internal class ShellViewModel : Caliburn.Micro.IHaveDisplayName
    {
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="songsViewModel">The songs view model.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public ShellViewModel(SongsViewModel songsViewModel, IEventAggregator eventAggregator)
        {
            this.Songs = songsViewModel;
            this.eventAggregator = eventAggregator;
        }

        /// <inheritdoc/>
        public string DisplayName { get; set; } = "This is the main window";

        /// <summary>
        /// Gets the songs view model.
        /// </summary>
        public SongsViewModel Songs { get; }

        /// <summary>
        /// Loads iTunes
        /// </summary>
        /// <returns>The task to load iTunes.</returns>
        public async System.Threading.Tasks.Task LoadITunesAsync()
        {
            var iTunesLoader = Caliburn.Micro.IoC.Get<ISongLoader>("itunes");
            var evt = new Models.SongsLoadedEvent(await iTunesLoader.GetTagInformationAsync(null).ConfigureAwait(true));
            this.eventAggregator.Publish(evt);
        }
    }
}
