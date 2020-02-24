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
    public class MediaInfoTagProvider : TagProvider, IFileProvider
    {
        /// <inheritdoc/>
        public string? File { get; set; }

        /// <inheritdoc/>
        public override TagLib.Tag? GetTag()
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

            return mediaTag;
        }
    }
}
