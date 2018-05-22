// -----------------------------------------------------------------------
// <copyright file="LocalFileAbstraction.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor
{
    /// <summary>
    /// The local file abstraction.
    /// </summary>
    public class LocalFileAbstraction : TagLib.File.IFileAbstraction, System.IDisposable
    {
        private bool disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalFileAbstraction"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="openForWrite">Set to <see langword="true"/> to open for writing.</param>
        public LocalFileAbstraction(string path, bool openForWrite = false)
        {
            this.Name = System.IO.Path.GetFileName(path);
            this.ReadStream = this.WriteStream = openForWrite ? System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite) : System.IO.File.OpenRead(path);
        }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public System.IO.Stream ReadStream { get; }

        /// <inheritdoc/>
        public System.IO.Stream WriteStream { get; }

        /// <inheritdoc/>
        public void CloseStream(System.IO.Stream stream) => stream?.Close();

        /// <inheritdoc/>
        public void Dispose()
        {
            this.Dispose(true);
            System.GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        /// <param name="disposing">Set to <see langword="true"/> to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.CloseStream(this.ReadStream);

                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            this.disposed = true;
        }
    }
}