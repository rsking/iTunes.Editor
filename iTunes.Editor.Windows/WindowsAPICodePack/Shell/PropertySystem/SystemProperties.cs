// <copyright file="SystemProperties.cs" company="RossKing">
// Copyright(c) Microsoft Corporation.All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem;

/// <summary>
/// Provides easy access to all the system properties (property keys and their descriptions).
/// </summary>
public static class SystemProperties
{
    /// <summary>
    /// System Properties.
    /// </summary>
    public static class System
    {
        /// <summary>
        /// Gets the title property key.
        /// <para>Name:     System.Title -- PKEY_Title.</para>
        /// <para>Description: Title of item.
        /// </para>
        /// <para>Type:     String -- VT_LPWSTR  (For variants: VT_BSTR)  Legacy code may treat this as VT_LPSTR.</para>
        /// <para>FormatID: (FMTID_SummaryInformation) {F29F85E0-4FF9-1068-AB91-08002B27B3D9}, 2 (PIDSI_TITLE).</para>
        /// </summary>
        public static PropertyKey Title => new(new Guid("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}"), 2);

        /// <summary>
        /// Music Properties.
        /// </summary>
        public static class Music
        {
            /// <summary>
            /// Gets the artist property key.
            /// <para>Name:     System.Music.Artist -- PKEY_Music_Artist.</para>
            /// <para>Description:
            /// </para>
            /// <para>Type:     Multivalue String -- VT_VECTOR | VT_LPWSTR  (For variants: VT_ARRAY | VT_BSTR).</para>
            /// <para>FormatID: (FMTID_MUSIC) {56A3372E-CE9C-11D2-9F0E-006097C686F6}, 2 (PIDSI_MUSIC_ARTIST).</para>
            /// </summary>
            public static PropertyKey Artist => new(new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}"), 2);

            /// <summary>
            /// Gets the album title property key.
            /// <para>Name:     System.Music.AlbumTitle -- PKEY_Music_AlbumTitle.</para>
            /// <para>Description:
            /// </para>
            /// <para>Type:     String -- VT_LPWSTR  (For variants: VT_BSTR).</para>
            /// <para>FormatID: (FMTID_MUSIC) {56A3372E-CE9C-11D2-9F0E-006097C686F6}, 4 (PIDSI_MUSIC_ALBUM).</para>
            /// </summary>
            public static PropertyKey AlbumTitle => new(new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}"), 4);

            /// <summary>
            /// Gets the track number property key.
            /// <para>Name:     System.Music.TrackNumber -- PKEY_Music_TrackNumber</para>
            /// <para>Description:
            /// </para>
            /// <para>Type:     UInt32.</para>
            /// <para>FormatID: (FMTID_MUSIC) {56A3372E-CE9C-11D2-9F0E-006097C686F6}, 7 (PIDSI_MUSIC_TRACK_NUMBER).</para>
            /// </summary>
            public static PropertyKey TrackNumber => new(new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}"), 7);

            /// <summary>
            /// Gets the genre property key.
            /// <para>Name:     System.Music.Genre -- PKEY_Music_Genre.</para>
            /// <para>Description:
            /// </para>
            /// <para>Type:     Multivalue String -- VT_VECTOR | VT_LPWSTR  (For variants: VT_ARRAY | VT_BSTR).</para>
            /// <para>FormatID: (FMTID_MUSIC) {56A3372E-CE9C-11D2-9F0E-006097C686F6}, 11 (PIDSI_MUSIC_GENRE).</para>
            /// </summary>
            public static PropertyKey Genre => new(new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}"), 11);

            /// <summary>
            /// Gets the album artist property key.
            /// <para>Name:     System.Music.AlbumArtist -- PKEY_Music_AlbumArtist.</para>
            /// <para>Description:
            /// </para>
            /// <para>Type:     String -- VT_LPWSTR  (For variants: VT_BSTR).</para>
            /// <para>FormatID: (FMTID_MUSIC) {56A3372E-CE9C-11D2-9F0E-006097C686F6}, 13 (PIDSI_MUSIC_ALBUM_ARTIST).</para>
            /// </summary>
            public static PropertyKey AlbumArtist => new(new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}"), 13);

            /// <summary>
            /// Gets the artist sort overrride property key.
            /// <para>Name:     System.Music.ArtistSortOverride -- PKEY_Music_ArtistSortOverride.</para>
            /// <para>Description:
            /// </para>
            /// <para>Type:     Multivalue String -- VT_VECTOR | VT_LPWSTR  (For variants: VT_ARRAY | VT_BSTR).</para>
            /// <para>FormatID: (FMTID_MUSIC) {DEEB2DB5-0696-4CE0-94FE-A01F77A45FB5}, 102 (PIDSI_MUSIC_ARTIST).</para>
            /// </summary>
            public static PropertyKey ArtistSortOverride => new(new Guid("{DEEB2DB5-0696-4CE0-94FE-A01F77A45FB5}"), 102);

            /// <summary>
            /// Gets the album artist property key.
            /// <para>Name:     System.Music.AlbumArtistSortOverride -- PKEY_Music_AlbumArtistSortOverride.</para>
            /// <para>Description:
            /// </para>
            /// <para>Type:     String -- VT_LPWSTR  (For variants: VT_BSTR).</para>
            /// <para>FormatID: (FMTID_MUSIC) {F1FDB4AF-F78C-466C-BB05-56E92DB0B8EC}, 103 (PIDSI_MUSIC_ALBUM_ARTIST_SORT_OVERRIDE).</para>
            /// </summary>
            public static PropertyKey AlbumArtistSortOverride => new(new Guid("{F1FDB4AF-F78C-466C-BB05-56E92DB0B8EC}"), 103);

            /// <summary>
            /// Gets the disc number property key.
            /// <para>Name:     System.Music.DiscNumber -- PKEY_Music_DiscNumber.</para>
            /// <para>Description:
            /// </para>
            /// <para>Type:     UInt32.</para>
            /// <para>FormatID: (FMTID_MUSIC) {F1FDB4AF-F78C-466C-BB05-56E92DB0B8EC}, 104 (PIDSI_MUSIC_DISK_NUMBER).</para>
            /// </summary>
            public static PropertyKey DiscNumber => new(new Guid("{F1FDB4AF-F78C-466C-BB05-56E92DB0B8EC}"), 104);
        }
    }
}
