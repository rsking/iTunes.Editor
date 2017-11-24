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
        /// <inheritdoc/>
        public string DisplayName { get; set; } = "This is the main window";
    }
}
