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
    public class MediaInfoTagProvider : ITagProvider
    {
        /// <inheritdoc/>
        public TagLib.Tag GetTag(string input)
        {
            var handle = NativeMethods.MediaInfo_New();
            MediaInfoTag mediaTag = null;
            if (NativeMethods.MediaInfo_Open(handle, input))
            {
                NativeMethods.MediaInfo_Option(handle, "Complete", "0");
                mediaTag = new MediaInfoTag(System.Runtime.InteropServices.Marshal.PtrToStringUni(NativeMethods.MediaInfo_Inform(handle)));
                NativeMethods.MediaInfo_Close(handle);
            }

            NativeMethods.MediaInfo_Delete(handle);

            return mediaTag;
        }

        /// <inheritdoc/>
        public System.Threading.Tasks.Task<TagLib.Tag> GetTagAsync(string input)
        {
            return System.Threading.Tasks.Task.Run(() => this.GetTag(input));
        }
    }
}
