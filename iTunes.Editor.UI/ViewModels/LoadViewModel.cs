// -----------------------------------------------------------------------
// <copyright file="LoadViewModel.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Toolkit.Mvvm.Messaging;

    /// <summary>
    /// The load view model.
    /// </summary>
    public class LoadViewModel : Microsoft.Toolkit.Mvvm.ComponentModel.ObservableRecipient, Models.ILoad
    {
        private readonly Services.Contracts.IOpenFile openFile;

        private readonly Services.Contracts.ISelectFolder selectFolder;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadViewModel"/> class.
        /// </summary>
        /// <param name="loaders">The loaders.</param>
        /// <param name="openFile">The open file dialog.</param>
        /// <param name="selectFolder">The select folder dialog.</param>
        /// <param name="messenger">The messenger.</param>
        public LoadViewModel(
            IEnumerable<ISongsProvider> loaders,
            Services.Contracts.IOpenFile openFile,
            Services.Contracts.ISelectFolder selectFolder,
            IMessenger messenger)
            : base(messenger)
        {
            this.Providers = loaders.ToArray();
            this.openFile = openFile;
            this.selectFolder = selectFolder;
        }

        /// <inheritdoc />
        public IEnumerable<ISongsProvider> Providers { get; }

        /// <inheritdoc />
        public async System.Threading.Tasks.Task LoadAsync(ISongsProvider provider)
        {
            if (provider is null)
            {
                return;
            }

            switch (provider)
            {
                case IFileProvider fileProvider:
                    fileProvider.File = await this.openFile.GetFileNameAsync(fileProvider.File).ConfigureAwait(false);
                    break;
                case IFolderProvider folderProvider:
                    folderProvider.Folder = await this.selectFolder.GetSelectedPathAsync(folderProvider.Folder).ConfigureAwait(false);
                    break;
            }

            _ = this.Messenger.Send(new Models.SongsLoadedEvent(provider.GetTagInformationAsync()));
        }
    }
}
