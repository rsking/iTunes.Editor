// -----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="RossKing">
// Copyright (c) RossKing. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ITunes.Editor.MediaInfo
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Native methods.
    /// </summary>
    internal static class NativeMethods
    {
        /// <summary>
        /// The kinds of streams.
        /// </summary>
        public enum StreamKind
        {
            /// <summary>
            /// General stream.
            /// </summary>
            General,

            /// <summary>
            /// Video stream.
            /// </summary>
            Video,

            /// <summary>
            /// Audio stream.
            /// </summary>
            Audio,

            /// <summary>
            /// Text stream.
            /// </summary>
            Text,

            /// <summary>
            /// Other stream.
            /// </summary>
            Other,

            /// <summary>
            /// Image stream.
            /// </summary>
            Image,

            /// <summary>
            /// Menu stream.
            /// </summary>
            Menu,
        }

        /// <summary>
        /// The kinds of infomration.
        /// </summary>
        public enum InfoKind
        {
            /// <summary>
            /// Name information.
            /// </summary>
            Name,

            /// <summary>
            /// Text information.
            /// </summary>
            Text,

            /// <summary>
            /// Measure information.
            /// </summary>
            Measure,

            /// <summary>
            /// Options information.
            /// </summary>
            Options,

            /// <summary>
            /// Name/Text information.
            /// </summary>
            NameText,

            /// <summary>
            /// Measure/Text information.
            /// </summary>
            MeasureText,

            /// <summary>
            /// Info information.
            /// </summary>
            Info,

            /// <summary>
            /// How-to information.
            /// </summary>
            HowTo
        }

        /// <summary>
        /// The information options.
        /// </summary>
        public enum InfoOptions
        {
            /// <summary>
            /// Show in inform.
            /// </summary>
            ShowInInform,

            /// <summary>
            /// Support information option.
            /// </summary>
            Support,

            /// <summary>
            /// Show in supported.
            /// </summary>
            ShowInSupported,

            /// <summary>
            /// Type of value.
            /// </summary>
            TypeOfValue
        }

        /// <summary>
        /// The information file options.
        /// </summary>
        public enum InfoFileOptions
        {
            /// <summary>
            /// File option nothing.
            /// </summary>
            FileOption_Nothing = 0x00,

            /// <summary>
            /// File option no recursive.
            /// </summary>
            FileOption_NoRecursive = 0x01,

            /// <summary>
            /// File option close all.
            /// </summary>
            FileOption_CloseAll = 0x02,

            /// <summary>
            /// File option maximum.
            /// </summary>
            FileOption_Max = 0x04
        }

        /// <summary>
        /// The status.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// Status of none.
            /// </summary>
            None = 0x00,

            /// <summary>
            /// Status of accepted.
            /// </summary>
            Accepted = 0x01,

            /// <summary>
            /// Status of filled.
            /// </summary>
            Filled = 0x02,

            /// <summary>
            /// Status of updated.
            /// </summary>
            Updated = 0x04,

            /// <summary>
            /// Status of finalized.
            /// </summary>
            Finalized = 0x08,
        }

        /// <summary>
        /// Creates a new instance of <c>MediaInfo</c>.
        /// </summary>
        /// <returns>The new instance of <c>MediaInfo</c>.</returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_New();

        /// <summary>
        /// Deletes the specified <c>MediaInfo</c>.
        /// </summary>
        /// <param name="Handle">The handle to delete.</param>
        [DllImport("MediaInfo.dll")]
        public static extern void MediaInfo_Delete(IntPtr Handle);

        /// <summary>
        /// Opens the file name.
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="FileName">The file name to open.</param>
        /// <returns><see langword="true"/> if the file is opened; otherwise <see langword="false"/></returns>
        [DllImport("MediaInfo.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool MediaInfo_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string FileName);

        /// <summary>
        /// Opens the file name.
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="FileName">The file name to open.</param>
        /// <returns><see langword="true"/> if the file is opened; otherwise <see langword="false"/></returns>
        [DllImport("MediaInfo.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool MediaInfoA_Open(IntPtr Handle, [MarshalAs(UnmanagedType.LPStr)] string FileName);

        /// <summary>
        /// Open a stream and collect information about it (technical information and tags).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="File_Size">Estimated file size.</param>
        /// <param name="File_Offset">Offset of the file (if we don't have the beginning of the file).</param>
        /// <returns><see langword="true"/> if the file is opened; otherwise <see langword="false"/></returns>
        [DllImport("MediaInfo.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool MediaInfo_Open_Buffer_Init(IntPtr Handle, long File_Size, long File_Offset);

        /// <summary>
        /// Open a file and collect information about it (technical information and tags)
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="File_Size">Estimated file size.</param>
        /// <param name="File_Offset">Offset of the file (if we don't have the beginning of the file).</param>
        /// <returns><see langword="true"/> if the file is opened; otherwise <see langword="false"/></returns>
        [DllImport("MediaInfo.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool MediaInfoA_Open(IntPtr Handle, long File_Size, long File_Offset);

        /// <summary>
        /// Open a stream and collect information about it (technical information and tags).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="Buffer">pointer to the stream </param>
        /// <param name="Buffer_Size">Count of bytes to read.</param>
        /// <returns>a bitfield
        /// <para>bit 0: Is Accepted(format is known)</para>
        /// <para>bit 1: Is Filled(main data is collected) </para>
        /// <para>bit 2: Is Updated(some data have beed updated, example: duration for a real time MPEG-TS stream) </para>
        /// <para>bit 3: Is Finalized(No more data is needed, will not use further data) </para>
        /// <para>bit 4-15: Reserved bit </para>
        /// <para>16-31: User defined</para>
        /// </returns>
        [DllImport("MediaInfo.dll")]
        public static extern int MediaInfo_Open_Buffer_Continue(IntPtr Handle, byte[] Buffer, int Buffer_Size);

        /// <summary>
        /// Open a stream and collect information about it (technical information and tags).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="File_Size">The file size.</param>
        /// <param name="Buffer">pointer to the stream </param>
        /// <param name="Buffer_Size">Count of bytes to read.</param>
        /// <returns>a bitfield
        /// <para>bit 0: Is Accepted(format is known)</para>
        /// <para>bit 1: Is Filled(main data is collected) </para>
        /// <para>bit 2: Is Updated(some data have beed updated, example: duration for a real time MPEG-TS stream) </para>
        /// <para>bit 3: Is Finalized(No more data is needed, will not use further data) </para>
        /// <para>bit 4-15: Reserved bit </para>
        /// <para>16-31: User defined</para>
        /// </returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoA_Open_Buffer_Continue(IntPtr Handle, long File_Size, byte[] Buffer, int Buffer_Size);

        /// <summary>
        /// Open a stream and collect information about it (technical information and tags).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <returns>the needed offset of the file or File size if no more bytes are needed</returns>
        [DllImport("MediaInfo.dll")]
        public static extern long MediaInfo_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);

        /// <summary>
        /// Open a stream and collect information about it (technical information and tags).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <returns>the needed offset of the file or File size if no more bytes are needed</returns>
        [DllImport("MediaInfo.dll")]
        public static extern long MediaInfoA_Open_Buffer_Continue_GoTo_Get(IntPtr Handle);

        /// <summary>
        /// Open a stream and collect information about it (technical information and tags)
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <returns><see langword="true"/> if successful; otherwise <see langword="false"/>.</returns>
        [DllImport("MediaInfo.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool MediaInfo_Open_Buffer_Finalize(IntPtr Handle);

        /// <summary>
        /// Open a stream and collect information about it (technical information and tags)
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <returns><see langword="true"/> if successful; otherwise <see langword="false"/>.</returns>
        [DllImport("MediaInfo.dll")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool MediaInfoA_Open_Buffer_Finalize(IntPtr Handle);

        /// <summary>
        /// Close a file opened before with <see cref="MediaInfo_Open(IntPtr, string)"/> (without saving).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        [DllImport("MediaInfo.dll")]
        public static extern void MediaInfo_Close(IntPtr Handle);

        /// <summary>
        /// Get all details about a file in one string.
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="Reserved">Reserved, do not use.</param>
        /// <returns>Text with information about the file.</returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Inform(IntPtr Handle, int Reserved = 0);

        /// <summary>
        /// Get all details about a file in one string.
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="Reserved">Reserved, do not use.</param>
        /// <returns>Text with information about the file.</returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoA_Inform(IntPtr Handle, int Reserved = 0);

        /// <summary>
        /// Get a piece of information about a file (parameter is am integer).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="StreamKind">Kind of stream (general, video, audio...)</param>
        /// <param name="StreamNumber">Stream number in Kind of stream (first, second...).</param>
        /// <param name="Parameter">
        /// <para>Parameter you are looking for in the stream (Codec, width, bitrate...), in integer format (first parameter, second parameter...)</para>
        /// <para>This integer is arbitarily assigned by the library, so its consistency should not be relied on, but is useful when looping through all the parameters</para>
        /// </param>
        /// <param name="KindOfInfo">Kind of information you want about the parameter (the text, the measure, the help...).</param>
        /// <returns>a string about information you search.</returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_GetI(IntPtr Handle, StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo);

        /// <summary>
        /// Get a piece of information about a file (parameter is am integer).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="StreamKind">Kind of stream (general, video, audio...)</param>
        /// <param name="StreamNumber">Stream number in Kind of stream (first, second...).</param>
        /// <param name="Parameter">
        /// <para>Parameter you are looking for in the stream (Codec, width, bitrate...), in integer format (first parameter, second parameter...)</para>
        /// <para>This integer is arbitarily assigned by the library, so its consistency should not be relied on, but is useful when looping through all the parameters</para>
        /// </param>
        /// <param name="KindOfInfo">Kind of information you want about the parameter (the text, the measure, the help...).</param>
        /// <returns>a string about information you search.</returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoA_GetI(IntPtr Handle, StreamKind StreamKind, int StreamNumber, int Parameter, InfoKind KindOfInfo);

        /// <summary>
        /// Get a piece of information about a file (parameter is a string).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="StreamKind">Kind of stream (general, video, audio...)</param>
        /// <param name="StreamNumber">Stream number in Kind of stream (first, second...).</param>
        /// <param name="Parameter">
        /// <para>Parameter you are looking for in the stream (Codec, width, bitrate...), in integer format (first parameter, second parameter...)</para>
        /// <para>This integer is arbitarily assigned by the library, so its consistency should not be relied on, but is useful when looping through all the parameters</para>
        /// </param>
        /// <param name="KindOfInfo">Kind of information you want about the parameter (the text, the measure, the help...).</param>
        /// <param name="KindOfSearch">Where to look for the parameter</param>
        /// <returns>a string about information you search.</returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Get(IntPtr Handle, StreamKind StreamKind, int StreamNumber, [MarshalAs(UnmanagedType.LPWStr)] string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch);

        /// <summary>
        /// Get a piece of information about a file (parameter is a string).
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="StreamKind">Kind of stream (general, video, audio...)</param>
        /// <param name="StreamNumber">Stream number in Kind of stream (first, second...).</param>
        /// <param name="Parameter">
        /// <para>Parameter you are looking for in the stream (Codec, width, bitrate...), in integer format (first parameter, second parameter...)</para>
        /// <para>This integer is arbitarily assigned by the library, so its consistency should not be relied on, but is useful when looping through all the parameters</para>
        /// </param>
        /// <param name="KindOfInfo">Kind of information you want about the parameter (the text, the measure, the help...).</param>
        /// <param name="KindOfSearch">Where to look for the parameter</param>
        /// <returns>a string about information you search.</returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoA_Get(IntPtr Handle, StreamKind StreamKind, int StreamNumber, [MarshalAs(UnmanagedType.LPStr)] string Parameter, InfoKind KindOfInfo, InfoKind KindOfSearch);

        /// <summary>
        /// Configure or get information about.
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="Option">The name of option.</param>
        /// <param name="Value">The value of option.</param>
        /// <returns>Depend of the option: by default "" (nothing) means No, other means Yes </returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPWStr)] string Option, [MarshalAs(UnmanagedType.LPWStr)] string Value = "");

        /// <summary>
        /// Configure or get information about.
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="Option">The name of option.</param>
        /// <param name="Value">The value of option.</param>
        /// <returns>Depend of the option: by default "" (nothing) means No, other means Yes </returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfoA_Option(IntPtr Handle, [MarshalAs(UnmanagedType.LPStr)] string Option, [MarshalAs(UnmanagedType.LPStr)] string Value);

        /// <summary>
        /// Get the state of the library .
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <returns>The return value.</returns>
        [DllImport("MediaInfo.dll")]
        public static extern IntPtr MediaInfo_State_Get(IntPtr Handle);

        /// <summary>
        /// Count of streams of a stream kind (StreamNumber not filled), or count of piece of information in this stream.
        /// </summary>
        /// <param name="Handle">The <c>MediaInfo</c> handle.</param>
        /// <param name="StreamKind">Kind of stream (general, video, audio...).</param>
        /// <param name="StreamNumber">Stream number in this kind of stream (first, second...).</param>
        /// <returns>The count of fields for this stream kind / stream number if stream number is provided, else the count of streams for this stream kind.</returns>
        [DllImport("MediaInfo.dll")]
        public static extern int MediaInfo_Count_Get(IntPtr Handle, StreamKind StreamKind, int StreamNumber);
    }
}
