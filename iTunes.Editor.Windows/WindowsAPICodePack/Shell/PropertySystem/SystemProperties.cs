// <copyright file="SystemProperties.cs" company="RossKing">
// Copyright(c) Microsoft Corporation.All rights reserved.
// </copyright>

namespace Microsoft.WindowsAPICodePack.Shell.PropertySystem
{
    using System;

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
            public static PropertyKey Title
            {
                get
                {
                    var key = new PropertyKey(new Guid("{F29F85E0-4FF9-1068-AB91-08002B27B3D9}"), 2);

                    return key;
                }
            }

            /// <summary>
            /// Music Properties.
            /// </summary>
            public static class Music
            {
                /// <summary>
                /// Gets the genre property key.
                /// <para>Name:     System.Music.Genre -- PKEY_Music_Genre.</para>
                /// <para>Description:
                /// </para>
                /// <para>Type:     Multivalue String -- VT_VECTOR | VT_LPWSTR  (For variants: VT_ARRAY | VT_BSTR).</para>
                /// <para>FormatID: (FMTID_MUSIC) {56A3372E-CE9C-11D2-9F0E-006097C686F6}, 11 (PIDSI_MUSIC_GENRE).</para>
                /// </summary>
                public static PropertyKey Genre
                {
                    get
                    {
                        PropertyKey key = new PropertyKey(new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}"), 11);

                        return key;
                    }
                }

                /// <summary>
                /// Gets the album artist property key.
                /// <para>Name:     System.Music.AlbumArtist -- PKEY_Music_AlbumArtist.</para>
                /// <para>Description:
                /// </para>
                /// <para>Type:     String -- VT_LPWSTR  (For variants: VT_BSTR).</para>
                /// <para>FormatID: (FMTID_MUSIC) {56A3372E-CE9C-11D2-9F0E-006097C686F6}, 13 (PIDSI_MUSIC_ALBUM_ARTIST).</para>
                /// </summary>
                public static PropertyKey AlbumArtist
                {
                    get
                    {
                        var key = new PropertyKey(new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}"), 13);

                        return key;
                    }
                }

                /// <summary>
                /// Gets the album artist property key.
                /// <para>Name:     System.Music.AlbumArtistSortOverride -- PKEY_Music_AlbumArtistSortOverride.</para>
                /// <para>Description:
                /// </para>
                /// <para>Type:     String -- VT_LPWSTR  (For variants: VT_BSTR).</para>
                /// <para>FormatID: (FMTID_MUSIC) {F1FDB4AF-F78C-466C-BB05-56E92DB0B8EC}, 103 (PIDSI_MUSIC_ALBUM_ARTIST_SORT_OVERRIDE).</para>
                /// </summary>
                public static PropertyKey AlbumArtistSortOverride
                {
                    get
                    {
                        var key = new PropertyKey(new Guid("{F1FDB4AF-F78C-466C-BB05-56E92DB0B8EC}"), 103);

                        return key;
                    }
                }

                /// <summary>
                /// Gets the album title property key.
                /// <para>Name:     System.Music.AlbumTitle -- PKEY_Music_AlbumTitle.</para>
                /// <para>Description:
                /// </para>
                /// <para>Type:     String -- VT_LPWSTR  (For variants: VT_BSTR).</para>
                /// <para>FormatID: (FMTID_MUSIC) {56A3372E-CE9C-11D2-9F0E-006097C686F6}, 4 (PIDSI_MUSIC_ALBUM).</para>
                /// </summary>
                public static PropertyKey AlbumTitle
                {
                    get
                    {
                        var key = new PropertyKey(new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}"), 4);

                        return key;
                    }
                }

                /// <summary>
                /// Gets the artist property key.
                /// <para>Name:     System.Music.Artist -- PKEY_Music_Artist.</para>
                /// <para>Description:
                /// </para>
                /// <para>Type:     Multivalue String -- VT_VECTOR | VT_LPWSTR  (For variants: VT_ARRAY | VT_BSTR).</para>
                /// <para>FormatID: (FMTID_MUSIC) {56A3372E-CE9C-11D2-9F0E-006097C686F6}, 2 (PIDSI_MUSIC_ARTIST).</para>
                /// </summary>
                public static PropertyKey Artist
                {
                    get
                    {
                        var key = new PropertyKey(new Guid("{56A3372E-CE9C-11D2-9F0E-006097C686F6}"), 2);

                        return key;
                    }
                }

                /// <summary>
                /// Gets the artist sort overrride property key.
                /// <para>Name:     System.Music.ArtistSortOverride -- PKEY_Music_ArtistSortOverride.</para>
                /// <para>Description:
                /// </para>
                /// <para>Type:     Multivalue String -- VT_VECTOR | VT_LPWSTR  (For variants: VT_ARRAY | VT_BSTR).</para>
                /// <para>FormatID: (FMTID_MUSIC) {DEEB2DB5-0696-4CE0-94FE-A01F77A45FB5}, 102 (PIDSI_MUSIC_ARTIST).</para>
                /// </summary>
                public static PropertyKey ArtistSortOverride
                {
                    get
                    {
                        var key = new PropertyKey(new Guid("{DEEB2DB5-0696-4CE0-94FE-A01F77A45FB5}"), 102);

                        return key;
                    }
                }
            }
        }
    }
}
