// <copyright file="LoadRoutableViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.ViewModels;

using System.ComponentModel;

/// <summary>
/// The routable <see cref="LoadViewModel"/>.
/// </summary>
public class LoadRoutableViewModel : LoadViewModel, ReactiveUI.IRoutableViewModel
{
    /// <inheritdoc cref="LoadViewModel"/>
    public LoadRoutableViewModel(
        ReactiveUI.IScreen screen,
        IEnumerable<ISongsProvider> loaders,
        Services.Contracts.IOpenFile openFile,
        Services.Contracts.ISelectFolder selectFolder,
        Microsoft.Toolkit.Mvvm.Messaging.IMessenger messenger)
        : base(loaders, openFile, selectFolder, messenger) => this.HostScreen = screen;

    /// <inheritdoc/>
    public string? UrlPathSegment { get; }

    /// <inheritdoc/>
    public ReactiveUI.IScreen HostScreen { get; }

    /// <inheritdoc/>
    public void RaisePropertyChanged(PropertyChangedEventArgs args) => this.OnPropertyChanged(args);

    /// <inheritdoc/>
    public void RaisePropertyChanging(PropertyChangingEventArgs args) => this.OnPropertyChanging(args);
}
