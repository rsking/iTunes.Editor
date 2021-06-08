// <copyright file="ShellViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>

namespace ITunes.Editor.ViewModels
{
    /// <summary>
    /// The shell view model.
    /// </summary>
    public class ShellViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableObject, ReactiveUI.IScreen
    {
        /// <inheritdoc/>
        /// <remarks>Required by the <see cref="ReactiveUI.IScreen"/> interface.</remarks>
        public ReactiveUI.RoutingState Router { get; } = new ReactiveUI.RoutingState();
    }
}
