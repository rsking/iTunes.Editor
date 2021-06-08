// <copyright file="SongsRoutableViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.ViewModels
{
    using System.ComponentModel;

    /// <summary>
    /// The routable <see cref="SongsViewModel"/>.
    /// </summary>
    public class SongsRoutableViewModel : SongsViewModel, ReactiveUI.IRoutableViewModel
    {
        /// <inheritdoc cref="SongsViewModel"/>
        public SongsRoutableViewModel(ReactiveUI.IScreen screen, Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger)
            : base(messenger) => this.HostScreen = screen;

        /// <inheritdoc/>
        public string? UrlPathSegment { get; }

        /// <inheritdoc/>
        public ReactiveUI.IScreen HostScreen { get; }

        /// <inheritdoc/>
        public void RaisePropertyChanged(PropertyChangedEventArgs args) => this.OnPropertyChanged(args);

        /// <inheritdoc/>
        public void RaisePropertyChanging(PropertyChangingEventArgs args) => this.OnPropertyChanging(args);
    }
}
