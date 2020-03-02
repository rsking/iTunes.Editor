// -----------------------------------------------------------------------
// <copyright file="MediaInfoTagProvider.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.MediaInfo
{
    /// <summary>
    /// The MediaInfo <see cref="ITagProvider"/>.
    /// </summary>
    public class MediaInfoTagProvider : ITagProvider, IFileProvider
    {
        /// <inheritdoc/>
        public string? File { get; set; }

        /// <inheritdoc/>
        public System.Threading.Tasks.Task<TagLib.Tag?> GetTagAsync(System.Threading.CancellationToken cancellationToken) => System.Threading.Tasks.Task.Run(
            () =>
            {
                var handle = NativeMethods.MediaInfo_New();
                MediaInfoTag? mediaTag = null;
                if (NativeMethods.MediaInfo_Open(handle, this.File))
                {
                    NativeMethods.MediaInfo_Option(handle, "Complete", "0");
                    mediaTag = new MediaInfoTag(System.Runtime.InteropServices.Marshal.PtrToStringAuto(NativeMethods.MediaInfo_Inform(handle)));
                    NativeMethods.MediaInfo_Close(handle);
                }
                else if (NativeMethods.MediaInfoA_Open(handle, this.File))
                {
                    NativeMethods.MediaInfoA_Option(handle, "Complete", "0");
                    mediaTag = new MediaInfoTag(System.Runtime.InteropServices.Marshal.PtrToStringAnsi(NativeMethods.MediaInfoA_Inform(handle)));
                    NativeMethods.MediaInfo_Close(handle);
                }

                NativeMethods.MediaInfo_Delete(handle);

                return mediaTag as TagLib.Tag;
            },
            cancellationToken);
    }
}
