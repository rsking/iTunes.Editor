// -----------------------------------------------------------------------
// <copyright file="ShellViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The main window view model.
    /// </summary>
    internal class ShellViewModel : ViewModelBase, Models.IShell
    {
        private readonly IEventAggregator eventAggregator;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        public ShellViewModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="load">The load model.</param>
        /// <param name="songs">The sonds model.</param>
        /// <param name="eventAggregator">The event aggregator.</param>
        public ShellViewModel(Models.ILoad load, Models.ISongs songs, IEventAggregator eventAggregator)
        {
            this.Load = load;
            this.Songs = songs;
            this.eventAggregator = eventAggregator;
            this.Current = this.Load;

            this.eventAggregator.GetEvent<Models.SongsLoadedEvent>().Subscribe(_ => this.Current = this.Songs);
        }

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
